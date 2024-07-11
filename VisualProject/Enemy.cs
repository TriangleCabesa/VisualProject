using System.Collections.Generic;

namespace VisualProject
{
    public class Enemy : IGameObject, ICollidable
    {
        public static Player? Player;
        private (double X, double Y) _location;
        private (double X, double Y) _direction;
        private double _moveDistance;
        private double _movementSpeedMultiplier;
        private double _rotation;
        private List<Polygon> _collisionBox;
        private TimeSpan _explosionTimer;
        private Bitmap _playerSprite = new(Image.FromFile(Directory.GetCurrentDirectory().Split("VisualProject").First() + @"VisualProject\VisualProject\Sprites\EnemySprite.png"));
        private int _opacity;
        private int _explosionNumber = 0;
        private int _explosionFrameRate = 15;

        private readonly List<Point> _hitboxPoints =
        [
            new(0, 9),
            new(0, 5),
            new(5, 0),
            new(10, 5),
            new(10, 9),
        ];

        public Enemy(Rectangle spawnWindow)
        {
            _location = GetSafeSpawnPoint(spawnWindow);
            _direction = GetUpdatedDirection();
            _collisionBox = GetObjectSprite();
            _movementSpeedMultiplier = ((double)(new Random().Next(100)) / 100) + 1;
        }

        /// <inheritdoc/>
        public List<Polygon> GetObjectSprite()
        {
            int size = _explosionNumber > 0 ? 3 : 5;
            _explosionNumber = Math.Min(_explosionNumber, 7);

            if (_explosionNumber > 0)
                _playerSprite = new(Image.FromFile(Directory.GetCurrentDirectory().Split("VisualProject").First() + @$"VisualProject\VisualProject\Sprites\Explosion{_explosionNumber}.png"));

            List<Polygon> result = ImageConverter.GetPolygonsFromImage(new Point((int)_location.X,(int)_location.Y), _playerSprite, size, opacity: _opacity);

            for (int i = 0; i < result.Count; i++)
                for (int j = 0; j < result[i].Points.Count; j++)
                    result[i].Points[j] = result[i].Points[j].Rotate(new Point((int)_location.X, (int)_location.Y), _rotation);

            if (_explosionNumber > 0)
                return result;

            Polygon polygon = new();

            foreach (var point in _hitboxPoints)
                polygon.Points.Add(point.GetActualPointFromTargetPoint(new Point((int)_location.X, (int)_location.Y), _playerSprite, 5, 0));

            _collisionBox = [polygon];

            return result;
        }

        /// <inheritdoc/>
        public bool Update(List<(Keys key, TimeSpan time)> pressedTimers, List<IGameObject> gameObjects)
        {
            if (_explosionNumber > 0)
            {
                UpdateExplosionNumber(pressedTimers);

                return _explosionNumber <= 7;
            }

            if (_opacity < 255)
                _opacity ++;

            foreach (var gameObject in gameObjects.Where(x => x is ICollidable))
            {
                if (gameObject is not ICollidable collidable || collidable is Enemy)
                    continue;

                if (collidable.CollidesWith(_collisionBox))
                {
                    _explosionNumber = 1;
                    _opacity = -1;
                    // Player.Health -= Enemy.Damage;

                    return true;
                }
            }

            double lastX = _location.X;
            double lastY = _location.Y;

            _direction = GetUpdatedDirection();
            _moveDistance = pressedTimers.First(x => x.key == Keys.F20).time.TotalMilliseconds / 20;
            _moveDistance *= _movementSpeedMultiplier;
            _location.X += _moveDistance * _direction.X;
            _location.Y += _moveDistance * _direction.Y;

            if (lastX != _location.X || lastY != _location.Y)
            {
                var deltaX = lastX - _location.X;
                var deltaY = lastY - _location.Y;

                _rotation = Math.Atan2(-deltaX, deltaY);
            }

            return true;
        }

        /// <inheritdoc/>
        public bool CollidesWith(List<Polygon> polygons) =>
            CollisionDetector.CollidesWith(_collisionBox, polygons);

        private double GetMinimumSpawnDistance(Rectangle spawnWindow)
        {
            double maxDistance = Math.Sqrt(Math.Pow(Player!.Location.X, 2) + Math.Pow(Player.Location.Y, 2));
            maxDistance = Math.Max(maxDistance, Math.Sqrt(Math.Pow(Player.Location.X, 2) + Math.Pow(Player.Location.Y - spawnWindow.Height, 2)));
            maxDistance = Math.Max(maxDistance, Math.Sqrt(Math.Pow(Player.Location.X - spawnWindow.Width, 2) + Math.Pow(Player.Location.Y - spawnWindow.Height, 2)));
            maxDistance = Math.Max(maxDistance, Math.Sqrt(Math.Pow(Player.Location.X - spawnWindow.Width, 2) + Math.Pow(Player.Location.Y, 2)));

            // At this point, maxDistance represents which corner of the spawn window the player object is the furthest from.
            // If there is no space within the spawn window further than 1000 units away, a minimum distance of 500 is used instead the max distance divided by two.

            return maxDistance < 1000 ? maxDistance / 2 : 500;
        }

        private (double X, double Y) GetSafeSpawnPoint(Rectangle spawnWindow)
        {
            double hypotenuse = 0;
            double x = 0;
            double y = 0;

            double minimumSpawnDistance = GetMinimumSpawnDistance(spawnWindow);

            while (hypotenuse < minimumSpawnDistance || (x >= 0 && x <= spawnWindow.Width) || (y >= 0 && y <= spawnWindow.Height))
            {
                Random random = new();
                x = random.Next(spawnWindow.Width + 1000) - 500;
                y = random.Next(spawnWindow.Height + 1000) - 500;

                hypotenuse = Math.Sqrt(Math.Pow(Player!.Location.X - x, 2) + Math.Pow(Player.Location.Y - y, 2));
            }

            return (x, y);
        }

        private (double X, double Y) GetUpdatedDirection()
        {
            double xDiff = Player!.Location.X - _location.X;
            double yDiff = Player.Location.Y - _location.Y;

            double hypotenuse = Math.Sqrt(Math.Pow(Player.Location.X - _location.X, 2) + Math.Pow(Player.Location.Y - _location.Y, 2));

            return (xDiff / hypotenuse, yDiff / hypotenuse);
        }

        private void UpdateExplosionNumber(List<(Keys key, TimeSpan time)> pressedTimers)
        {
            _explosionTimer += pressedTimers.First(x => x.key == Keys.F20).time;

            while (_explosionTimer > TimeSpan.FromMilliseconds(1000 / _explosionFrameRate))
            {
                _explosionTimer -= TimeSpan.FromMilliseconds(1000 / _explosionFrameRate);
                _explosionNumber++;
            }
        }
    }
}
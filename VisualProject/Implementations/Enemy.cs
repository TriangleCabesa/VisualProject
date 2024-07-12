using VisualProject.Extensions;
using VisualProject.HelperClasses;
using VisualProject.Interfaces;

namespace VisualProject.Implementations
{
    public class Enemy : IGameObject, ICollidable, IDamagable
    {
        private static Player? _player;
        private (double X, double Y) _location;
        private (double X, double Y) _direction;
        private double _moveDistance;
        private double _movementSpeedMultiplier;
        private double _rotation;
        private Bitmap _playerSprite = new(Image.FromFile(Directory.GetCurrentDirectory().Split("VisualProject").First() + @"VisualProject\VisualProject\Sprites\EnemySprite.png"));
        private int _opacity;
        private readonly int _damage = 1;

        private int _health = 1;
        private int Health 
        {
            get
            {
                return _health;
            }
            set
            {
                _health = value;

                if (_health < 1)
                    _explosionHandler = new();
            }
        }

        private ExplosionHandler? _explosionHandler;

        private readonly List<Point> _hitboxPoints =
        [
            new(0, 9),
            new(0, 5),
            new(5, 0),
            new(10, 5),
            new(10, 9),
        ];

        public List<Polygon> CollisionBox { get; private set; }

        public bool IsAlive
        {
            get
            {
                if (Health <= 0)
                    _explosionHandler = new();

                return Health > 0;
            }
        }

        public int BodyDamage { get => _damage; }

        public Enemy(Rectangle spawnWindow, int damage = 1, int health = 1)
        {
            _damage = damage;
            Health = health;
            _location = GetSafeSpawnPoint(spawnWindow);
            _direction = GetUpdatedDirection();
            CollisionBox = GetCollisionBox();
            _movementSpeedMultiplier = (double)new Random().Next(100) / 100 + 1;
        }

        /// <summary>
        /// Set the player object for all enemies to use.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> object of the game.</param>
        public static void SetPlayer(Player player) => _player = player;

        /// <inheritdoc/>
        public List<Polygon> GetObjectSprite()
        {
            if (_explosionHandler is not null)
            {
                CollisionBox = [];

                return _explosionHandler.GetExplosionSprite(new Point((int)_location.X, (int)_location.Y), _rotation);
            }

            List<Polygon> result = ImageReader.GetPolygonsFromImage(new Point((int)_location.X, (int)_location.Y), _playerSprite, 5, opacity: _opacity);

            for (int i = 0; i < result.Count; i++)
                for (int j = 0; j < result[i].Points.Count; j++)
                    result[i].Points[j] = result[i].Points[j].Rotate(new Point((int)_location.X, (int)_location.Y), _rotation);

            Polygon polygon = new();

            foreach (var point in _hitboxPoints)
                polygon.Points.Add(point.GetActualPointFromTargetPoint(new Point((int)_location.X, (int)_location.Y), _playerSprite, 5, 0));

            CollisionBox = [polygon];

            return result;
        }

        /// <inheritdoc/>
        public bool Update(List<(Keys key, TimeSpan time)> pressedTimers, List<IGameObject> gameObjects)
        {
            if (_explosionHandler is not null)
            {
                _explosionHandler.UpdateExplosionNumber(pressedTimers);

                return _explosionHandler.ExplosionNotComplete;
            }

            if (_opacity < 255)
                _opacity++;

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
        public bool CollidesWith(ICollidable collidable) =>
            CollisionDetector.CollidesWith(CollisionBox, collidable.CollisionBox);

        /// <inheritdoc/>
        public void TakeDamage(IProjectile projectile) => Health -= projectile.Damage;

        /// <inheritdoc/>
        public void TakeDamage(IDamagable damagable) => Health -= damagable.BodyDamage;

        /// <inheritdoc/>
        public void DealDamage(IDamagable damagable) => damagable.TakeDamage(this);

        private static double GetMinimumSpawnDistance(Rectangle spawnWindow)
        {
            double maxDistance = Math.Sqrt(Math.Pow(_player!.Location.X, 2) + Math.Pow(_player.Location.Y, 2));
            maxDistance = Math.Max(maxDistance, Math.Sqrt(Math.Pow(_player.Location.X, 2) + Math.Pow(_player.Location.Y - spawnWindow.Height, 2)));
            maxDistance = Math.Max(maxDistance, Math.Sqrt(Math.Pow(_player.Location.X - spawnWindow.Width, 2) + Math.Pow(_player.Location.Y - spawnWindow.Height, 2)));
            maxDistance = Math.Max(maxDistance, Math.Sqrt(Math.Pow(_player.Location.X - spawnWindow.Width, 2) + Math.Pow(_player.Location.Y, 2)));

            // At this point, maxDistance represents which corner of the spawn window the player object is the furthest from.
            // If there is no space within the spawn window further than 1000 units away, a minimum distance of 500 is used instead the max distance divided by two.

            return maxDistance < 1000 ? maxDistance / 2 : 500;
        }

        private static (double X, double Y) GetSafeSpawnPoint(Rectangle spawnWindow)
        {
            double hypotenuse = 0;
            double x = 0;
            double y = 0;
            double minimumSpawnDistance = GetMinimumSpawnDistance(spawnWindow);

            while (hypotenuse < minimumSpawnDistance || x >= 0 && x <= spawnWindow.Width || y >= 0 && y <= spawnWindow.Height)
            {
                Random random = new();
                x = random.Next(spawnWindow.Width + 1000) - 500;
                y = random.Next(spawnWindow.Height + 1000) - 500;

                hypotenuse = Math.Sqrt(Math.Pow(_player!.Location.X - x, 2) + Math.Pow(_player.Location.Y - y, 2));
            }

            return (x, y);
        }

        private (double X, double Y) GetUpdatedDirection()
        {
            double xDiff = _player!.Location.X - _location.X;
            double yDiff = _player.Location.Y - _location.Y;

            double hypotenuse = Math.Sqrt(Math.Pow(_player.Location.X - _location.X, 2) + Math.Pow(_player.Location.Y - _location.Y, 2));

            return (xDiff / hypotenuse, yDiff / hypotenuse);
        }

        private List<Polygon> GetCollisionBox()
        {
            Polygon polygon = new();

            foreach (var point in _hitboxPoints)
                polygon.Points.Add(point.GetActualPointFromTargetPoint(new Point((int)_location.X, (int)_location.Y), _playerSprite, 5, 0));

            return [polygon];
        }

        public void HandleCollision(ICollidable collidable)
        {
            if (collidable is not Player player)
                return;

            DealDamage(player);
        }
    }
}
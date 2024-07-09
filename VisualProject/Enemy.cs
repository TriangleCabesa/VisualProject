namespace VisualProject
{
    public class Enemy : IGameObject, ICollidable
    {
        private (double X, double Y) _location;
        private (double X, double Y) _direction = (0, 0);
        private Player _player;
        private double _moveDistance = 0;
        private List<Polygon> _collisionBox = [];

        public Enemy(Player player, Rectangle spawnWindow)
        {
            _player = player;

            double hypotenuse = 0;
            double xDiff = 0;
            double yDiff = 0;

            double maxDistance = Math.Sqrt(Math.Pow(_player.X - 0, 2) + Math.Pow(_player.Y - 0, 2));
            maxDistance = Math.Max(maxDistance, Math.Sqrt(Math.Pow(_player.X - 0, 2) + Math.Pow(_player.Y - spawnWindow.Height, 2)));
            maxDistance = Math.Max(maxDistance, Math.Sqrt(Math.Pow(_player.X - spawnWindow.Width, 2) + Math.Pow(_player.Y - spawnWindow.Height, 2)));
            maxDistance = Math.Max(maxDistance, Math.Sqrt(Math.Pow(_player.X - spawnWindow.Width, 2) + Math.Pow(_player.Y - 0, 2)));
            
            double minimumSpawnDistance = maxDistance < 1000 ? maxDistance * 0.5 : 500;

            while (hypotenuse < minimumSpawnDistance)
            {
                Random random = new();
                _location.X = random.Next(spawnWindow.Width);
                _location.Y = random.Next(spawnWindow.Height);

                xDiff = _player.X - _location.X;
                yDiff = _player.Y - _location.Y;

                hypotenuse = Math.Sqrt(Math.Pow(_player.X - _location.X, 2) + Math.Pow(_player.Y - _location.Y, 2));
            }

            _direction.X = xDiff / hypotenuse;
            _direction.Y = yDiff / hypotenuse;
        }

        /// <inheritdoc/>
        public List<Polygon> GetObjectSprite()
        {
            List<Polygon> list = [];
            Polygon polygon = new();

            polygon.Points.Add(new Point((int)_location.X, (int)_location.Y));
            polygon.Points.Add(new Point((int)(_location.X + 50), (int)_location.Y));
            polygon.Points.Add(new Point((int)(_location.X + 50), (int)(_location.Y + 50)));
            polygon.Points.Add(new Point((int)_location.X, (int)(_location.Y + 50)));
            polygon.Brush = Brushes.DarkRed;

            list.Add(polygon);

            _collisionBox = list;

            return list;
        }

        /// <inheritdoc/>
        public bool Update(List<(Keys key, TimeSpan time)> pressedTimers, List<IGameObject> gameObjects)
        {
            foreach (var gameObject in gameObjects.Where(x => x is ICollidable))
            {
                ICollidable? collidable = gameObject as ICollidable;

                if (collidable is null || collidable is Enemy)
                    continue;
                
                if (collidable.CollidesWith(_collisionBox))
                    return false;
            }

            double xDiff = _player.X - _location.X;
            double yDiff = _player.Y - _location.Y;

            double hypotenuse = Math.Sqrt(Math.Pow(_player.X - _location.X, 2) + Math.Pow(_player.Y - _location.Y, 2));

            _direction.X = xDiff / hypotenuse;
            _direction.Y = yDiff / hypotenuse;

            _moveDistance = pressedTimers.First(x => x.key == Keys.F20).time.TotalMilliseconds / 20;

            _location.X += _moveDistance * _direction.X;
            _location.Y += _moveDistance * _direction.Y;

            return Math.Abs(hypotenuse) >= 1;
        }

        public bool CollidesWith(List<Polygon> polygons)
        {
            ArgumentNullException.ThrowIfNull(polygons);

            foreach (var polygonOne in _collisionBox)
            {
                foreach (var polygonTwo in polygons)
                {
                    if (polygonOne.PolygonsIntersect(polygonTwo))
                        return true;
                    
                    if (polygonOne.Points.Any(polygonTwo.IsPointInPolygon)
                     || polygonTwo.Points.Any(polygonOne.IsPointInPolygon))
                        return true;
                }
            }

            return false;
        }
    }
}
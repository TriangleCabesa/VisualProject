using System.Diagnostics;

namespace VisualProject
{
    public class Projectile : IGameObject, ICollidable
    {
        private (double X, double Y) _location = (0, 0);
        private (double X, double Y) _direction = (0, 0);
        private List<Polygon> _collisionBox = [];
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private readonly TimeSpan _lifeSpan;

        public Projectile(int x, int y, (int X, int Y) destination, TimeSpan lifeSpan)
        {
            _location.X = x;
            _location.Y = y;

            int xDiff = destination.X - x;
            int yDiff = destination.Y - y;

            double hypotenuse = Math.Sqrt(Math.Pow(destination.X - x, 2) + Math.Pow(destination.Y - y, 2));

            _direction.X = xDiff / hypotenuse;
            _direction.Y = yDiff / hypotenuse;
            _lifeSpan = lifeSpan;
        }

        /// <inheritdoc/>
        public List<Polygon> GetObjectSprite()
        {
            List<Polygon> list = [];
            Polygon polygon = new();

            int bulletSize = 10;
            polygon.Points.Add(new Point((int)_location.X, (int)_location.Y));
            polygon.Points.Add(new Point((int)_location.X + bulletSize, (int)_location.Y));
            polygon.Points.Add(new Point((int)_location.X + bulletSize, (int)_location.Y + bulletSize));
            polygon.Points.Add(new Point((int)_location.X, (int)_location.Y + bulletSize));

            list.Add(polygon);
            _collisionBox = list;

            return list;
        }

        /// <inheritdoc/>
        public bool Update(List<(Keys key, TimeSpan time)> pressedTimers, List<IGameObject> gameObjects)
        {
            double moveDistance = pressedTimers.First(x => x.key == Keys.F20).time.TotalMilliseconds;
            _location.X += _direction.X * moveDistance;
            _location.Y += _direction.Y * moveDistance;

            return _stopwatch.Elapsed < _lifeSpan;
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

using System.Diagnostics;

namespace VisualProject
{
    public class Projectile : IGameObject, ICollidable
    {
        private double X;
        private double Y;
        private (double X, double Y) Direction = (0, 0);
        private List<Polygon> _collisionBox = [];
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private readonly TimeSpan _lifeSpan;

        public Projectile(int x, int y, (int X, int Y) destination, TimeSpan lifeSpan)
        {
            X = x;
            Y = y;

            int xDiff = destination.X - x;
            int yDiff = destination.Y - y;

            double hypotenuse = Math.Sqrt(Math.Pow(destination.X - x, 2) + Math.Pow(destination.Y - y, 2));

            Direction.X = xDiff / hypotenuse;
            Direction.Y = yDiff / hypotenuse;
            _lifeSpan = lifeSpan;
        }

        /// <inheritdoc/>
        public List<Polygon> GetObjectSprite()
        {
            List<Polygon> list = [];
            Polygon polygon = new();

            int bulletSize = 10;
            polygon.Points.Add(new Point((int)X,(int)Y));
            polygon.Points.Add(new Point((int)X + bulletSize, (int)Y));
            polygon.Points.Add(new Point((int)X + bulletSize, (int)Y + bulletSize));
            polygon.Points.Add(new Point((int)X, (int)Y + bulletSize));

            list.Add(polygon);
            _collisionBox = list;

            return list;
        }

        /// <inheritdoc/>
        public bool Update(List<(Keys key, TimeSpan time)> pressedTimers, List<IGameObject> gameObjects)
        {
            double moveDistance = pressedTimers.First(x => x.key == Keys.F20).time.TotalMilliseconds;
            X += Direction.X * moveDistance;
            Y += Direction.Y * moveDistance;

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

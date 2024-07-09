using System.Diagnostics;

namespace VisualProject
{
    public class Projectile : IGameObject, ICollidable
    {
        // We can't use point because doubles are needed for higher accuracy.
        private (double X, double Y) _location;
        private (double X, double Y) _direction;
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private List<Polygon> _collisionBox = [];
        private readonly TimeSpan _lifeSpan;

        /// <summary>
        /// Makes a new projectile that will move towards the specified location.
        /// </summary>
        /// <param name="origin">The origin point.</param>
        /// <param name="destination">The target destination point.</param>
        /// <param name="lifeSpan">The amount of time until the <see cref="Update(List{ValueTuple{Keys, TimeSpan}}, List{IGameObject})"/> method returns false.</param>
        public Projectile(Point origin, Point destination, TimeSpan lifeSpan)
        {
            _location.X = origin.X;
            _location.Y = origin.Y;

            int xDiff = destination.X - origin.X;
            int yDiff = destination.Y - origin.Y;

            double hypotenuse = Math.Sqrt(Math.Pow(destination.X - origin.X, 2) + Math.Pow(destination.Y - origin.Y, 2));

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
            polygon.Points.Add(new Point((int)_location.X - bulletSize / 2, (int)_location.Y - bulletSize / 2));
            polygon.Points.Add(new Point((int)_location.X + bulletSize / 2, (int)_location.Y - bulletSize / 2));
            polygon.Points.Add(new Point((int)_location.X + bulletSize / 2, (int)_location.Y + bulletSize / 2));
            polygon.Points.Add(new Point((int)_location.X - bulletSize / 2, (int)_location.Y + bulletSize / 2));

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

        public bool CollidesWith(List<Polygon> polygons) =>
            CollisionDetector.CollidesWith(_collisionBox, polygons);
    }
}

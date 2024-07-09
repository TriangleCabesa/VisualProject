﻿using System.Diagnostics;

namespace VisualProject
{
    public class Projectile : IGameObject, ICollidable
    {
        // We can't use point because doubles are needed for higher accuracy.
        private (double X, double Y) _location = (0, 0);
        private (double X, double Y) _direction = (0, 0);
        private List<Polygon> _collisionBox = [];
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
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

        /// <inheritdoc/>
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualProject
{
    public class Projectile : IGameObject, ICollidable
    {
        private double X;
        private double Y;
        private (double X, double Y) Direction = (0, 0);
        private Stopwatch stopwatch = Stopwatch.StartNew();
        private TimeSpan LifeSpan;
        private List<Polygon> CollisionBox { get; set; } = [];

        public Projectile(int x, int y, (int X, int Y) destination, TimeSpan lifeSpan)
        {
            X = x;
            Y = y;

            int xDiff = destination.X - x;
            int yDiff = destination.Y - y;

            double hypotenuse = Math.Sqrt(Math.Pow(destination.X - x, 2) + Math.Pow(destination.Y - y, 2));

            Direction.X = xDiff / hypotenuse;
            Direction.Y = yDiff / hypotenuse;
            LifeSpan = lifeSpan;
        }

        /// <inheritdoc/>
        public List<Polygon> GetObjectSprite()
        {
            List<Polygon> list = new List<Polygon>();      

            Polygon polygon = new Polygon();

            int bulletSize = 10;
            polygon.Points.Add(new Point((int)X,(int)Y));
            polygon.Points.Add(new Point((int)X + bulletSize, (int)Y));
            polygon.Points.Add(new Point((int)X + bulletSize, (int)Y + bulletSize));
            polygon.Points.Add(new Point((int)X, (int)Y + bulletSize));

            list.Add(polygon);

            CollisionBox = list;

            return list;
        }

        /// <inheritdoc/>
        public bool Update(List<(Keys key, TimeSpan time)> pressedTimers, List<IGameObject> gameObjects)
        {
            double moveDistance = pressedTimers.First(x => x.key == Keys.F20).time.TotalMilliseconds;
            X += Direction.X * moveDistance;
            Y += Direction.Y * moveDistance;

            return stopwatch.Elapsed < LifeSpan;
        }

        public bool CollidesWith(List<Polygon> polygons)
        {
            ArgumentNullException.ThrowIfNull(polygons);

            foreach (var polygonOne in CollisionBox)
            {
                foreach (var polygonTwo in polygons)
                {
                    if (polygonOne.PolygonsIntersect(polygonTwo))
                        return true;
                }
            }

            return false;
        }
    }
}

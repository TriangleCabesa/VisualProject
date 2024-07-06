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
        private Rectangle CollisionBox { get; set; } = new();

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

            polygon.Points.Add(new Point((int)X,(int)Y));
            polygon.Points.Add(new Point((int)X + 10, (int)Y));
            polygon.Points.Add(new Point((int)X + 10, (int)Y + 10));
            polygon.Points.Add(new Point((int)X, (int)Y + 10));

            list.Add(polygon);

            CollisionBox = polygon.ToRectangle();

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

        public bool CollidesWith(Rectangle rectangle)
        {
            if (rectangle == CollisionBox)
                return false;

            return CollisionBox.IntersectsWith(rectangle);
        }
    }
}

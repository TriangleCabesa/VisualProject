
using System.Drawing;
using System.Reflection;

namespace VisualProject
{
    public class Player : IGameObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Size { get; set; } = 100;

        public List<Polygon> GetObjectSprite()
        {
            if (Size < 0)
                Size = 0;

            double sizePercent = .2;

            List<Polygon> result = [];
            
            foreach (var (brush, rectangleLoop) in new Rectangle(X, Y, Size, Size).AsHollowedSquare(Brushes.Red, sizePercent))
            {
                Polygon polygonLoop = new();
                polygonLoop.Points.Add(new Point(rectangleLoop.X, rectangleLoop.Y));
                polygonLoop.Points.Add(new Point(rectangleLoop.X + rectangleLoop.Width, rectangleLoop.Y));
                polygonLoop.Points.Add(new Point(rectangleLoop.X + rectangleLoop.Width, rectangleLoop.Y + rectangleLoop.Height));
                polygonLoop.Points.Add(new Point(rectangleLoop.X, rectangleLoop.Y + rectangleLoop.Height));
                polygonLoop.Brush = brush;
                result.Add(polygonLoop);
            }

            Rectangle rectangle = new(X + Size / 3, Y + Size, Size / 3 + 1, Size * 3 + 1);
            Polygon polygon = new();
            polygon.Points.Add(new Point(rectangle.X, rectangle.Y));
            polygon.Points.Add(new Point(rectangle.X + rectangle.Width, rectangle.Y));
            polygon.Points.Add(new Point(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height));
            polygon.Points.Add(new Point(rectangle.X, rectangle.Y + rectangle.Height));
            polygon.Brush = Brushes.Black;
            result.Add(polygon);
            return result;
        }

        public bool Update(List<(Keys key, TimeSpan time)> pressedTimers)
        {
            foreach (var (key, timer) in pressedTimers)
            {
                if (key == Keys.W)
                    Y -= (int)timer.TotalMilliseconds / 2;
                if (key == Keys.A)
                    X -= (int)timer.TotalMilliseconds / 2;
                if (key == Keys.S)
                    Y += (int)timer.TotalMilliseconds / 2;
                if (key == Keys.D)
                    X += (int)timer.TotalMilliseconds / 2;
                if (key == Keys.Up)
                {
                    Size += (int)timer.TotalMilliseconds;
                    X -= (int)timer.TotalMilliseconds / 2;
                    Y -= (int)timer.TotalMilliseconds / 2;
                }
                if (key == Keys.Down)
                {
                    Size -= (int)timer.TotalMilliseconds;

                    if (Size < 0)
                    {
                        Size = 0;
                        continue;
                    }

                    X += (int)timer.TotalMilliseconds / 2;
                    Y += (int)timer.TotalMilliseconds / 2;
                }
            }

            return true;
        }
    }
}

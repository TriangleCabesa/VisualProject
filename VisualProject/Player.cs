
using System.Drawing;
using System.Reflection;

namespace VisualProject
{
    public class Player : IGameObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Size { get; set; } = 100;
        public int Rotation { get; set; } = 0;
        public int ShotsPerSecond = 3;

        private bool canFire = true;
        public bool CanFire 
        {
            get { return canFire; }
            set
            {
                canFire = value;

                if (!value)
                    Task.Run(() => 
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(1000 / ShotsPerSecond));
                        canFire = true;
                    });
            }
        }

        public List<Polygon> GetObjectSprite()
        {
            if (Size < 0)
                Size = 0;

            List<Polygon> result = [];
            
            double rotation = Rotation * (Math.PI / 180);
            Point startPoint = new(X,Y);
            Polygon polygon = new();
            polygon.Points.Add(new Point(X, Y - Size / 2));
            polygon.Points.Add(new Point(X - Size, Y + Size / 3));
            polygon.Points.Add(new Point(polygon.Points.Last().X, polygon.Points.Last().Y + Size / 5));
            polygon.Points.Add(new Point(polygon.Points.Last().X + Size / 5, polygon.Points.Last().Y));
            polygon.Points.Add(new Point(polygon.Points.Last().X, polygon.Points.Last().Y - Size / 5));
            polygon.Points.Add(new Point(X + Size - Size / 5, Y + Size / 3));
            polygon.Points.Add(new Point(polygon.Points.Last().X, polygon.Points.Last().Y + Size / 5));
            polygon.Points.Add(new Point(polygon.Points.Last().X + Size / 5, polygon.Points.Last().Y));
            polygon.Points.Add(new Point (X + Size, Y + Size / 3));

            polygon.Brush = Brushes.Gray;
            
            result.Add(polygon);

            polygon = new();
            polygon.Points.Add(new Point(X, Y - Size / 3));
            polygon.Points.Add(new Point(X + Size / 3, Y));
            polygon.Points.Add(new Point(X - Size / 3, Y));
            polygon.Brush = Brushes.Cyan;
            result.Add(polygon);

            for (int i = 0; i < result.Count; i++)
                for (int j = 0; j < result[i].Points.Count; j++)
                    result[i].Points[j] = result[i].Points[j].Rotate(startPoint, rotation);
                
            return result;
        }

        public bool Update(List<(Keys key, TimeSpan time)> pressedTimers, List<IGameObject> gameObjects, PaintEventArgs paintEventArgs)
        {
            int lastX = X;
            int lastY = Y;

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

            if (lastX != X || lastY != Y)
            {
                var deltaX = lastX - X;
                var deltaY = lastY - Y; // Y is inverted because coordinates are backwards in programming

                Rotation = (int)(Math.Atan2(deltaX, deltaY) * (-180 / Math.PI));
            }

            return true;
        }
    }
}

namespace VisualProject
{
    public class Enemy : IGameObject
    {
        private double X { get; set; }
        private double Y { get; set; }
        private (double X, double Y) Direction = (0, 0);
        private Player Player { get; set; }
        double MoveDistance { get; set; } = 0;

        public Enemy(Player player, Rectangle spawnWindow)
        {
            Player = player;

            double hypotenuse = 0;
            double xDiff = 0;
            double yDiff = 0;

            double maxDistance = Math.Sqrt(Math.Pow(Player.X - 0, 2) + Math.Pow(Player.Y - 0, 2));
            maxDistance = Math.Max(maxDistance, Math.Sqrt(Math.Pow(Player.X - 0, 2) + Math.Pow(Player.Y - spawnWindow.Height, 2)));
            maxDistance = Math.Max(maxDistance, Math.Sqrt(Math.Pow(Player.X - spawnWindow.Width, 2) + Math.Pow(Player.Y - spawnWindow.Height, 2)));
            maxDistance = Math.Max(maxDistance, Math.Sqrt(Math.Pow(Player.X - spawnWindow.Width, 2) + Math.Pow(Player.Y - 0, 2)));
            
            double minimumSpawnDistance = maxDistance < 1000 ? maxDistance * 0.5 : 500;

            while (hypotenuse < minimumSpawnDistance)
            {
                Random random = new();
                X = random.Next(spawnWindow.Width);
                Y = random.Next(spawnWindow.Height);

                xDiff = Player.X - X;
                yDiff = Player.Y - Y;

                hypotenuse = Math.Sqrt(Math.Pow(Player.X - X, 2) + Math.Pow(Player.Y - Y, 2));
            }

            Direction.X = xDiff / hypotenuse;
            Direction.Y = yDiff / hypotenuse;
        }

        public List<Polygon> GetObjectSprite()
        {
            List<Polygon> list = [];
            Polygon polygon = new();

            polygon.Points.Add(new Point((int)X, (int)Y));
            polygon.Points.Add(new Point((int)(X + 10), (int)Y));
            polygon.Points.Add(new Point((int)(X + 10), (int)(Y + 10)));
            polygon.Points.Add(new Point((int)X, (int)(Y + 10)));
            polygon.Brush = Brushes.DarkRed;

            list.Add(polygon);

            return list;
        }

        public bool Update(List<(Keys key, TimeSpan time)> pressedTimers, List<IGameObject> gameObjects, PaintEventArgs paintEventArgs)
        {
            double xDiff = Player.X - X;
            double yDiff = Player.Y - Y;

            double hypotenuse = Math.Sqrt(Math.Pow(Player.X - X, 2) + Math.Pow(Player.Y - Y, 2));

            Direction.X = xDiff / hypotenuse;
            Direction.Y = yDiff / hypotenuse;

            MoveDistance = pressedTimers.First(x => x.key == Keys.F20).time.TotalMilliseconds / 20;

            X += MoveDistance * Direction.X;
            Y += MoveDistance * Direction.Y;

            return Math.Abs(hypotenuse) >= 50;
        }
    }
}
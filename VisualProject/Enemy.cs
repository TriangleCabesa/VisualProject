namespace VisualProject
{
    public class Enemy : IGameObject, ICollidable
    {
        private double X { get; set; }
        private double Y { get; set; }
        private (double X, double Y) Direction = (0, 0);
        private Player Player { get; set; }
        double MoveDistance { get; set; } = 0;
        private Rectangle CollisionBox { get; set; } = new Rectangle();

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

        /// <inheritdoc/>
        public List<Polygon> GetObjectSprite()
        {
            List<Polygon> list = [];
            Polygon polygon = new();

            polygon.Points.Add(new Point((int)X, (int)Y));
            polygon.Points.Add(new Point((int)(X + 50), (int)Y));
            polygon.Points.Add(new Point((int)(X + 50), (int)(Y + 50)));
            polygon.Points.Add(new Point((int)X, (int)(Y + 50)));
            polygon.Brush = Brushes.DarkRed;

            list.Add(polygon);

            CollisionBox = polygon.ToRectangle();

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
                
                if (collidable.CollidesWith(CollisionBox))
                    return false;
            }

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

        public bool CollidesWith(Rectangle rectangle)
        {
            if (rectangle == CollisionBox)
                return false;

            return CollisionBox.IntersectsWith(rectangle);
        }
    }
}
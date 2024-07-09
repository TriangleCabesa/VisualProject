namespace VisualProject
{
    public class Player : IGameObject, ICollidable
    {
        public Point Location { get; private set; }
        public int Size { get; set; } = 100;
        public int Rotation { get; set; } = 0;

        private bool _canFire = true;
        public bool CanFire 
        {
            get { return _canFire; }
            set
            {
                _canFire = value;

                if (!value)
                    Task.Run(() => 
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(1000 / _shotsPerSecond));
                        _canFire = true;
                    });
            }
        }

        private List<Polygon> _collisionBox = [];
        private readonly int _shotsPerSecond = 10;

        /// <inheritdoc/>
        public List<Polygon> GetObjectSprite()
        {
            if (Size < 0)
                Size = 0;

            List<Polygon> result = [];
            
            double rotation = Rotation * (Math.PI / 180);
            Point startPoint = new(Location.X, Location.Y);
            Polygon polygon = new();
            polygon.Points.Add(new Point(Location.X, Location.Y - Size / 2));
            polygon.Points.Add(new Point(Location.X - Size, Location.Y + Size / 3));
            polygon.Points.Add(new Point(polygon.Points.Last().X, polygon.Points.Last().Y + Size / 5));
            polygon.Points.Add(new Point(polygon.Points.Last().X + Size / 5, polygon.Points.Last().Y));
            polygon.Points.Add(new Point(polygon.Points.Last().X, polygon.Points.Last().Y - Size / 5));
            polygon.Points.Add(new Point(Location.X + Size - Size / 5, Location.Y + Size / 3));
            polygon.Points.Add(new Point(polygon.Points.Last().X, polygon.Points.Last().Y + Size / 5));
            polygon.Points.Add(new Point(polygon.Points.Last().X + Size / 5, polygon.Points.Last().Y));
            polygon.Points.Add(new Point (Location.X + Size, Location.Y + Size / 3));

            polygon.Brush = Brushes.Gray;
            
            result.Add(polygon);

            polygon = new();
            polygon.Points.Add(new Point(Location.X, Location.Y - Size / 3));
            polygon.Points.Add(new Point(Location.X + Size / 3, Location.Y));
            polygon.Points.Add(new Point(Location.X - Size / 3, Location.Y));
            polygon.Brush = Brushes.Cyan;
            result.Add(polygon);

            for (int i = 0; i < result.Count; i++)
                for (int j = 0; j < result[i].Points.Count; j++)
                    result[i].Points[j] = result[i].Points[j].Rotate(startPoint, rotation);

            _collisionBox = result;
                
            return result;
        }

        /// <inheritdoc/>
        public bool Update(List<(Keys key, TimeSpan time)> pressedTimers, List<IGameObject> gameObjects)
        {
            int lastX = Location.X;
            int lastY = Location.Y;

            foreach (var (key, timer) in pressedTimers)
            {
                if (key == Keys.W)
                    Location = new Point(Location.X, Location.Y - (int)timer.TotalMilliseconds / 2);
                if (key == Keys.A)
                    Location = new Point(Location.X - (int)timer.TotalMilliseconds / 2, Location.Y);
                if (key == Keys.S)
                    Location = new Point(Location.X, Location.Y + (int)timer.TotalMilliseconds / 2);
                if (key == Keys.D)
                    Location = new Point(Location.X + (int)timer.TotalMilliseconds / 2, Location.Y);
                if (key == Keys.Up)
                {
                    Size += (int)timer.TotalMilliseconds;
                    Location = new Point(Location.X - (int)timer.TotalMilliseconds / 2, Location.Y - (int)timer.TotalMilliseconds / 2);
                }
                if (key == Keys.Down)
                {
                    Size -= (int)timer.TotalMilliseconds;

                    if (Size < 0)
                        Size = 0;
                    else
                        Location = new Point(Location.X + (int)timer.TotalMilliseconds / 2, Location.Y + (int)timer.TotalMilliseconds / 2);
                }
            }

            if (lastX != Location.X || lastY != Location.Y)
            {
                var deltaX = lastX - Location.X;
                var deltaY = lastY - Location.Y;

                Rotation = (int)(Math.Atan2(deltaX, deltaY) * (-180 / Math.PI));
            }

            return true;
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

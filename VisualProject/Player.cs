namespace VisualProject
{
    public class Player : IGameObject, ICollidable
    {
        public Point Location { get; private set; }

        private Point _projectileOrigin;
        public Point ProjectileOrigin
        {
            get
            {
                _barrelNumber++;

                if (_barrelNumber == _barrelLocations.Count)
                    _barrelNumber = 0;

                return _projectileOrigin;
            }
            private set => _projectileOrigin = value;
        }
        public int Size { get; set; } = 10;
        public double Rotation { get; set; } = 0;

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


        private int _barrelNumber = 0;
        private Bitmap _playerSprite = new(Image.FromFile(Directory.GetCurrentDirectory().Split("VisualProject").First() + @"VisualProject\VisualProject\Sprites\ShipSprite.png"));
        private readonly List<Point> _barrelLocations =
        [
            new(1, 9),
            new(6, 6),
            new(17, 6),
            new(22, 9),
        ];

        /// <inheritdoc/>
        public List<Polygon> GetObjectSprite()
        {
            Size = Size < 0 ? 0 : Size;

            List<Polygon> result = ImageConverter.GetPolygonsFromImage(Location, _playerSprite, Size, 215);

            for (int i = 0; i < result.Count; i++)
                for (int j = 0; j < result[i].Points.Count; j++)
                    result[i].Points[j] = result[i].Points[j].Rotate(Location, Rotation);

            _collisionBox = result;
            ProjectileOrigin = GetBarrelLocation(); // We update this here so it's set to the right place on the next frame

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

                Rotation = Math.Atan2(-deltaX, deltaY);
            }

            return true;
        }

        public bool CollidesWith(List<Polygon> polygons) =>
            CollisionDetector.CollidesWith(_collisionBox, polygons);

        private Point GetBarrelLocation()
        {
            Point targetPoint = _barrelLocations[_barrelNumber];

            if (targetPoint.X > _playerSprite.Width || targetPoint.Y > _playerSprite.Height)
                throw new ArgumentOutOfRangeException(nameof(targetPoint), "point not in range of sprite image");

            int spriteSizeX = _playerSprite.Width * Size;
            int spriteSizeY = _playerSprite.Height * Size;

            int startX = Location.X - spriteSizeX / 2;
            int startY = Location.Y - spriteSizeY / 2;

            int locationX = startX + targetPoint.X * Size + Size / 2;
            int locationY = startY + targetPoint.Y * Size + Size / 2;

            return new Point(locationX, locationY).Rotate(Location, Rotation);
        }
    }
}

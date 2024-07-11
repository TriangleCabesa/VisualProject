namespace VisualProject
{
    public class Player : IGameObject, ICollidable
    {
        /// <summary>
        /// The location of the player object.
        /// </summary>
        public Point Location { get; private set; }

        /// <summary>
        /// The maximum number of projectiles the player can shoot per second.
        /// </summary>
        public int ShotsPerSecond { get; set; } = 4;

        /// <summary>
        /// Size of the sprite's pixels.
        /// </summary>
        public int Size { get; set; } = 5;

        /// <summary>
        /// The radians the player is rotated by.
        /// </summary>
        public double Rotation { get; set; } = 0;

        private Point _projectileOrigin;
        /// <summary>
        /// The location for the next projectile to spawn.
        /// </summary>
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

        

        private bool _canFire = true;
        /// <summary>
        /// Whether or not the player can fire again. Automatically resets itself after enough time has passed (based on shots per second).
        /// </summary>
        public bool CanFire 
        {
            get { return _canFire; }
            set
            {
                _canFire = value;

                if (!value)
                    Task.Run(() => 
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(1000 / ShotsPerSecond));
                        _canFire = true;
                    });
            }
        }

        private int _barrelNumber = 0;
        private List<Polygon> _collisionBox = [];
        private readonly Bitmap _playerSprite = new(Image.FromFile(Directory.GetCurrentDirectory().Split("VisualProject").First() + @"VisualProject\VisualProject\Sprites\ShipSprite.png"));
        private readonly List<Point> _barrelLocations =
        [
            new(1, 9),
            new(22, 9),
            new(6, 6),
            new(17, 6),
        ];
        private readonly List<Point> _hitboxPoints =
        [
            new(1, 10),
            new(6, 7),
            new(11, 1),
            new(12, 1),
            new(17, 7),
            new(22, 10),
            new(21, 23),
            new(14, 21),
            new(9, 21),
            new(2, 23),
        ];

        /// <inheritdoc/>
        public List<Polygon> GetObjectSprite()
        {
            Size = Size < 0 ? 0 : Size;

            List<Polygon> result = ImageConverter.GetPolygonsFromImage(Location, _playerSprite, Size);

            for (int i = 0; i < result.Count; i++)
                for (int j = 0; j < result[i].Points.Count; j++)
                    result[i].Points[j] = result[i].Points[j].Rotate(Location, Rotation);

            ProjectileOrigin = GetActualPointFromTargetPoint(_barrelLocations[_barrelNumber]); // We update this here so it's set to the right place on the next frame

            Polygon polygon = new();

            foreach (var point in _hitboxPoints)
                polygon.Points.Add(GetActualPointFromTargetPoint(point));

            _collisionBox = [polygon];

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

        /// <inheritdoc/>
        public bool CollidesWith(List<Polygon> polygons) =>
            CollisionDetector.CollidesWith(_collisionBox, polygons);

        private Point GetActualPointFromTargetPoint(Point targetPoint)
        {
            return targetPoint.GetActualPointFromTargetPoint(Location, _playerSprite, Size, Rotation);
        }
    }
}

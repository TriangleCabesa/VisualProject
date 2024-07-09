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
        public int ShotsPerSecond { get; set; } = 10;

        /// <summary>
        /// Size of the sprite's pixels.
        /// </summary>
        public int Size { get; set; } = 10;

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

        /// <inheritdoc/>
        public List<Polygon> GetObjectSprite()
        {
            Size = Size < 0 ? 0 : Size;

            List<Polygon> result = ImageConverter.GetPolygonsFromImage(Location, _playerSprite, Size);

            for (int i = 0; i < result.Count; i++)
                for (int j = 0; j < result[i].Points.Count; j++)
                    result[i].Points[j] = result[i].Points[j].Rotate(Location, Rotation);

            _collisionBox = result;
            ProjectileOrigin = GetActualPointFromTargetPoint(_barrelLocations[_barrelNumber]); // We update this here so it's set to the right place on the next frame

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

        /// <summary>
        /// <see cref="Location"/> is in the center of the displayed sprite. This takes the <paramref name="targetPoint"/>, and converts it
        /// into a new <see cref="Point"/>. This is done based on the current <see cref="Location"/>, <see cref="Rotation"/>, and <see cref="Size"/>
        /// of the <see cref="Player"/>.
        /// </summary>
        /// <param name="targetPoint">The point location as found on the sprite.</param>
        /// <returns>The converted <see cref="Point"/> for the barrel location.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="targetPoint"/> is outside of the sprite image.</exception>
        private Point GetActualPointFromTargetPoint(Point targetPoint)
        {
            if (targetPoint.X > _playerSprite.Width || targetPoint.Y > _playerSprite.Height || targetPoint.X < 0 || targetPoint.Y < 0)
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

using VisualProject.Extensions;
using VisualProject.HelperClasses;
using VisualProject.Interfaces;

namespace VisualProject.Implementations
{
    public class Player : IGameObject, ICollidable, IDamagable
    {
        private int _damage;
        private int _barrelNumber;
        private ExplosionHandler? _explosionHandler;
        private readonly Bitmap _playerSprite;
        private readonly List<Point> _barrelLocations;
        private readonly List<Point> _hitboxPoints;

        private int _health;
        private int Health
        {
            get
            {
                return _health;
            }
            set
            {
                _health = value;

                if (_health < 1)
                    _explosionHandler = new();
            }
        }

        /// <summary>
        /// The location of the player object.
        /// </summary>
        public Point Location { get; private set; }

        /// <summary>
        /// The maximum number of projectiles the player can shoot per second.
        /// </summary>
        public int ShotsPerSecond { get; set; }

        /// <summary>
        /// Size of the sprite's pixels.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// The radians the player is rotated by.
        /// </summary>
        public double Rotation { get; set; }

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

        /// <inheritdoc/>
        public int BodyDamage { get => _damage; private set => _damage = value; }

        /// <inheritdoc/>
        public bool IsAlive
        {
            get
            {
                if (Health <= 0)
                    _explosionHandler = new();

                return Health > 0;
            }
        }

        public int Experience { get; set; }
        public int Level { get; set; }

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

        /// <inheritdoc/>
        public List<Polygon> CollisionBox { get; private set; }
        
        public Player(Rectangle? spawnWindow = null)
        {
            spawnWindow ??= new Rectangle(0, 0, 800, 450);
            Location = new(spawnWindow.Value.Width / 2, spawnWindow.Value.Height / 2);
            ShotsPerSecond = 10;
            Size = 6;
            _damage = 1;
            Health = 10;
            CollisionBox = [];
            _barrelLocations = [new(1, 9), new(22, 9), new(6, 6), new(17, 6)];
            _hitboxPoints = [ new(1, 10), new(6, 7), new(11, 1), new(12, 1), new(17, 7), new(22, 10), new(21, 23), new(14, 21), new(9, 21), new(2, 23) ];
            _playerSprite = new(Image.FromFile(Directory.GetCurrentDirectory().Split("VisualProject").First() + @"VisualProject\VisualProject\Sprites\ShipSprite.png"));
        }

        /// <inheritdoc/>
        public List<Polygon> GetObjectSprite()
        {
            Size = Size < 0 ? 0 : Size;

            if (_explosionHandler is not null)
            {
                CollisionBox = [];

                return _explosionHandler.GetExplosionSprite(Location, Rotation, Size);
            }

            List<Polygon> result = ImageReader.GetPolygonsFromImage(Location, _playerSprite, Size);

            for (int i = 0; i < result.Count; i++)
                for (int j = 0; j < result[i].Points.Count; j++)
                    result[i].Points[j] = result[i].Points[j].Rotate(Location, Rotation);

            ProjectileOrigin = GetActualPointFromTargetPoint(_barrelLocations[_barrelNumber]); // We update this here so it's set to the right place on the next frame

            Polygon polygon = new();

            foreach (var point in _hitboxPoints)
                polygon.Points.Add(GetActualPointFromTargetPoint(point));

            CollisionBox = [polygon];

            return result;
        }

        /// <inheritdoc/>
        public bool Update(List<(Keys key, TimeSpan time)> pressedTimers, List<IGameObject> gameObjects)
        {
            if (_explosionHandler is not null)
            {
                _explosionHandler.UpdateExplosionSprite(pressedTimers);

                return _explosionHandler.ExplosionNotComplete;
            }

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
        public bool CollidesWith(ICollidable collidable) =>
            CollisionDetector.CollidesWith(CollisionBox, collidable.CollisionBox);

        /// <inheritdoc/>
        public void TakeDamage(IProjectile projectile) => Health -= projectile.Damage;

        /// <inheritdoc/>
        public void TakeDamage(IDamagable damagable) => Health -= damagable.BodyDamage;

        /// <inheritdoc/>
        public void DealDamage(IDamagable damagable) => damagable.TakeDamage(this);

        /// <inheritdoc/>
        public void HandleCollision(ICollidable collidable)
        {
            if (collidable is Enemy enemy)
            {
                DealDamage(enemy);
            }
        }
        
        private Point GetActualPointFromTargetPoint(Point targetPoint)
        {
            return targetPoint.GetActualPointFromTargetPoint(Location, _playerSprite, Size, Rotation);
        }
    }
}

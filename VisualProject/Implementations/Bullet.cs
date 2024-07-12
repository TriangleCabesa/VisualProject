using System.Diagnostics;
using VisualProject.HelperClasses;
using VisualProject.Interfaces;

namespace VisualProject.Implementations
{
    public class Bullet : IGameObject, ICollidable, IProjectile
    {
        // We can't use point because doubles are needed for higher accuracy.
        private (double X, double Y) _location;
        private (double X, double Y) _direction;
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private readonly TimeSpan _lifeSpan;
        private int _health;
        private int _damage;

        /// <inheritdoc/>
        public List<Polygon> CollisionBox { get; private set; }

        /// <inheritdoc/>
        public bool IsNotBroken { get => _health > 0;  }

        /// <inheritdoc/>
        public int Damage { get => _damage; }

        /// <inheritdoc/>
        public Type OriginType { get; private set; }

        /// <summary>
        /// Makes a new projectile that will move towards the specified location.
        /// </summary>
        /// <param name="origin">The origin point.</param>
        /// <param name="destination">The target destination point.</param>
        /// <param name="lifeSpan">The amount of time until the <see cref="Update(List{ValueTuple{Keys, TimeSpan}}, List{IGameObject})"/> method returns false.</param>
        public Bullet(Point origin, Point destination, TimeSpan lifeSpan, Type shooterType, int health = 1, int damage = 1)
        {
            _health = health;
            _damage = damage;
            _location.X = origin.X;
            _location.Y = origin.Y;

            int xDiff = destination.X - origin.X;
            int yDiff = destination.Y - origin.Y;

            double hypotenuse = Math.Sqrt(Math.Pow(destination.X - origin.X, 2) + Math.Pow(destination.Y - origin.Y, 2));

            _direction.X = xDiff / hypotenuse;
            _direction.Y = yDiff / hypotenuse;
            _lifeSpan = lifeSpan;

            OriginType = shooterType;
            CollisionBox = GetObjectSprite();
        }

        /// <inheritdoc/>
        public List<Polygon> GetObjectSprite()
        {
            List<Polygon> list = [];
            Polygon polygon = new();

            int bulletSize = 10;
            polygon.Points.Add(new Point((int)_location.X - bulletSize / 2, (int)_location.Y - bulletSize / 2));
            polygon.Points.Add(new Point((int)_location.X + bulletSize / 2, (int)_location.Y - bulletSize / 2));
            polygon.Points.Add(new Point((int)_location.X + bulletSize / 2, (int)_location.Y + bulletSize / 2));
            polygon.Points.Add(new Point((int)_location.X - bulletSize / 2, (int)_location.Y + bulletSize / 2));

            list.Add(polygon);
            CollisionBox = list;

            return list;
        }

        /// <inheritdoc/>
        public bool Update(List<(Keys key, TimeSpan time)> pressedTimers, List<IGameObject> gameObjects)
        {
            double moveDistance = pressedTimers.First(x => x.key == Keys.F20).time.TotalMilliseconds;
            _location.X += _direction.X * moveDistance;
            _location.Y += _direction.Y * moveDistance;

            return _stopwatch.Elapsed < _lifeSpan && IsNotBroken;
        }

        /// <inheritdoc/>
        public bool CollidesWith(ICollidable collidable) =>
            CollisionDetector.CollidesWith(CollisionBox, collidable.CollisionBox);

        /// <inheritdoc/>
        public void DealDamage(IDamagable damagable)
        {
            while (damagable.IsAlive && IsNotBroken)
            {
                damagable.TakeDamage(this);
                _health -= damagable.BodyDamage;
            }
        }

        /// <inheritdoc/>
        public void HandleCollision(ICollidable collidable)
        {
            if (collidable.GetType() == OriginType)
                return;

            if (collidable is IDamagable damagable)
            {
                DealDamage(damagable);
            }
        }
    }
}

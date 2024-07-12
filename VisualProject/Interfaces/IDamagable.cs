using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualProject.Interfaces
{
    public interface IDamagable
    {
        /// <summary>
        /// The amount of damage dealt when something collides with the <see cref="IDamagable"/>.
        /// </summary>
        int BodyDamage { get; }

        /// <summary>
        /// Whether or not the <see cref="IDamagable"/> is still alive.
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// Updates the <see cref="IDamagable"/>'s variables based on the damage dealt by <see cref="IProjectile"/>.
        /// </summary>
        /// <param name="projectile">An <see cref="IProjectile"/> that has collided with the <see cref="IDamagable"/>.</param>
        void TakeDamage(IProjectile projectile);

        /// <summary>
        /// Updates the <see cref="IDamagable"/>'s variables based on the damage dealt by <see cref="IDamagable"/>.
        /// </summary>
        /// <param name="projectile">An <see cref="IProjectile"/> that has collided with the <see cref="IDamagable"/>.</param>
        void TakeDamage(IDamagable damagable);

        /// <summary>
        /// Deals damage to an <see cref="IDamagable"/> that collided with this object.
        /// </summary>
        /// <param name="damagable">The <see cref="IDamagable"/> to deal damage to.</param>
        void DealDamage(IDamagable damagable);

        
    }
}

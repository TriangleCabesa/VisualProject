using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualProject.Interfaces
{
    public interface IProjectile
    {
        /// <summary>
        /// The object <see cref="Type"/> the <see cref="IProjectile"/> originated from.
        /// </summary>
        Type OriginType { get; }

        /// <summary>
        /// Damage the projectile deals.
        /// </summary>
        int Damage { get; }

        /// <summary>
        /// Whether or not the project is still capable of dealing damage.
        /// </summary>
        bool IsNotBroken { get; }

        /// <summary>
        /// Deals damage to the <see cref="IDamagable"/> object.
        /// </summary>
        /// <param name="damagable"></param>
        void DealDamage(IDamagable damagable);
    }
}

using VisualProject.HelperClasses;

namespace VisualProject.Interfaces
{
    public interface ICollidable
    {
        /// <summary>
        /// A <see cref="List{T}"/> of <see cref="Polygon"/>s that make up the <see cref="ICollidable"/>'s collision box.
        /// </summary>
        List<Polygon> CollisionBox { get; }

        /// <summary>
        /// Checks whether or not the <see cref="ICollidable"/> collides with the passed <see cref="ICollidable"/>.
        /// </summary>
        /// <example>
        /// For anything that doesn't move super fast, you can just use <see cref="CollisionDetector"/>.
        /// </example>
        /// <param name="polygons">List of polygons that represent the sprite to check for collisions against.</param>
        /// <returns>True if this object collides with one of the polygons.</returns>
        bool CollidesWith(ICollidable collidable);

        /// <summary>
        /// Handles any updates that need to happen as the result of a collision detection.
        /// </summary>
        /// <param name="collidable"></param>
        void HandleCollision(ICollidable collidable);
    }
}
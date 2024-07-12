namespace VisualProject.Interfaces
{
    public interface ICollidable
    {
        List<Polygon> CollisionBox { get; }

        /// <summary>
        /// Checks whether or not the object collides with any of the polygons in the given list.
        /// </summary>
        /// <param name="polygons">List of polygons that represent the sprite to check for collisions against.</param>
        /// <returns>True if this object collides with one of the polygons.</returns>
        bool CollidesWith(ICollidable collidable);

        void HandleCollision(ICollidable collidable);
    }
}
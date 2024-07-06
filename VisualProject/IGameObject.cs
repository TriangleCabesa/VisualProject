namespace VisualProject
{
    public interface IGameObject
    {
        /// <summary>
        /// Gets the polygons required to draw the sprite using Graphics2D
        /// </summary>
        /// <returns>A list of <see cref="Polygon"/>s.</returns>
        List<Polygon> GetObjectSprite();

        /// <summary>
        /// Updates the state of the <see cref="IGameObject"/> using the given information.
        /// </summary>
        /// <param name="pressedTimers">A list of which keys were pressed, and how long they were pressed for.</param>
        /// <param name="gameObjects">A list of <see cref="IGameObject"/>s'</param>
        /// <returns>Returns true if the <see cref="IGameObject"/> is still needed.</returns>
        bool Update(List<(Keys key, TimeSpan time)> pressedTimers, List<IGameObject> gameObjects);
    }
}

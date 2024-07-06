namespace VisualProject
{
    public interface IGameObject
    {
        List<Polygon> GetObjectSprite();
        bool Update(List<(Keys key, TimeSpan time)> pressedTimers, List<IGameObject> gameObjects, PaintEventArgs paintEventArgs);
    }
}

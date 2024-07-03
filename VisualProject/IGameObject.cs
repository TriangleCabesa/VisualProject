namespace VisualProject
{
    public interface IGameObject
    {
        List<Polygon> GetObjectSprite();
        bool Update(List<(Keys key, TimeSpan time)> pressedTimers);
    }
}

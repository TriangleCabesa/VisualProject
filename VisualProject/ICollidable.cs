namespace VisualProject
{
    public interface ICollidable
    {
        bool CollidesWith(List<Polygon> polygons);
    }
}
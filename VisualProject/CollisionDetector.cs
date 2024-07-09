namespace VisualProject
{
    public class CollisionDetector
    {
        public static bool CollidesWith(List<Polygon> collisionBox, List<Polygon> polygons)
        {
            ArgumentNullException.ThrowIfNull(polygons);

            foreach (var polygonOne in collisionBox)
            {
                foreach (var polygonTwo in polygons)
                {
                    if (polygonOne.PolygonsIntersect(polygonTwo))
                        return true;

                    if (polygonOne.Points.Any(polygonTwo.IsPointInPolygon)
                     || polygonTwo.Points.Any(polygonOne.IsPointInPolygon))
                        return true;
                }
            }

            return false;
        }
    }
}

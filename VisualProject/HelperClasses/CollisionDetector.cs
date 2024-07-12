using VisualProject.Extensions;

namespace VisualProject.HelperClasses
{
    public class CollisionDetector
    {
        /// <summary>
        /// Checks for collisions between two different collision boxes.
        /// </summary>
        /// <param name="collisionBoxOne">First collision box.</param>
        /// <param name="collisionBoxTwo">Second collision box.</param>
        /// <returns>True if they collide.</returns>
        public static bool CollidesWith(List<Polygon> collisionBoxOne, List<Polygon> collisionBoxTwo)
        {
            ArgumentNullException.ThrowIfNull(collisionBoxTwo);

            foreach (var polygonOne in collisionBoxOne)
            {
                foreach (var polygonTwo in collisionBoxTwo)
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

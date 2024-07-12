namespace VisualProject.HelperClasses
{
    public class ImageReader
    {
        /// <summary>
        /// Predicate that <see cref="GetPolygonsFromImage(Point, Bitmap, int, Predicate{Color}?)"/> uses to remove pixels. Pixels are removed when the predicate is false.
        /// Defaults to always return true.
        /// </summary>
        public static Predicate<Color> PixelNotUsedCondition { get; set; } = _ => true;

        /// <summary>
        /// Takes the <paramref name="centerPoint"/>, <paramref name="bitmap"/>, as well as the <paramref name="scaleFactor"/>, and
        /// turns it into a <see cref="List{T}"/> of <see cref="Polygon"/>s.
        /// </summary>
        /// <param name="centerPoint">The <see cref="Point"/> at the center of the <see cref="Polygon"/>s.</param>
        /// <param name="bitmap">The reference <see cref="Bitmap"/> to create the <see cref="Polygon"/>s from.</param>
        /// <param name="scaleFactor">The size of the individual <see cref="Polygon"/>s created.</param>
        /// <param name="pixelNotUsedCondition">Optional <see cref="Predicate{T}"/> that will only add <see cref="Polygon"/>s if it returns <see langword="true"/>.
        /// Defaults to <see cref="PixelNotUsedCondition"/>.</param>
        /// <returns></returns>
        public static List<Polygon> GetPolygonsFromImage(Point centerPoint, Bitmap bitmap, int scaleFactor, Predicate<Color>? pixelNotUsedCondition = null, int opacity = -1)
        {
            pixelNotUsedCondition ??= PixelNotUsedCondition;

            int width = bitmap.Width * scaleFactor;
            int height = bitmap.Height * scaleFactor;

            int startingX = centerPoint.X - width / 2;
            int startingY = centerPoint.Y - height / 2;

            List<Polygon> result = [];

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    int currentX = i * scaleFactor + startingX;
                    int currentY = j * scaleFactor + startingY;

                    Polygon polygon = new();
                    polygon.Points.Add(new Point(currentX, currentY));
                    polygon.Points.Add(new Point(currentX, currentY + scaleFactor));
                    polygon.Points.Add(new Point(currentX + scaleFactor, currentY + scaleFactor));
                    polygon.Points.Add(new Point(currentX + scaleFactor, currentY));
                    Color color = bitmap.GetPixel(i, j);

                    if (opacity >= 0)
                        color = Color.FromArgb(opacity, color.R, color.G, color.B);

                    polygon.Brush = new SolidBrush(color);

                    if (pixelNotUsedCondition(color))
                        result.Add(polygon);
                }
            }

            return result;
        }
    }
}

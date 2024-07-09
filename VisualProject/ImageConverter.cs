using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualProject
{
    public class ImageConverter
    {
        public static List<Polygon> GetPolygonsFromImage(Point centerPoint, Bitmap image, int scaleFactor, byte maxWhite = 255)
        {
            int width = image.Width * scaleFactor;
            int height = image.Height * scaleFactor;

            int startingX = centerPoint.X - width / 2;
            int startingY = centerPoint.Y - height / 2;

            List<Polygon> result = [];

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    int currentX = (i * scaleFactor) + startingX;
                    int currentY = (j * scaleFactor) + startingY;

                    Polygon polygon = new();
                    polygon.Points.Add(new Point(currentX, currentY));
                    polygon.Points.Add(new Point(currentX, currentY + scaleFactor));
                    polygon.Points.Add(new Point(currentX + scaleFactor, currentY + scaleFactor));
                    polygon.Points.Add(new Point(currentX + scaleFactor, currentY));
                    Color color = image.GetPixel(i, j);
                    polygon.Brush = new SolidBrush(color);

                    if (!(color.R >= maxWhite && color.G >= maxWhite && color.B >= maxWhite))
                        result.Add(polygon);
                }
            }

            return result;
        }
    }
}

namespace VisualProject
{
    public class Polygon
    {
        public List<Point> Points { get; set; } = new();
        public Brush Brush { get; set; } = Brushes.Black;
        public Rectangle ToRectangle()
        {
            if (Points == null || Points.Count != 4)
                throw new InvalidOperationException("Polygon doesn't have exactly four points");
            
            int minimumX = Math.Min(Math.Min(Points[0].X,Points[1].X),Math.Min(Points[2].X,Points[3].X));
            int minimumY = Math.Min(Math.Min(Points[0].Y,Points[1].Y),Math.Min(Points[2].Y,Points[3].Y));
            int maximumX = Math.Max(Math.Max(Points[0].X,Points[1].X),Math.Max(Points[2].X,Points[3].X));
            int maximumY = Math.Max(Math.Max(Points[0].Y,Points[1].Y),Math.Max(Points[2].Y,Points[3].Y));

            return new Rectangle(minimumX, minimumY, maximumX - minimumX, maximumY - minimumY);
        }
    }
}

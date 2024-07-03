using System.Drawing.Drawing2D;

namespace VisualProject
{
    public class PanelPainter
    {
        public int rectangleSize = 100;
        private Bitmap bitmap;
        private Graphics graphics;
        public int mouseX = 0;
        public int mouseY = 0;

        public List<IGameObject> Objects { get; set; } = new();
        public Player Player { get; set; } = new();

        public PanelPainter()
        {
            bitmap = new Bitmap(1, 1);
            graphics = Graphics.FromImage(bitmap);
            Objects.Add(Player);
        }

        public void Update(List<(Keys key, TimeSpan time)> pressedTimers)
        {
            Objects.ForEach(gameObject => gameObject.Update(pressedTimers));

            if (pressedTimers.Any(x => x.key == Keys.Space))
                Objects.Add(new Projectile(Player.X, Player.Y, (mouseX, mouseY), TimeSpan.FromSeconds(10)));

            Objects.RemoveAll(gameObject => !gameObject.Update(pressedTimers));
        }

        public void Paint(PaintEventArgs e, bool paintChange)
        {
            bitmap ??= new Bitmap(e.ClipRectangle.Width, e.ClipRectangle.Height);

            if (!paintChange)
            {
                e.Graphics.DrawImage(bitmap, 0, 0);
                return;
            }

            bitmap = new Bitmap(e.ClipRectangle.Width, e.ClipRectangle.Height);
            graphics = Graphics.FromImage(bitmap);
            PaintObjects();
            e.Graphics.DrawImage(bitmap, 0, 0);
        }

        private void PaintObjects()
        {
            foreach (var gameObject in Objects)
                foreach (Polygon polygon in gameObject.GetObjectSprite())
                {
                    graphics.FillPolygon(polygon.Brush, polygon.Points.ToArray());
                }
        }
    }
}

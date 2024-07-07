using System.Drawing.Drawing2D;

namespace VisualProject
{
    public class PanelPainter
    {
        public int rectangleSize = 100;
        private Bitmap bitmap;
        private Graphics graphics;
        public int MouseX = 0;
        public int MouseY = 0;

        public List<IGameObject> Objects { get; set; } = new();
        public Player Player { get; set; } = new();
        public PaintEventArgs LastEventArgs { get; set; }

        public PanelPainter()
        {
            bitmap = new Bitmap(1, 1);
            graphics = Graphics.FromImage(bitmap);
            Objects.Add(Player);
            LastEventArgs = new(graphics, new Rectangle());
        }

        public void Update(List<(Keys key, TimeSpan time)> pressedTimers)
        {
            if (pressedTimers.Any(x => x.key == Keys.Space) && Player.CanFire)
            {
                Objects.Add(new Projectile(Player.X, Player.Y, (MouseX, MouseY), TimeSpan.FromSeconds(10)));
                Player.CanFire = false;
            }

            if (!Objects.Any(obj => obj is Enemy))
            {
                int max = new Random().Next(10,20);

                for (int i = 0; i < max; i++)
                    Objects.Add(new Enemy(Player, LastEventArgs.ClipRectangle));
            }

            Objects.RemoveAll(gameObject => !gameObject.Update(pressedTimers, Objects));
        }

        public void Paint(PaintEventArgs e, bool paintChange)
        {
            LastEventArgs = e;
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

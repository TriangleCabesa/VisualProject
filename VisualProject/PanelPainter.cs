namespace VisualProject
{
    public class PanelPainter
    {
        public Point MouseLocation { get; set; } = new Point(0, 0);
        public List<IGameObject> Objects { get; set; } = [];
        public Player Player { get; set; } = new();
        public PaintEventArgs LastEventArgs { get; set; }

        private Bitmap bitmap;
        private Graphics graphics;

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
                Objects.Add(new Projectile(new Point(Player.Location.X, Player.Location.Y), new Point(MouseLocation.X, MouseLocation.Y), TimeSpan.FromSeconds(10)));
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

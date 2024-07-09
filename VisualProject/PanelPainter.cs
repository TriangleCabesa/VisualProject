namespace VisualProject
{
    public class PanelPainter
    {
        public PaintEventArgs LastWindow { get; set; }
        public Point MouseLocation { get; set; }
        public List<IGameObject> Objects { get; set; }
        public Player Player { get; set; }

        private Bitmap bitmap;
        private Graphics graphics;

        public PanelPainter()
        {
            MouseLocation = new Point(0, 0);
            Objects = [];
            Player = new();
            Objects.Add(Player);
            bitmap = new Bitmap(800, 450);
            graphics = Graphics.FromImage(bitmap);
            LastWindow = new(graphics, new Rectangle(0, 0, 800, 450));
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
                    Objects.Add(new Enemy(Player, LastWindow.ClipRectangle));
            }

            Objects.RemoveAll(gameObject => !gameObject.Update(pressedTimers, Objects));
        }

        public void Paint(PaintEventArgs window, bool paintChange)
        {
            LastWindow = window;
            bitmap ??= new Bitmap(LastWindow.ClipRectangle.Width, LastWindow.ClipRectangle.Height);

            if (!paintChange)
            {
                LastWindow.Graphics.DrawImage(bitmap, 0, 0);
                return;
            }

            bitmap = new Bitmap(LastWindow.ClipRectangle.Width, LastWindow.ClipRectangle.Height);
            graphics = Graphics.FromImage(bitmap);
            PaintObjects();
            LastWindow.Graphics.DrawImage(bitmap, 0, 0);
        }

        private void PaintObjects()
        {
            foreach (IGameObject gameObject in Objects)
                foreach (Polygon polygon in gameObject.GetObjectSprite())
                    graphics.FillPolygon(polygon.Brush, polygon.Points.ToArray());
        }
    }
}

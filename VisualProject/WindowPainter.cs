using VisualProject.Implementations;
using VisualProject.Interfaces;

namespace VisualProject
{
    public class WindowPainter
    {
        public PaintEventArgs LastPaintEvent { get; set; }
        public Point MouseLocation { get; set; }
        public List<IGameObject> Objects { get; set; }
        public IScreen? Screen { get; set; }
        public Player Player { get; set; }

        private static Window? _window;
        private Bitmap _bitmap;
        private Graphics _graphics;

        public WindowPainter()
        {
            MouseLocation = new Point(0, 0);
            Objects = [];
            Player = new();
            Objects.Add(Player);
            _bitmap = new Bitmap(800, 450);
            _graphics = Graphics.FromImage(_bitmap);
            LastPaintEvent = new(_graphics, new Rectangle(0, 0, 800, 450));
            Enemy.SetPlayer(Player);
        }

        public static void SetWindow(Window window) => _window = window;

        public void Update()
        {
            if (!Objects.Any(o => o is Player))
            {
                HandleRestartScreen();

                return;
            }

            AddNewEntities();

            for (int i = 0; i < Objects.Count; i++)
            {
                CheckForCollisions(i);
            }

            Objects.RemoveAll(gameObject => !gameObject.Update(_window!.PressedKeyTimers, Objects));
        }

        public void Paint(PaintEventArgs paintEvent)
        {
            LastPaintEvent = paintEvent;
            _bitmap ??= new Bitmap(LastPaintEvent.ClipRectangle.Width, LastPaintEvent.ClipRectangle.Height);

            if (!_window!.CanPaint)
            {
                LastPaintEvent.Graphics.DrawImage(_bitmap, 0, 0);
                return;
            }

            _bitmap = new Bitmap(_window.Width, _window.Height);
            _graphics = Graphics.FromImage(_bitmap);
            _graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            PaintObjects();
            
            LastPaintEvent.Graphics.DrawImage(_bitmap, 0, 0);
        }

        #region HelperMethods
        private void AddNewEntities()
        {
            if (_window!.PressedKeyTimers.Any(x => x.key == Keys.Space) && Player.CanFire)
            {
                Objects.Add(new Bullet(Player.ProjectileOrigin, new Point(MouseLocation.X, MouseLocation.Y), TimeSpan.FromSeconds(10), typeof(Player)));
                Player.CanFire = false;
            }

            if (!Objects.Any(obj => obj is Enemy))
            {
                int max = new Random().Next(10, 20);

                for (int i = 0; i < max; i++)
                    Objects.Add(new Enemy(LastPaintEvent.ClipRectangle));
            }
        }

        private void CheckForCollisions(int i)
        {
            for (int j = i; j < Objects.Count; j++)
            {
                if (j == i)
                    continue;

                IGameObject objectOne = Objects[i];
                IGameObject objectTwo = Objects[j];

                if (objectOne.GetType() == objectTwo.GetType())
                    continue;

                if (objectOne is not ICollidable collidableOne || objectTwo is not ICollidable collidableTwo)
                    continue;

                if (collidableOne.CollidesWith(collidableTwo))
                {
                    collidableOne.HandleCollision(collidableTwo);
                    collidableTwo.HandleCollision(collidableOne);
                }
            }
        }

        private void PaintObjects()
        {
            foreach (IGameObject gameObject in Objects)
                foreach (Polygon polygon in gameObject.GetObjectSprite())
                    _graphics.FillPolygon(polygon.Brush, polygon.Points.ToArray());

            if (Screen is not null)
            {
                _graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0, 0)), LastPaintEvent.ClipRectangle);

                var screenContents = Screen.GetButtons(LastPaintEvent.ClipRectangle);

                foreach (var(polygon, text) in screenContents)
                {
                    _graphics.FillPolygon(polygon.Brush, polygon.Points.ToArray());
                    _graphics.DrawString(text.Text, text.Font, text.Brush, text.Bounds, text.StringFormat);
                }
            }
        }

        private void HandleRestartScreen()
        {
            Screen ??= new RestartScreen();

            Action? action = null;

            if (!_window!.MouseEventArgsList.Any(x => x.Button == MouseButtons.Left))
                return;

            foreach (var e in _window.MouseEventArgsList)
            {
                action ??= Screen.GetClickedButtonAction(e.Location, this);
            }

            if (action is not null)
                action();
        }
        #endregion
    }
}

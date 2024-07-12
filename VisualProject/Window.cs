using System.Diagnostics;
using VisualProject.HelperClasses;

namespace VisualProject
{
    public partial class Window : Form
    {
        private bool _shootPressed = false;
        public int FrameRate = 120;
        public bool CanPaint = true;
        bool windowOpen = true;
        PanelPainter? painter;
        public List<(Keys key, Stopwatch timer)> KeyList = [];
        public List<(Keys key, TimeSpan time)> PressedKeyTimers = [];
        Stopwatch Stopwatch = Stopwatch.StartNew();

        public Window()
        {
            InitializeComponent();
            DoubleBuffered = true;
            PanelPainter.SetWindow(this);

            Task.Run(() =>
            {
                while (true)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    while (stopwatch.Elapsed < TimeSpan.FromMilliseconds(1000 / FrameRate))
                        ;

                    CanPaint = true;
                }
            });
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            painter ??= new PanelPainter();
            painter.Paint(e);
        }

        private void Window_Shown(object sender, EventArgs e)
        {
            while (windowOpen)
            {
                RefreshPanel();
                DoEvents();
            }
        }

        private void RefreshPanel()
        {
            if (!CanPaint)
                return;

            UpdateVariables();
            Refresh();
            CanPaint = false;
        }

        private void UpdateVariables()
        {
            painter ??= new PanelPainter();

            foreach (var (key, stopwatch) in KeyList)
                PressedKeyTimers.Add((key, stopwatch.Elapsed));

            for (int i = 0; i < KeyList.Count; i++)
                KeyList[i] = (KeyList[i].key, Stopwatch.StartNew());

            if (!PressedKeyTimers.Any(x => x.key == Keys.Space) && _shootPressed)
                PressedKeyTimers.Add((Keys.Space, TimeSpan.FromTicks(1)));

            PressedKeyTimers.Add((Keys.F20, Stopwatch.Elapsed));
            Stopwatch.Restart();

            foreach (var (key, timeSpan) in PressedKeyTimers.Where(x => x.key == Keys.Left || x.key == Keys.Right))
            {
                FrameRate += key == Keys.Right ? 1 : FrameRate > 1 ? -1 : 0;
            }

            painter.Update();
            PressedKeyTimers.Clear();
        }

        private void DoEvents()
        {
            while (!CanPaint)
            {
                Application.DoEvents();
            }
        }

        private void Window_SizeChanged(object sender, EventArgs e)
        {
            RefreshPanel();
        }

        private void Window_Resize(object sender, EventArgs e)
        {
            RefreshPanel();
        }

        private void Window_ResizeBegin(object sender, EventArgs e)
        {
            RefreshPanel();
        }

        private void Window_ResizeEnd(object sender, EventArgs e)
        {
            RefreshPanel();
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            windowOpen = false;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!KeyList.Any(x => x.key == e.KeyCode))
                KeyList.Add((e.KeyCode, Stopwatch.StartNew()));
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (KeyList.Any(x => x.key == e.KeyCode))
            {
                PressedKeyTimers.Add((e.KeyCode, KeyList.First(x => x.key == e.KeyCode).timer.Elapsed));
                KeyList.RemoveAll(x => x.key == e.KeyCode);
            }
        }

        private void Window_Paint(object sender, PaintEventArgs e)
        {
            painter ??= new PanelPainter();
            painter.Paint(e);
        }

        private void Window_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                if (e.OldValue < e.NewValue)
                    PressedKeyTimers.Add((Keys.Up, TimeSpan.FromMilliseconds(2)));
                else
                    PressedKeyTimers.Add((Keys.Down, TimeSpan.FromMilliseconds(2)));
            }
        }

        private void Window_MouseWheel(object sender, MouseEventArgs e)
        {
            PressedKeyTimers.Add((Keys.Up, TimeSpan.FromMilliseconds(e.Delta / 60)));
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            painter ??= new PanelPainter();
            painter.MouseLocation = e.Location;
        }

        private void Window_Load(object sender, EventArgs e)
        {
            ImageReader.PixelNotUsedCondition = color => !(color.R >= 215 && color.G >= 215 && color.B >= 215);
        }

        private void Window_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _shootPressed = true;
        }

        private void Window_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _shootPressed = false;
        }
    }
}

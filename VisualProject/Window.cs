using System.Diagnostics;
using System.Reflection;

namespace VisualProject
{
    public partial class Window : Form
    {
        int frameRate = 120;
        bool canPaint = true;
        bool windowOpen = true;
        PanelPainter? painter;
        List<(Keys key, Stopwatch timer)> keyList = [];
        List<(Keys key, TimeSpan time)> pressedTimers = [];
        Stopwatch Stopwatch = Stopwatch.StartNew();
        
        public Window()
        {
            InitializeComponent();
            DoubleBuffered = true;

            Task.Run(() =>
            {
                while (true)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    while (stopwatch.Elapsed < TimeSpan.FromMilliseconds(1000 / frameRate))
                        ;

                    canPaint = true;
                }
            });
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            painter ??= new PanelPainter();
            painter.Paint(e, canPaint);
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
            if (!canPaint)
                return;

            UpdateVariables();
            Refresh();
            canPaint = false;
        }

        private void UpdateVariables()
        {
            painter ??= new PanelPainter();

            foreach (var (key, stopwatch) in keyList)
                pressedTimers.Add((key, stopwatch.Elapsed));

            for (int i = 0; i < keyList.Count; i++)
                keyList[i] = (keyList[i].key, Stopwatch.StartNew());

            pressedTimers.Add((Keys.F20, Stopwatch.Elapsed));
            Stopwatch.Restart();

            foreach (var (key, timeSpan) in pressedTimers.Where(x => x.key == Keys.Left || x.key == Keys.Right))
            {
                frameRate += key == Keys.Right ? 1 : frameRate > 1 ? -1 : 0;
            }

            painter.Update(pressedTimers);
            pressedTimers.Clear();
        }

        private void DoEvents()
        {
            while (!canPaint)
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
            if (!keyList.Any(x => x.key == e.KeyCode))
                keyList.Add((e.KeyCode, Stopwatch.StartNew()));
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (keyList.Any(x => x.key == e.KeyCode))
            {
                pressedTimers.Add((e.KeyCode, keyList.First(x => x.key == e.KeyCode).timer.Elapsed));
                keyList.RemoveAll(x => x.key == e.KeyCode);
            }
        }

        private void Window_Paint(object sender, PaintEventArgs e)
        {
            painter ??= new PanelPainter();
            painter.Paint(e, canPaint);
        }

        private void Window_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                if (e.OldValue < e.NewValue)
                    pressedTimers.Add((Keys.Up, TimeSpan.FromMilliseconds(2)));
                else
                    pressedTimers.Add((Keys.Down, TimeSpan.FromMilliseconds(2)));
            }
        }

        private void Window_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta == 120 || e.Delta == -120)
                pressedTimers.Add((Keys.Up, TimeSpan.FromMilliseconds(e.Delta / 60)));
            else
                pressedTimers.Add((Keys.Up, TimeSpan.FromMilliseconds(e.Delta / 15)));
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            painter ??= new PanelPainter();
            painter.MouseX = e.X;
            painter.MouseY = e.Y;
        }
    }
}

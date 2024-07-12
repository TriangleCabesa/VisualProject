using VisualProject.Extensions;
using VisualProject.Interfaces;

namespace VisualProject.Implementations
{
    public class PauseScreen : IScreen
    {
        private static (int width, int height) _buttonSize = (160, 40);

        public List<(Polygon polygon, GraphicsString text)> GetButtons(Rectangle window)
        {
            Polygon polygon = new();
            polygon.Points.Add(new Point(window.Width / 2 - _buttonSize.width / 2, window.Height / 2 - _buttonSize.height / 2));
            polygon.Points.Add(new Point(window.Width / 2 + _buttonSize.width / 2, window.Height / 2 - _buttonSize.height / 2));
            polygon.Points.Add(new Point(window.Width / 2 + _buttonSize.width / 2, window.Height / 2 + _buttonSize.height / 2));
            polygon.Points.Add(new Point(window.Width / 2 - _buttonSize.width / 2, window.Height / 2 + _buttonSize.height / 2));
            polygon.Brush = new SolidBrush(Color.LightGray);

            StringFormat format = new();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;
            format.FormatFlags = StringFormatFlags.FitBlackBox;

            GraphicsString text = new()
            {
                Text = "Paused",
                Bounds = polygon.ToRectangle(),
                StringFormat = format,
                Brush = Brushes.Red,
            };

            return [(null, text)];
        }

        public Action? GetClickedButtonAction(Point point, WindowPainter painter)
        {
            return () => { painter.Screen = null; };
        }
    }
}

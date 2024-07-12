using VisualProject.Extensions;
using VisualProject.Interfaces;

namespace VisualProject.Implementations
{
    public class RestartScreen : IScreen
    {
        private List<Polygon> _polygons = [];
        private static (int width, int height) _buttonSize = (160, 40);

        public List<(Polygon polygon, GraphicsString text)> GetButtons(Rectangle window)
        {
            _polygons = [];

            Polygon polygon = new();
            polygon.Points.Add(new Point(window.Width / 2 - _buttonSize.width / 2, window.Height / 2 - _buttonSize.height / 2));
            polygon.Points.Add(new Point(window.Width / 2 + _buttonSize.width / 2, window.Height / 2 - _buttonSize.height / 2));
            polygon.Points.Add(new Point(window.Width / 2 + _buttonSize.width / 2, window.Height / 2 + _buttonSize.height / 2));
            polygon.Points.Add(new Point(window.Width / 2 - _buttonSize.width / 2, window.Height / 2 + _buttonSize.height / 2));
            polygon.Brush = new SolidBrush(Color.LightGray);

            StringFormat format = new();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;

            GraphicsString text = new()
            {
                Text = "Restart",
                Bounds = polygon.ToRectangle(),
                StringFormat = format,
            };

            _polygons.Add(polygon);

            return [(polygon, text)];
        }

        public Action? GetClickedButtonAction(Point point, WindowPainter painter)
        {
            if (!_polygons.Any(p => p.IsPointInPolygon(point)))
                return null;

            return () =>
            {
                painter.Player = new();
                painter.Objects.Clear();
                painter.Objects.Add(painter.Player);
                Enemy.SetPlayer(painter.Player);
                painter.Screen = null;
            };
        }
    }
}

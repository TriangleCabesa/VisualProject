namespace VisualProject.Interfaces
{
    public interface IScreen
    {
        List<(Polygon polygon, GraphicsString text)> GetButtons(Rectangle window);

        Action? GetClickedButtonAction(Point point, WindowPainter painter);
    }
}

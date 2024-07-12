using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualProject
{
    public class GraphicsString
    {
        public string Text { get; set; }
        public Font Font { get; set; }
        public Brush Brush { get; set; }
        public RectangleF Bounds { get; set; }
        public StringFormat StringFormat { get; set; }

        public GraphicsString()
        {
            Text = string.Empty;
            Font = new("Arial", 24);
            Brush = new SolidBrush(Color.Black);
            Bounds = new RectangleF();
            StringFormat = new();
        }
    }
}

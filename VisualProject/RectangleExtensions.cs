﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualProject
{
    public static class RectangleExtensions
    {
        public static List<(Brush brush, Rectangle rectangle)> AsHollowedSquare(this Rectangle rectangle, Brush brush, double filledPercentage)
        {
            filledPercentage /= 2;
            int X = rectangle.X;
            int Y = rectangle.Y;
            double Size = rectangle.Size.Width;

            return new()
            {
                (brush, new Rectangle(X, Y, (int)(Size * filledPercentage), (int)Size)),
                (brush, new Rectangle(X, Y, (int)Size, (int)(Size * filledPercentage))),
                (brush, new Rectangle(X + (int)Size - (int)(Size * filledPercentage), Y, (int)(Size * filledPercentage), (int)Size)),
                (brush, new Rectangle(X, Y + (int)Size - (int)(Size * filledPercentage), (int)Size, (int)(Size * filledPercentage))),
            };
        }

        public static Point Rotate(this Point point, Point centerPoint, double angle)
        {
            if (angle == 0)
                return new Point(point.X,point.Y);
            
            return new Point((int)(centerPoint.X + (point.X - centerPoint.X) * Math.Cos(angle) - (point.Y - centerPoint.Y) * Math.Sin(angle)),
                             (int)(centerPoint.Y + (point.X - centerPoint.X) * Math.Sin(angle) + (point.Y - centerPoint.Y) * Math.Cos(angle)));
        }
    }
}
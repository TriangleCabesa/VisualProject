﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualProject
{
    public static class PolygonExtensions
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

        public static bool PolygonsIntersect(this Polygon polygonOne, Polygon polygonTwo)
        {
            for (int i = 0; i < polygonOne.Points.Count; i++)
            {
                int POSPI = i + 1 == polygonOne.Points.Count ? 0 : i + 1; //POSPI - Polygon One, Second Point Index

                for (int j = 0; j < polygonTwo.Points.Count; j++)
                {
                    int PTSPI = j + 1 == polygonTwo.Points.Count ? 0 : j + 1; //PTSPI - Polygon Two, Second Point Index

                    if (LinesIntersect((polygonOne.Points[i], polygonOne.Points[POSPI]), (polygonTwo.Points[j], polygonTwo.Points[PTSPI])))
                        return true;
                }
            }

            return false;
        }

        private static bool LinesIntersect((Point a, Point b) lineOne, (Point a, Point b) lineTwo)
        {
            // Variable names copied from linear algebra notation.
            double A1 = lineOne.b.Y - lineOne.a.Y;
            double B1 = lineOne.a.X - lineOne.b.X;
            double C1 = A1 * lineOne.a.X + B1 * lineOne.a.Y;
            double A2 = lineTwo.b.Y - lineTwo.a.Y;
            double B2 = lineTwo.a.X - lineTwo.b.X;
            double C2 = A2 * lineTwo.a.X + B2 * lineTwo.a.Y;
            double delta = A1 * B2 - A2 * B1;

            if (delta == 0)
                return false;

            double xIntercept = (B2 * C1 - B1 * C2) / delta;
            double yIntercept = (A1 * C2 - A2 * C1) / delta;

            bool onLineOneX = Math.Max(lineOne.a.X, lineOne.b.X) >= xIntercept
                           && Math.Min(lineOne.a.X, lineOne.b.X) <= xIntercept;
            bool onLineOneY = Math.Max(lineOne.a.Y, lineOne.b.Y) >= yIntercept
                           && Math.Min(lineOne.a.Y, lineOne.b.Y) <= yIntercept;
            bool onLineTwoX = Math.Max(lineTwo.a.X, lineTwo.b.X) >= xIntercept
                           && Math.Min(lineTwo.a.X, lineTwo.b.X) <= xIntercept;
            bool onLineTwoY = Math.Max(lineTwo.a.Y, lineTwo.b.Y) >= yIntercept
                           && Math.Min(lineTwo.a.Y, lineTwo.b.Y) <= yIntercept;

            return onLineOneX && onLineOneY && onLineTwoX && onLineTwoY;
        }

        /// <summary>
        /// Determines if the given point is inside the polygon
        /// </summary>
        /// <param name="points">the vertices of polygon</param>
        /// <param name="testPoint">the given point</param>
        /// <returns>true if the point is inside the polygon; otherwise, false</returns>
        public static bool IsPointInPolygon(this Polygon polygon, Point testPoint)
        {
            Point[] points = [.. polygon.Points];

            bool result = false;
            int j = points.Length - 1;

            for (int i = 0; i < points.Length; i++)
            {
                if (points[i].Y < testPoint.Y && points[j].Y >= testPoint.Y
                 || points[j].Y < testPoint.Y && points[i].Y >= testPoint.Y)
                {
                    bool rayIntersectsPolygon = points[i].X + (testPoint.Y - points[i].Y) /
                        (points[j].Y - points[i].Y) *
                        (points[j].X - points[i].X) < testPoint.X;

                    if (rayIntersectsPolygon)
                        result = !result;
                }
                
                j = i;
            }

            return result;
        }
    }
}
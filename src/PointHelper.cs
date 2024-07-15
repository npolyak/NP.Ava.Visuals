﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using NP.Utilities;
using System;

namespace NP.Ava.Visuals
{
    public static class PointHelper
    {
        public static Point2D ToPoint2D(this Point point)
        {
            return new Point2D(point.X, point.Y);
        }


        public static Point2D ToPoint2D(this PixelPoint point)
        {
            return new Point2D(point.X, point.Y);
        }

        public static Point ToPoint(this Point2D pt)
        {
            return new Point(pt.X, pt.Y);
        }

        public static PixelPoint ToPixelPoint(this Point2D point, double scale = 1)
        {
            return new PixelPoint((int) (point.X * scale), (int) (point.Y * scale));
        }

        public static PixelPoint ToPixelPoint(this Point point, double scale = 1)
        {
            return point.ToPoint2D().ToPixelPoint(scale);
        }

        public static Point2D ToPoint2D(this Size size)
        {
            return new Point2D(size.Width, size.Height);
        }

        public static Size ToSize(this Point2D pt)
        {
            return new Size(pt.X, pt.Y);
        }

        public static Point2D MinimumDragDistance =>
            new Point2D(3.2, 3.2);

        public static Rect ToRect(this Rect2D rect)
        {
            return new Rect(rect.StartPoint.ToPoint(), rect.EndPoint.ToPoint());
        }

        public static Rect2D ToRect2D(this Rect rect)
        {
            return new Rect2D(rect.TopLeft.ToPoint2D(), rect.BottomRight.ToPoint2D());
        }

        public static Point2D GetSize(this Control c)
        {
            return new Point2D(c.Bounds.Width, c.Bounds.Height);
        }

        public static bool IsPointWithinControl(this Control c, Point p)
        {
            Rect2D bounds = new Rect2D(new Point2D(), c.GetSize());

            return bounds.ContainsPoint(p.ToPoint2D());
        }

        public static Rect2D GetScreenBounds(this InputElement c)
        {
            PixelPoint startPoint = 
                c.PointToScreen(new Point(0, 0));
            
            PixelPoint endPoint = 
                c.PointToScreen(new Point(c.Bounds.Width, c.Bounds.Height));

            return new Rect2D(startPoint.ToPoint2D(), endPoint.ToPoint2D());
        }

        public static bool IsPointerWithinControl(this Control c, PointerEventArgs e)
        {
            return c.IsPointWithinControl(e.GetPosition(c));
        }

        public static bool IsLeftMousePressed(this Control c, PointerEventArgs e)
        {
            var props = e.GetCurrentPoint(c).Properties;

            return props.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed;
        }

        public static Point Add(this Point point1,  Point point2)
        {
            return new Point(point1.X + point2.X, point1.Y + point2.Y);
        }

        public static Point Subtract(this Point point1, Point point2)
        {
            return new Point(point1.X - point2.X, point1.Y - point2.Y);
        }

        public static double SquareDist(this Point p1, Point p2)
        {
            return p1.Subtract(p2).ToPoint2D().AbsSquared();
        }

        public static double Dist(this Point p1, Point p2)
        {
            return Math.Sqrt(p1.SquareDist(p2));
        }

        public static Point ToPoint(this Rect rect)
        {
            return new Point(rect.Width, rect.Height);
        }

        public static Point ToPoint(this Control c)
        {
            return new Point(c.Bounds.Width, c.Bounds.Height);
        }

        public static Rect ToRect(this Control c)
        {
            return new Rect(new Point(), c.ToPoint());
        }

        public static double ActualWidth(this Control c)
        {
            return c.Bounds.Width;
        }

        public static double ActualHeight(this Control c)
        {
            return c.Bounds.Height;
        }
    }
}

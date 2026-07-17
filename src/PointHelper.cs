global using Point2D = NP.Utilities.Point.Point2D<double>;
global using Rect2D = NP.Utilities.Point.Rect2D<double>;

using Avalonia;
using Avalonia.Input;
using NP.Utilities.Point;
using System;


namespace NP.Ava.Visuals
{
    public static class PointHelper
    {
        public static Point Origin { get; } = new Point(0, 0);

        public static IPoint2D<double> ToPoint2D(this Point point)
        {
            return new Point2D(point.X, point.Y);
        }


        public static IPoint2D<double> ToPoint2D(this PixelPoint point)
        {
            return new Point2D<double>(point.X, point.Y);
        }

        public static Point ToPoint(this IPoint2D<double> pt)
        {
            return new Point(pt.X, pt.Y);
        }

        public static PixelPoint ToPixelPoint(this IPoint2D<double> point, double scale = 1)
        {
            return new PixelPoint((int)(point.X * scale), (int)(point.Y * scale));
        }

        public static PixelPoint ToPixelPoint(this Point point, double scale = 1)
        {
            return point.ToPoint2D().ToPixelPoint(scale);
        }

        public static Point2D ToPoint2D(this Size size)
        {
            return new Point2D(size.Width, size.Height);
        }

        public static Size ToSize(this IPoint2D<double> pt)
        {
            return new Size(pt.X, pt.Y);
        }

        public static double MinimumDragDistance { get; } = 3.2;

        public static PixelPoint OriginToScreen(this Visual visual)
        {
            return visual.PointToScreen(Origin);
        }

        public static Rect ToRect(this Rect2D rect)
        {
            var result =  new Rect(rect.StartPoint.ToPoint(), rect.EndPoint.ToPoint());

            return result;
        }

        public static Rect2D ToRect2D(this Rect rect)
        {
            return new Rect2D(rect.TopLeft.ToPoint2D(), rect.BottomRight.ToPoint2D());
        }

        public static Point2D GetSize(this Visual c)
        {
            return new Point2D(c.Bounds.Width, c.Bounds.Height);
        }

        public static bool IsPointWithinControl(this Visual c, Point p)
        {
            Rect2D<double> bounds = new Rect2D<double>(new Point2D<double>(), c.GetSize());
            
            return bounds.ContainsPoint(p.ToPixelPoint());
        }

        public static Rect GetBounds(this Visual c)
        {
            return new Rect(0, 0, c.ActualWidth(), c.ActualHeight());
        }

        public static Rect2D GetScreenBounds(this InputElement c)
        {
            PixelPoint startPoint = 
                c.PointToScreen(Origin);
            
            PixelPoint endPoint = 
                c.PointToScreen(new Point(c.Bounds.Width, c.Bounds.Height));

            return new Rect2D(startPoint.ToPoint2D(), endPoint.ToPoint2D());
        }

        public static Point ToPoint(this Size size)
        {
            return size.ToPoint2D().ToPoint();
        }

        public static Rect GetBoundsWithinVisual(this Visual v, Visual relativeTo, Side2D currentSide, double sideScaleFactor = 0.5)
        {
            var boundRect = v.Bounds;

            Rect rect = new Rect(boundRect.Size);

            Rect sideRect = rect.ScaleToSide(sideScaleFactor, currentSide);

            Point? startPointObj = v.TranslatePoint(sideRect.TopLeft, relativeTo);

            if (startPointObj == null)
            {
                return new Rect();
            }

            Point startPoint = startPointObj.Value;

            Point endPoint =
                v.TranslatePoint(sideRect.BottomRight, relativeTo).Value;

            return new Rect(startPoint, endPoint);
        }

        public static Thickness ToMargin(this Visual v, Visual relativeTo, Side2D currentSide, double sideScaleFactor)
        {
            Rect rect = v.GetBoundsWithinVisual(relativeTo, currentSide, sideScaleFactor);

            (double relativeToWidth, double relativeToHeight) =
                relativeTo.Bounds.Size;

            double rightMargin = relativeToWidth - rect.Right;
            double bottomMargin = relativeToHeight - rect.Bottom;


            return new Thickness(rect.Left, rect.Top, rightMargin, bottomMargin);
        }


        public static bool IsPointerWithinControl(this Visual c, PointerEventArgs e)
        {
            return c.IsPointWithinControl(e.GetPosition(c));
        }

        public static bool IsLeftMousePressed(this Visual c, PointerEventArgs e)
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

        public static double SquareMagnitude(this Point p)
        {
            return p.X * p.X + p.Y * p.Y;
        }

        public static double Magnitude(this Point p)
        {
            return Math.Sqrt(p.SquareMagnitude());
        }

        public static double SquareMagnitude(this PixelPoint p)
        {
            return p.X * p.X + p.Y * p.Y;
        }

        public static double Magnitude(this PixelPoint p)
        {
            return Math.Sqrt(p.SquareMagnitude());
        }

        public static double SquareDist(this PixelPoint p1, PixelPoint p2)
        {
            return (p1 - p2).SquareMagnitude();
        }

        public static double Dist(this PixelPoint p1, PixelPoint p2)
        {
            return Math.Sqrt(p1.SquareDist(p2));
        }

        public static Point ToPoint(this Rect rect)
        {
            return new Point(rect.Width, rect.Height);
        }

        public static Point ToPoint(this Visual c)
        {
            return new Point(c.Bounds.Width, c.Bounds.Height);
        }

        public static Rect ToRect(this Visual c)
        {
            return new Rect(Origin, c.ToPoint());
        }

        public static Rect ScaleToSide(this Rect rect, double scale, Side2D sideToScaleTo)
        {
            return rect.ToRect2D().ScaleToSide(scale, sideToScaleTo).ToRect();
        }

        public static double ActualWidth(this Visual c)
        {
            return c.Bounds.Width;
        }

        public static double ActualHeight(this Visual c)
        {
            return c.Bounds.Height;
        }

        public static bool ContainsPoint(this Rect r, PixelPoint p)
        {
            return r.Contains(p.ToPoint(1d));
        }

        public static bool ContainsPoint(this Rect2D r, PixelPoint p)
        {
            return r.ToRect().ContainsPoint(p);
        }

        public static (PixelPoint, PixelPoint)
        FitRectangleToRectangle<T>
            (
                this Rect borderRect,
                Rect rectToPlaceInside
            )
        {
            (IPoint2D<double> boundPosition, IPoint2D<double> delta) = 
                
                borderRect.ToRect2D()
                            .FitRectangleToRectangle(rectToPlaceInside.ToRect2D());

            return (boundPosition.ToPixelPoint(), delta.ToPixelPoint());    
        }

        public static Point Shift(this Point point, Point shift)
        {
            return point + shift;
        }

        public static PixelPoint Shift(this PixelPoint point, PixelPoint shift)
        {
            return point + shift;
        }

        public static Rect Shift(this Rect r, Point shift)
        {
            return new Rect(r.TopLeft + shift, r.BottomRight + shift);
        }
    }
}

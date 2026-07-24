using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UI.Controls.HUD.Factories;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Factories
{
    /// <summary>
    /// Creates reusable geometry used throughout the HUD.
    /// Contains NO styling, brushes, themes or animations.
    /// </summary>
    public class HudShapeFactory
    {
        #region Basic Shapes

        public static Geometry CreateTriangle(
     Point center,
     double radius)
        {
            return CreatePolygon(center, radius, 3);
        }

        public static Geometry CreateTriangle(
            Point center,
            double radius,
            double startAngle)
        {
            return CreatePolygon(center, radius, 3, startAngle);
        }

        public static Geometry CreateHexagon(
            Point center,
            double radius)
        {
            return CreatePolygon(center, radius, 6);
        }

        public static Geometry CreateOctagon(
            Point center,
            double radius)
        {
            return CreatePolygon(center, radius, 8);
        }
        
        public static Geometry Rectangle(double width, double height)
        {
            return new RectangleGeometry(
                new Rect(0, 0, width, height));
        }

        public static Geometry RoundedRectangle(
            double width,
            double height,
            double radius)
        {
            return new RectangleGeometry(
                new Rect(0, 0, width, height),
                radius,
                radius);
        }

        public static Geometry Circle(double radius)
        {
            return new EllipseGeometry(
                new Point(radius, radius),
                radius,
                radius);
        }

        public static Geometry Ellipse(
            double width,
            double height)
        {
            return new EllipseGeometry(
                new Rect(0, 0, width, height));
        }

        public static Geometry Line(
            Point start,
            Point end)
        {
            return new LineGeometry(start, end);
        }
        public static Geometry CreateLine(
            Point start,
            Point end)
        {
            StreamGeometry geometry = new();

            using (StreamGeometryContext context = geometry.Open())
            {
                context.BeginFigure(
                    start,
                    false,
                    false);

                context.LineTo(
                    end,
                    true,
                    false);
            }

            geometry.Freeze();

            return geometry;
        }

        
        #endregion

        #region Polygon
        public static Geometry CreatePolygon(
            Point center,
            double radius,
            int sides,
            double startAngle = -90)
        {
            if (sides < 3)
                throw new ArgumentOutOfRangeException(nameof(sides));

            StreamGeometry geometry = new();

            using (StreamGeometryContext context = geometry.Open())
            {
                for (int i = 0; i < sides; i++)
                {
                    double angle =
                        (startAngle + (360.0 / sides) * i) *
                        Math.PI / 180.0;

                    Point point = new(
                        center.X + radius * Math.Cos(angle),
                        center.Y + radius * Math.Sin(angle));

                    if (i == 0)
                    {
                        context.BeginFigure(point, false, true);
                    }
                    else
                    {
                        context.LineTo(point, true, false);
                    }
                }
            }

            geometry.Freeze();
            return geometry;
        }
        public static Geometry Polygon(params Point[] points)
        {
            if (points.Length < 3)
                throw new ArgumentException("Polygon requires at least three points.");

            var figure = new PathFigure
            {
                StartPoint = points[0],
                IsClosed = true
            };

            for (int i = 1; i < points.Length; i++)
            {
                figure.Segments.Add(
                    new LineSegment(points[i], true));
            }

            return new PathGeometry(
                new[]
                {
                    figure
                });
        }

        #endregion

        #region Arc

        public static Geometry Arc(
            Point center,
            double radius,
            double startAngle,
            double endAngle)
        {
            Point start = PointOnCircle(center, radius, startAngle);
            Point end = PointOnCircle(center, radius, endAngle);

            bool largeArc =
                Math.Abs(endAngle - startAngle) > 180;

            var figure = new PathFigure
            {
                StartPoint = start,
                IsClosed = false
            };

            figure.Segments.Add(
                new ArcSegment
                {
                    Point = end,
                    Size = new Size(radius, radius),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc = largeArc
                });

            return new PathGeometry(
                new[]
                {
                    figure
                });
        }
        public static Geometry CreateArc(
    Point center,
    double radius,
    double startAngle,
    double sweepAngle)
        {
            if (radius <= 0)
                throw new ArgumentOutOfRangeException(nameof(radius));

            StreamGeometry geometry = new();

            using (StreamGeometryContext context = geometry.Open())
            {
                double startRadians =
                    startAngle * Math.PI / 180.0;

                double endRadians =
                    (startAngle + sweepAngle) * Math.PI / 180.0;

                Point start = new(
                    center.X + radius * Math.Cos(startRadians),
                    center.Y + radius * Math.Sin(startRadians));

                Point end = new(
                    center.X + radius * Math.Cos(endRadians),
                    center.Y + radius * Math.Sin(endRadians));

                bool isLargeArc =
                    Math.Abs(sweepAngle) > 180;

                SweepDirection direction =
                    sweepAngle >= 0
                        ? SweepDirection.Clockwise
                        : SweepDirection.Counterclockwise;

                context.BeginFigure(
                    start,
                    false,
                    false);

                context.ArcTo(
                    end,
                    new Size(radius, radius),
                    0,
                    isLargeArc,
                    direction,
                    true,
                    false);
            }

            geometry.Freeze();

            return geometry;
        }
        #endregion

        #region Crosshair

        public static Geometry Crosshair(
            double radius,
            double gap)
        {
            GeometryGroup group = new();

            group.Children.Add(
                Line(
                    new Point(radius, 0),
                    new Point(radius, radius - gap)));

            group.Children.Add(
                Line(
                    new Point(radius, radius + gap),
                    new Point(radius, radius * 2)));

            group.Children.Add(
                Line(
                    new Point(0, radius),
                    new Point(radius - gap, radius)));

            group.Children.Add(
                Line(
                    new Point(radius + gap, radius),
                    new Point(radius * 2, radius)));

            return group;
        }

        #endregion

        #region Grid

        public static Geometry Grid(
            double width,
            double height,
            double spacing)
        {
            GeometryGroup group = new();

            for (double x = 0; x <= width; x += spacing)
            {
                group.Children.Add(
                    new LineGeometry(
                        new Point(x, 0),
                        new Point(x, height)));
            }

            for (double y = 0; y <= height; y += spacing)
            {
                group.Children.Add(
                    new LineGeometry(
                        new Point(0, y),
                        new Point(width, y)));
            }

            return group;
        }

        #endregion

        #region Hexagon

        public static Geometry Hexagon(double radius)
        {
            Point[] pts = new Point[6];

            for (int i = 0; i < 6; i++)
            {
                double angle =
                    (Math.PI / 180) * (60 * i - 30);

                pts[i] = new Point(
                    radius + radius * Math.Cos(angle),
                    radius + radius * Math.Sin(angle));
            }

            return Polygon(pts);
        }

        #endregion

        #region Helpers

        private static Point PointOnCircle(
            Point center,
            double radius,
            double angleDegrees)
        {
            double radians =
                angleDegrees * Math.PI / 180;

            return new Point(
                center.X + radius * Math.Cos(radians),
                center.Y + radius * Math.Sin(radians));
        }

        #endregion

        public static IReadOnlyList<Geometry> CreateTickRing(
                Point center,
                double radius,
                double tickLength,
                int tickCount)
        {
            if (tickCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(tickCount));

            List<Geometry> geometries = new(tickCount);

            double step = 360.0 / tickCount;

            for (int i = 0; i < tickCount; i++)
            {
                double angle =
                    (-90 + i * step) *
                    Math.PI / 180.0;

                Point start = new(
                    center.X + radius * Math.Cos(angle),
                    center.Y + radius * Math.Sin(angle));

                Point end = new(
                    center.X + (radius + tickLength) * Math.Cos(angle),
                    center.Y + (radius + tickLength) * Math.Sin(angle));

                StreamGeometry geometry = new();

                using (StreamGeometryContext context = geometry.Open())
                {
                    context.BeginFigure(start, false, false);
                    context.LineTo(end, true, false);
                }

                geometry.Freeze();

                geometries.Add(geometry);
            }

            return geometries;
        }

        #region Old Code
        public static Rectangle Rectangle(
            Point topLeft,
            double width,
            double height)
        {
            return new Rectangle
            {
                Width = width,
                Height = height,
                Fill = Brushes.Transparent,
                Stroke = Brushes.Transparent,
                StrokeThickness = 0
            };
        }

        public static Ellipse Ellipse(
            double width,
            double height,
            Brush fill,
            double opacity = 1.0)
        {
            return new Ellipse
            {
                Width = width,
                Height = height,
                Fill = fill,
                Opacity = opacity
            };
        }
        #endregion

        /// <summary>
        /// Creates a HUD line.
        /// </summary>
        public static Line CreateLine(
            Point start,
            Point end,
            HudTheme theme)
        {
            return new Line
            {
                X1 = start.X,
                Y1 = start.Y,

                X2 = end.X,
                Y2 = end.Y,

                Stroke = new SolidColorBrush(theme.PrimaryColor),

                StrokeThickness = theme.GridThickness,

                SnapsToDevicePixels = true
            };
        }

        /// <summary>
        /// Creates a HUD line with custom thickness.
        /// </summary>
        public static Line CreateLine(
            Point start,
            Point end,
            Brush brush,
            double thickness = 1.0)
        {
            return new Line
            {
                X1 = start.X,
                Y1 = start.Y,

                X2 = end.X,
                Y2 = end.Y,

                Stroke = brush,

                StrokeThickness = thickness,

                SnapsToDevicePixels = true
            };
        }
    }
}

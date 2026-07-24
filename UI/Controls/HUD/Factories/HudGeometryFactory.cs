using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace UI.Controls.HUD.Factories
{
    static class HudGeometryFactory
    {
        //---------------------------------------------------------
        // Circle
        //---------------------------------------------------------

        public static EllipseGeometry Circle(
            Point center,
            double radius)
        {
            return new EllipseGeometry(center, radius, radius);
        }

        //---------------------------------------------------------
        // Ring
        //---------------------------------------------------------

        public static Geometry Ring(
            Point center,
            double outerRadius,
            double innerRadius)
        {
            var outer = new EllipseGeometry(center, outerRadius, outerRadius);
            var inner = new EllipseGeometry(center, innerRadius, innerRadius);

            return Geometry.Combine(
                outer,
                inner,
                GeometryCombineMode.Exclude,
                null);
        }

        //---------------------------------------------------------
        // Arc
        //---------------------------------------------------------

        public static PathGeometry Arc(
            Point center,
            double radius,
            double startAngle,
            double sweepAngle)
        {
            Point start = PointOnCircle(center, radius, startAngle);

            Point end = PointOnCircle(
                center,
                radius,
                startAngle + sweepAngle);

            bool largeArc = sweepAngle > 180;

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

            return new PathGeometry(new[] { figure });
        }

        //---------------------------------------------------------
        // Radial Line
        //---------------------------------------------------------

        public static LineGeometry RadialLine(
            Point center,
            double innerRadius,
            double outerRadius,
            double angle)
        {
            Point p1 = PointOnCircle(center, innerRadius, angle);

            Point p2 = PointOnCircle(center, outerRadius, angle);

            return new LineGeometry(p1, p2);
        }

        //---------------------------------------------------------
        // Triangle
        //---------------------------------------------------------

        public static PathGeometry Triangle(
            Point center,
            double radius,
            double rotation = -90)
        {
            return RegularPolygon(
                center,
                radius,
                3,
                rotation);
        }

        //---------------------------------------------------------
        // Regular Polygon
        //---------------------------------------------------------

        public static PathGeometry RegularPolygon(
            Point center,
            double radius,
            int sides,
            double rotation = 0)
        {
            if (sides < 3)
                throw new ArgumentException("Polygon requires at least three sides.");

            var figure = new PathFigure();

            for (int i = 0; i < sides; i++)
            {
                double angle =
                    rotation + (360.0 / sides) * i;

                Point point =
                    PointOnCircle(center, radius, angle);

                if (i == 0)
                    figure.StartPoint = point;
                else
                    figure.Segments.Add(new LineSegment(point, true));
            }

            figure.IsClosed = true;

            return new PathGeometry(new[] { figure });
        }

        //---------------------------------------------------------
        // Tick Ring
        //---------------------------------------------------------

        public static IEnumerable<LineGeometry> TickRing(
            Point center,
            double radius,
            double tickLength,
            int tickCount)
        {
            double step = 360.0 / tickCount;

            for (int i = 0; i < tickCount; i++)
            {
                yield return RadialLine(
                    center,
                    radius,
                    radius + tickLength,
                    i * step);
            }
        }

        //---------------------------------------------------------
        // Point On Circle
        //---------------------------------------------------------

        public static Point PointOnCircle(
            Point center,
            double radius,
            double angle)
        {
            double radians = angle * Math.PI / 180.0;

            return new Point(
                center.X + radius * Math.Cos(radians),
                center.Y + radius * Math.Sin(radians));
        }
    }
}

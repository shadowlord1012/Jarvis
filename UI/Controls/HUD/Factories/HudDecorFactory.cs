using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Models;
using Path = System.Windows.Shapes.Path;

namespace UI.Controls.HUD.Factories
{
    /// <summary>
    /// Creates decorative HUD elements.
    /// These are visual accents only and contain no business logic.
    /// </summary>
    public static class HudDecorFactory
    {
        #region Corner Brackets

        public static Canvas CreateCornerBrackets(
            double width,
            double height,
            HudTheme? theme = null,
            double length = 20)
        {
            Canvas canvas = new()
            {
                Width = width,
                Height = height,
                IsHitTestVisible = false
            };

            AddCorner(canvas, theme.PrimaryBrush, theme.BorderThickness, 0, 0, length, true, true);
            AddCorner(canvas, theme.PrimaryBrush, theme.BorderThickness, width, 0, length, false, true);
            AddCorner(canvas, theme.PrimaryBrush, theme.BorderThickness, 0, height, length, true, false);
            AddCorner(canvas, theme.PrimaryBrush, theme.BorderThickness, width, height, length, false, false);

            return canvas;
        }

        private static void AddCorner(
            Canvas canvas,
            Brush brush,
            double thickness,
            double x,
            double y,
            double length,
            bool left,
            bool top)
        {
            Line h = new()
            {
                Stroke = brush,
                StrokeThickness = thickness,
                X1 = x,
                Y1 = y,
                X2 = left ? x + length : x - length,
                Y2 = y
            };

            Line v = new()
            {
                Stroke = brush,
                StrokeThickness = thickness,
                X1 = x,
                Y1 = y,
                X2 = x,
                Y2 = top ? y + length : y - length
            };

            canvas.Children.Add(h);
            canvas.Children.Add(v);
        }
        public static Geometry CreateCorner(
            Point position,
            CornerOrientation orientation,
            HudTheme theme)
        {
            double length = theme.FrameCornerLength;

            StreamGeometry geometry = new();

            using (StreamGeometryContext context = geometry.Open())
            {
                switch (orientation)
                {
                    case CornerOrientation.TopLeft:

                        context.BeginFigure(
                            new Point(position.X + length, position.Y),
                            false,
                            false);

                        context.LineTo(
                            position,
                            true,
                            false);

                        context.LineTo(
                            new Point(position.X, position.Y + length),
                            true,
                            false);

                        break;

                    case CornerOrientation.TopRight:

                        context.BeginFigure(
                            new Point(position.X - length, position.Y),
                            false,
                            false);

                        context.LineTo(
                            position,
                            true,
                            false);

                        context.LineTo(
                            new Point(position.X, position.Y + length),
                            true,
                            false);

                        break;

                    case CornerOrientation.BottomRight:

                        context.BeginFigure(
                            new Point(position.X, position.Y - length),
                            false,
                            false);

                        context.LineTo(
                            position,
                            true,
                            false);

                        context.LineTo(
                            new Point(position.X - length, position.Y),
                            true,
                            false);

                        break;

                    case CornerOrientation.BottomLeft:

                        context.BeginFigure(
                            new Point(position.X, position.Y - length),
                            false,
                            false);

                        context.LineTo(
                            position,
                            true,
                            false);

                        context.LineTo(
                            new Point(position.X + length, position.Y),
                            true,
                            false);

                        break;

                    default:
                        throw new ArgumentOutOfRangeException(
                            nameof(orientation));
                }
            }

            geometry.Freeze();

            return geometry;
        }

        #endregion

        #region Divider

        public static Line CreateDivider(
            double width,
            HudTheme? theme = null)
        {
            return new Line
            {
                X1 = 0,
                X2 = width,
                Y1 = 0,
                Y2 = 0,
                Stroke = theme.PrimaryBrush,
                StrokeThickness = theme.BorderThickness,
                Opacity = 0.7
            };
        }

        #endregion

        #region Accent Bar

        public static Rectangle CreateAccentBar(
            double width,
            double height,
            HudTheme? theme = null)
        {
            return new Rectangle
            {
                Width = width,
                Height = height,
                RadiusX = 1,
                RadiusY = 1,
                Fill = theme.PrimaryBrush
            };
        }

        #endregion

        #region Grid Overlay

        public static Path CreateGridOverlay(
            double width,
            double height,
            double spacing,
            HudTheme? theme = null)
        {
            return new Path
            {
                Data = HudShapeFactory.Grid(width, height, spacing),
                Stroke = theme.PrimaryBrush,
                StrokeThickness = 0.5,
                Opacity = 0.18
            };
        }

        #endregion

        #region Hex Overlay

        public static Path CreateHex(
            double radius,
            HudTheme? theme = null)
        {
            return new Path
            {
                Data = HudShapeFactory.Hexagon(radius),
                Stroke = theme.PrimaryBrush,
                StrokeThickness = theme.BorderThickness,
                Fill = Brushes.Transparent
            };
        }

        #endregion

        #region Circular Ring

        public static Ellipse CreateRing(
            double diameter,
            HudTheme? theme = null)
        {
            return new Ellipse
            {
                Width = diameter,
                Height = diameter,
                Stroke = theme.PrimaryBrush,
                StrokeThickness = theme.BorderThickness,
                Fill = Brushes.Transparent
            };
        }

        #endregion

        #region Scan Lines

        public static Canvas CreateScanLines(
            double width,
            double height,
            HudTheme? theme = null,
            double spacing = 6)
        {
            Canvas canvas = new()
            {
                Width = width,
                Height = height,
                IsHitTestVisible = false
            };

            for (double y = 0; y <= height; y += spacing)
            {
                canvas.Children.Add(new Line
                {
                    X1 = 0,
                    X2 = width,
                    Y1 = y,
                    Y2 = y,
                    Stroke = theme.PrimaryBrush,
                    StrokeThickness = 0.4,
                    Opacity = 0.12
                });
            }

            return canvas;
        }

        #endregion

        #region Target Brackets

        public static Canvas CreateTargetBrackets(
            double size,
            HudTheme? theme = null)
        {
            return CreateCornerBrackets(
                size,
                size,
                theme,
                size * 0.18);
        }

        #endregion

        #region Dashed Frame

        public static Rectangle CreateDashedFrame(
            double width,
            double height,
            HudTheme? theme = null)
        {
            return new Rectangle
            {
                Width = width,
                Height = height,
                Stroke = theme.PrimaryBrush,
                StrokeThickness = 1,
                Fill = Brushes.Transparent,
                StrokeDashArray = new DoubleCollection
                {
                    4,
                    2
                }
            };
        }

        #endregion

        #region Crosshair

        public static Path CreateCrosshair(
            double radius,
            HudTheme? theme = null)
        {
            return new Path
            {
                Data = HudShapeFactory.Crosshair(radius, radius * 0.18),
                Stroke = theme.PrimaryBrush,
                StrokeThickness = theme.BorderThickness
            };
        }

        #endregion

        #region Arc

        public static Path CreateArc(
            Point center,
            double radius,
            double startAngle,
            double endAngle,
            HudTheme? theme = null)
        {
            return new Path
            {
                Data = HudShapeFactory.Arc(
                    center,
                    radius,
                    startAngle,
                    endAngle),
                Stroke = theme.PrimaryBrush,
                StrokeThickness = theme.BorderThickness
            };
        }

        #endregion
    }
}

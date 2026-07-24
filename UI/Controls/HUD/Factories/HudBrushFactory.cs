using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace UI.Controls.HUD.Factories
{
    public static class HudBrushFactory
    {
        //---------------------------------------------------------
        // HUD Colors
        //---------------------------------------------------------

        public static readonly Color Cyan =
            Color.FromRgb(0, 255, 255);

        public static readonly Color Blue =
            Color.FromRgb(40, 170, 255);

        public static readonly Color DarkBlue =
            Color.FromRgb(5, 15, 25);

        //---------------------------------------------------------
        // Solid Brushes
        //---------------------------------------------------------

        public static SolidColorBrush Solid(Color color)
        {
            var brush = new SolidColorBrush(color);

            brush.Freeze();

            return brush;
        }

        public static SolidColorBrush Cyanbrush()
            => Solid(Cyan);

        public static SolidColorBrush BlueBrush()
            => Solid(Blue);

        //---------------------------------------------------------
        // Glow
        //---------------------------------------------------------

        public static RadialGradientBrush Glow(Color color)
        {
            var brush = new RadialGradientBrush();

            brush.GradientStops.Add(
                new GradientStop(color, 0));

            brush.GradientStops.Add(
                new GradientStop(Colors.Transparent, 1));

            return brush;
        }
        public static RadialGradientBrush CreateGlow(
                Color color,
                double opacity)
        {
            RadialGradientBrush brush = new();

            brush.Center = new Point(.5, .5);

            brush.GradientOrigin = new Point(.5, .5);

            brush.RadiusX = .5;

            brush.RadiusY = .5;

            brush.GradientStops.Add(
                new GradientStop(
                    Color.FromArgb(
                        (byte)(255 * opacity),
                        color.R,
                        color.G,
                        color.B),
                    0));

            brush.GradientStops.Add(
                new GradientStop(
                    Colors.Transparent,
                    1));

            brush.Freeze();

            return brush;
        }

        //---------------------------------------------------------
        // Scan Brushes
        //---------------------------------------------------------

        public static LinearGradientBrush HorizontalScan(Color color)
        {
            return new LinearGradientBrush(
                Colors.Transparent,
                color,
                90);
        }

        public static LinearGradientBrush VerticalScan(Color color)
        {
            return new LinearGradientBrush(
                Colors.Transparent,
                color,
                0);
        }
    }
}

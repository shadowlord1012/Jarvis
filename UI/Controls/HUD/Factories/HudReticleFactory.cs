using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Factories
{
    /// <summary>
    /// Creates complete targeting reticles and tactical HUD graphics.
    /// </summary>
    public static class HudReticleFactory
    {
        #region Standard Reticle

        public static Canvas CreateStandardReticle(
            double size,
            HudTheme? theme = null)
        {
            Canvas canvas = CreateCanvas(size);

            // Outer ring
            AddCentered(
                canvas,
                HudDecorFactory.CreateRing(
                    size,
                    theme));

            // Crosshair
            AddCentered(
                canvas,
                HudDecorFactory.CreateCrosshair(
                    size / 2,
                    theme));

            // Corner brackets
            Add(
                canvas,
                HudDecorFactory.CreateTargetBrackets(
                    size,
                    theme
                    ),
                0,
                0);

            return canvas;
        }

        #endregion

        #region Radar Reticle

        public static Canvas CreateRadarReticle(
            double size,
            HudTheme? theme = null)
        {
            Canvas canvas = CreateCanvas(size);

            AddCentered(
                canvas,
                HudDecorFactory.CreateRing(size, theme));

            AddCentered(
                canvas,
                HudDecorFactory.CreateRing(
                    size * .66,
                    theme));

            AddCentered(
                canvas,
                HudDecorFactory.CreateRing(
                    size * .33,
                    theme));

            AddCentered(
                canvas,
                HudDecorFactory.CreateCrosshair(
                    size / 2,
                    theme));

            return canvas;
        }

        #endregion

        #region Lock-On Reticle

        public static Canvas CreateLockOnReticle(
            double size,
            HudTheme? theme)
        {
            Canvas canvas = CreateCanvas(size);

            AddCentered(
                canvas,
                HudDecorFactory.CreateRing(
                    size,
                    theme));

            Add(
                canvas,
                HudDecorFactory.CreateTargetBrackets(
                    size,
                    theme),
                0,
                0);

            double arcRadius = size / 2;

            Add(
                canvas,
                HudDecorFactory.CreateArc(
                    new Point(arcRadius, arcRadius),
                    arcRadius,
                    25,
                    65,
                    theme),
                0,
                0);

            Add(
                canvas,
                HudDecorFactory.CreateArc(
                    new Point(arcRadius, arcRadius),
                    arcRadius,
                    115,
                    155,
                    theme),
                0,
                0);

            Add(
                canvas,
                HudDecorFactory.CreateArc(
                    new Point(arcRadius, arcRadius),
                    arcRadius,
                    205,
                    245,
                    theme),
                0,
                0);

            Add(
                canvas,
                HudDecorFactory.CreateArc(
                    new Point(arcRadius, arcRadius),
                    arcRadius,
                    295,
                    335,
                    theme),
                0,
                0);

            return canvas;
        }

        #endregion

        #region Scanner Reticle

        public static Canvas CreateScannerReticle(
            double size,
            HudTheme? theme = null)
        {
            Canvas canvas = CreateCanvas(size);

            AddCentered(
                canvas,
                HudDecorFactory.CreateRing(
                    size,
                    theme));

            AddCentered(
                canvas,
                HudDecorFactory.CreateHex(
                    size / 2,
                    theme));

            AddCentered(
                canvas,
                HudDecorFactory.CreateCrosshair(
                    size / 2,
                    theme));

            return canvas;
        }

        #endregion

        #region Compass Reticle

        public static Canvas CreateCompassReticle(
            double size,
            HudTheme? theme = null)
        {
            Canvas canvas = CreateCanvas(size);

            AddCentered(
                canvas,
                HudDecorFactory.CreateRing(
                    size,
                    theme));

            AddCentered(
                canvas,
                HudDecorFactory.CreateCrosshair(
                    size / 2,
                    theme));

            Add(
                canvas,
                HudDecorFactory.CreateDivider(
                    size,
                    theme),
                0,
                size / 2);

            return canvas;
        }

        #endregion

        #region Target Box

        public static Canvas CreateTargetBox(
            double width,
            double height,
            HudTheme? theme = null)
        {
            Canvas canvas = new()
            {
                Width = width,
                Height = height,
                Background = Brushes.Transparent
            };

            Add(
                canvas,
                HudDecorFactory.CreateCornerBrackets(
                    width,
                    height,
                    theme),
                0,
                0);

            return canvas;
        }

        #endregion

        #region Helpers

        private static Canvas CreateCanvas(double size)
        {
            return new Canvas
            {
                Width = size,
                Height = size,
                Background = Brushes.Transparent,
                IsHitTestVisible = false
            };
        }

        private static void Add(
            Canvas canvas,
            UIElement element,
            double left,
            double top)
        {
            Canvas.SetLeft(element, left);
            Canvas.SetTop(element, top);
            canvas.Children.Add(element);
        }

        private static void AddCentered(
            Canvas canvas,
            FrameworkElement element)
        {
            double x = (canvas.Width - element.Width) / 2;
            double y = (canvas.Height - element.Height) / 2;

            Add(canvas, element, x, y);
        }

        #endregion
    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Factories
{
    /// <summary>
    /// Creates complete HUD overlay layers by composing
    /// panels, decorations, reticles, and other HUD elements.
    /// </summary>
    public static class HudOverlayFactory
    {
        #region Base Overlay

        public static Canvas CreateOverlay(
            double width,
            double height)
        {
            return new Canvas
            {
                Width = width,
                Height = height,
                Background = Brushes.Transparent,
                ClipToBounds = true,
                IsHitTestVisible = false
            };
        }

        #endregion

        #region Add Element

        public static void Add(
            Canvas overlay,
            UIElement element,
            double left,
            double top)
        {
            Canvas.SetLeft(element, left);
            Canvas.SetTop(element, top);

            overlay.Children.Add(element);
        }

        #endregion

        #region Add Centered

        public static void AddCentered(
            Canvas overlay,
            FrameworkElement element)
        {
            double x = (overlay.Width - element.Width) / 2;
            double y = (overlay.Height - element.Height) / 2;

            Add(overlay, element, x, y);
        }

        #endregion

        #region Dashboard Overlay

        public static Canvas CreateDashboardOverlay(
            double width,
            double height,
            HudTheme? theme = null)
        {
            Canvas overlay = CreateOverlay(width, height);

            // Decorative frame
            Add(
                overlay,
                HudDecorFactory.CreateCornerBrackets(
                    width,
                    height,
                    theme),
                0,
                0);

            // Background grid
            Add(
                overlay,
                HudDecorFactory.CreateGridOverlay(
                    width,
                    height,
                    50,
                    theme),
                0,
                0);

            return overlay;
        }

        #endregion

        #region Scanner Overlay

        public static Canvas CreateScannerOverlay(
            double width,
            double height,
            HudTheme? theme = null)
        {
            Canvas overlay = CreateOverlay(width, height);

            Add(
                overlay,
                HudDecorFactory.CreateScanLines(
                    width,
                    height,
                    theme),
                0,
                0);

            Add(
                overlay,
                HudDecorFactory.CreateGridOverlay(
                    width,
                    height,
                    40,
                    theme),
                0,
                0);

            return overlay;
        }

        #endregion

        #region Target Overlay

        public static Canvas CreateTargetOverlay(
            double width,
            double height,
            HudTheme? theme = null)
        {
            Canvas overlay = CreateOverlay(width, height);

            var reticle =
                HudReticleFactory.CreateStandardReticle(
                    140,
                    theme);

            AddCentered(
                overlay,
                reticle);

            return overlay;
        }

        #endregion

        #region Radar Overlay

        public static Canvas CreateRadarOverlay(
            double width,
            double height,
            HudTheme? theme = null)
        {
            Canvas overlay = CreateOverlay(width, height);

            var radar =
                HudReticleFactory.CreateRadarReticle(
                    220,
                    theme);

            AddCentered(
                overlay,
                radar);

            return overlay;
        }

        #endregion

        #region Tactical Overlay

        public static Canvas CreateTacticalOverlay(
            double width,
            double height,
            HudTheme? theme = null)
        {
            Canvas overlay =
                CreateDashboardOverlay(
                    width,
                    height,
                    theme);

            Add(
                overlay,
                HudDecorFactory.CreateScanLines(
                    width,
                    height,
                    theme),
                0,
                0);

            return overlay;
        }

        #endregion

        #region Clear

        public static void Clear(Canvas overlay)
        {
            overlay.Children.Clear();
        }

        #endregion
    }
}

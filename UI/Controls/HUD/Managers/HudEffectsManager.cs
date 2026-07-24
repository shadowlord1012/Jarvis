using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using UI.Controls.HUD.Models;
using UI.Controls.HUD.Renderers;

namespace UI.Controls.HUD.Managers
{
    /// <summary>
    /// Applies visual effects and animations to HUD elements.
    /// Does not create UI elements—it only decorates them.
    /// </summary>
    public sealed class HudEffectsManager
    {
        private static readonly Lazy<HudEffectsManager> _instance =
            new(() => new HudEffectsManager());

        public static HudEffectsManager Instance => _instance.Value;

        private HudEffectsManager()
        {
        }

        private HudTheme Theme =>
            HudThemeManager.Instance.CurrentTheme;

        #region Glow

        public DropShadowEffect CreateGlow()
        {
            return new DropShadowEffect
            {
                BlurRadius = Theme.GlowRadius,
                Color = GetColor(Theme.PrimaryBrush),
                ShadowDepth = 0,
                Opacity = Theme.GlowOpacity
            };
        }

        public void ApplyGlow(UIElement element)
        {
            element.Effect = CreateGlow();
        }

        #endregion

        #region Warning Glow

        public DropShadowEffect CreateWarningGlow()
        {
            return new DropShadowEffect
            {
                BlurRadius = Theme.GlowRadius + 6,
                ShadowDepth = 0,
                Color = GetColor(Theme.WarningBrush),
                Opacity = Theme.GlowOpacity
            };
        }

        public void ApplyWarningGlow(UIElement element)
        {
            element.Effect = CreateWarningGlow();
        }

        #endregion

        #region Danger Glow

        public DropShadowEffect CreateDangerGlow()
        {
            return new DropShadowEffect
            {
                BlurRadius = Theme.GlowRadius + 8,
                ShadowDepth = 0,
                Color = GetColor(Theme.DangerBrush),
                Opacity = Theme.GlowOpacity
            };
        }

        public void ApplyDangerGlow(UIElement element)
        {
            element.Effect = CreateDangerGlow();
        }

        #endregion

        #region Blur

        public BlurEffect CreateBlur(double radius = 3)
        {
            return new BlurEffect
            {
                Radius = radius
            };
        }

        public void ApplyBlur(UIElement element, double radius = 3)
        {
            element.Effect = CreateBlur(radius);
        }

        #endregion

        #region Opacity

        public void FadeTo(
            UIElement element,
            double opacity,
            double seconds)
        {
            DoubleAnimation animation = new()
            {
                To = opacity,
                Duration = TimeSpan.FromSeconds(seconds)
            };

            element.BeginAnimation(
                UIElement.OpacityProperty,
                animation);
        }

        #endregion

        #region Pulse

        public void ApplyPulse(UIElement element)
        {
            DoubleAnimation animation = new()
            {
                From = 0.45,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(Theme.PulseSpeed),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            element.BeginAnimation(
                UIElement.OpacityProperty,
                animation);
        }

        #endregion

        #region Rotate

        public void RotateForever(
            UIElement element,
            double seconds = 8)
        {
            RotateTransform transform = new();

            element.RenderTransform = transform;

            element.RenderTransformOrigin =
                new Point(0.5, 0.5);

            DoubleAnimation animation = new()
            {
                From = 0,
                To = 360,
                Duration = TimeSpan.FromSeconds(seconds),
                RepeatBehavior = RepeatBehavior.Forever
            };

            transform.BeginAnimation(
                RotateTransform.AngleProperty,
                animation);
        }

        #endregion

        #region Scale Pulse

        public void ApplyScalePulse(
            FrameworkElement element,
            double scale = 1.05)
        {
            ScaleTransform transform = new(1, 1);

            element.RenderTransform = transform;

            element.RenderTransformOrigin =
                new Point(.5, .5);

            DoubleAnimation animation = new()
            {
                From = 1,
                To = scale,
                Duration = TimeSpan.FromSeconds(
                    Theme.PulseSpeed),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            transform.BeginAnimation(
                ScaleTransform.ScaleXProperty,
                animation);

            transform.BeginAnimation(
                ScaleTransform.ScaleYProperty,
                animation);
        }

        #endregion

        #region Stop

        public void StopAnimations(UIElement element)
        {
            element.BeginAnimation(
                UIElement.OpacityProperty,
                null);

            if (element.RenderTransform is RotateTransform rotate)
            {
                rotate.BeginAnimation(
                    RotateTransform.AngleProperty,
                    null);
            }

            if (element.RenderTransform is ScaleTransform scale)
            {
                scale.BeginAnimation(
                    ScaleTransform.ScaleXProperty,
                    null);

                scale.BeginAnimation(
                    ScaleTransform.ScaleYProperty,
                    null);
            }
        }

        #endregion

        #region Helpers

        private static Color GetColor(Brush brush)
        {
            if (brush is SolidColorBrush solid)
                return solid.Color;

            return Colors.White;
        }

        #endregion


    }
}

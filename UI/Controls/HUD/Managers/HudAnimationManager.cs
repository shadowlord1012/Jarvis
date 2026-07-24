using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Managers
{
    /// <summary>
    /// Central animation manager for every HUD animation.
    /// No Storyboards should be created outside this class.
    /// </summary>
    public sealed class HudAnimationManager
    {
        private static readonly Lazy<HudAnimationManager> _instance =
            new(() => new HudAnimationManager());

        public static HudAnimationManager Instance => _instance.Value;

        private HudAnimationManager()
        {
        }

        private HudTheme Theme =>
            HudThemeManager.Instance.CurrentTheme;

        #region Fade

        public void FadeIn(
            UIElement element,
            double duration = -1)
        {
            AnimateOpacity(
                element,
                0,
                1,
                duration < 0
                    ? Theme.FadeSpeed
                    : duration);
        }

        public void FadeOut(
            UIElement element,
            double duration = -1)
        {
            AnimateOpacity(
                element,
                element.Opacity,
                0,
                duration < 0
                    ? Theme.FadeSpeed
                    : duration);
        }

        #endregion

        #region Pulse

        public void Pulse(UIElement element)
        {
            DoubleAnimation animation = new()
            {
                From = .45,
                To = 1,
                Duration = TimeSpan.FromSeconds(
                    Theme.PulseSpeed),

                AutoReverse = true,

                RepeatBehavior =
                    RepeatBehavior.Forever
            };

            element.BeginAnimation(
                UIElement.OpacityProperty,
                animation);
        }

        #endregion

        #region Rotate

        public void Rotate(
            UIElement element,
            double seconds = -1)
        {
            if (seconds < 0)
                seconds = 8;

            RotateTransform rotate = new();

            element.RenderTransform = rotate;

            element.RenderTransformOrigin =
                new Point(.5, .5);

            DoubleAnimation animation = new()
            {
                From = 0,
                To = 360,
                Duration =
                    TimeSpan.FromSeconds(seconds),

                RepeatBehavior =
                    RepeatBehavior.Forever
            };

            rotate.BeginAnimation(
                RotateTransform.AngleProperty,
                animation);
        }

        #endregion

        #region Scale Pulse

        public void ScalePulse(
            FrameworkElement element,
            double amount = 1.08)
        {
            ScaleTransform scale =
                new(1, 1);

            element.RenderTransform =
                scale;

            element.RenderTransformOrigin =
                new Point(.5, .5);

            DoubleAnimation animation = new()
            {
                From = 1,
                To = amount,

                Duration =
                    TimeSpan.FromSeconds(
                        Theme.PulseSpeed),

                AutoReverse = true,

                RepeatBehavior =
                    RepeatBehavior.Forever
            };

            scale.BeginAnimation(
                ScaleTransform.ScaleXProperty,
                animation);

            scale.BeginAnimation(
                ScaleTransform.ScaleYProperty,
                animation);
        }

        #endregion

        #region Scan Sweep

        public void ScanSweep(
            FrameworkElement element,
            double distance)
        {
            TranslateTransform translate = new();

            element.RenderTransform =
                translate;

            DoubleAnimation animation = new()
            {
                From = -distance,

                To = distance,

                Duration =
                    TimeSpan.FromSeconds(
                        Theme.ScanSpeed),

                RepeatBehavior =
                    RepeatBehavior.Forever
            };

            translate.BeginAnimation(
                TranslateTransform.YProperty,
                animation);
        }

        #endregion

        #region Blink

        public void Blink(
            UIElement element,
            double speed = .25)
        {
            DoubleAnimation animation = new()
            {
                From = 1,

                To = 0,

                Duration =
                    TimeSpan.FromSeconds(speed),

                AutoReverse = true,

                RepeatBehavior =
                    RepeatBehavior.Forever
            };

            element.BeginAnimation(
                UIElement.OpacityProperty,
                animation);
        }

        #endregion

        #region Progress

        public void AnimateProgress(
            FrameworkElement element,
            DependencyProperty property,
            double from,
            double to,
            double seconds)
        {
            DoubleAnimation animation = new()
            {
                From = from,

                To = to,

                Duration =
                    TimeSpan.FromSeconds(seconds)
            };

            element.BeginAnimation(
                property,
                animation);
        }

        #endregion

        #region Stop

        public void Stop(UIElement element)
        {
            element.BeginAnimation(
                UIElement.OpacityProperty,
                null);

            switch (element.RenderTransform)
            {
                case RotateTransform rotate:

                    rotate.BeginAnimation(
                        RotateTransform.AngleProperty,
                        null);

                    break;

                case ScaleTransform scale:

                    scale.BeginAnimation(
                        ScaleTransform.ScaleXProperty,
                        null);

                    scale.BeginAnimation(
                        ScaleTransform.ScaleYProperty,
                        null);

                    break;

                case TranslateTransform translate:

                    translate.BeginAnimation(
                        TranslateTransform.XProperty,
                        null);

                    translate.BeginAnimation(
                        TranslateTransform.YProperty,
                        null);

                    break;
            }
        }

        #endregion

        #region Helpers

        private static void AnimateOpacity(
            UIElement element,
            double from,
            double to,
            double seconds)
        {
            DoubleAnimation animation = new()
            {
                From = from,

                To = to,

                Duration =
                    TimeSpan.FromSeconds(seconds)
            };

            element.BeginAnimation(
                UIElement.OpacityProperty,
                animation);
        }

        #endregion
    }
}

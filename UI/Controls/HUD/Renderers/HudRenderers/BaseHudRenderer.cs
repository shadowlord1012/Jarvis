using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UI.Controls.HUD.Configurations;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Managers;
using UI.Controls.HUD.Models;
using Path = System.Windows.Shapes.Path;

namespace UI.Controls.HUD.Renderers.HudRenderers
{
    /// <summary>
    /// Base class for all HUD renderers.
    /// Provides common access to the HUD layer,
    /// theme, effects, animations and helper methods.
    /// </summary>
    public abstract class BaseHudRenderer :
        IHudRenderer,
        IDisposable
    {
        protected Canvas Canvas { get; }

        protected HudLayer Layer { get; }

        protected HudLayoutCalculator Layout { get; }
        protected HudLayerManager LayerManager { get; }
        protected HudTheme Theme =>
            HudThemeManager.Instance.CurrentTheme;

        protected HudEffectsManager Effects =>
            HudEffectsManager.Instance;

        protected HudAnimationManager Animations =>
            HudAnimationManager.Instance;

        protected double Width { get; private set; }

        protected double Height { get; private set; }

        protected Point Center =>
            new(
                Width / 2.0,
                Height / 2.0);

        //--------------------------------------------------

        protected BaseHudRenderer(
            HudLayerManager layers,
            HudLayer layer)
        {
            LayerManager = layers;

            Layer = layer;

            Canvas =
                layers.GetLayer(layer);

            Layout =
                new HudLayoutCalculator();

            HudThemeManager.Instance.ThemeChanged +=
                ThemeChanged;
        }

        //--------------------------------------------------

        public virtual void Initialize(
            double width,
            double height)
        {
            Width = width;
            Height = height;

            Canvas.Children.Clear();

            Build();
        }

        //--------------------------------------------------

        public virtual void Resize(
            double width,
            double height)
        {
            Initialize(width, height);
        }

        //--------------------------------------------------

        public abstract void Update(
            double deltaTime);

        //--------------------------------------------------

        protected abstract void Build();

        //--------------------------------------------------

        protected virtual void OnThemeChanged(
            HudTheme theme)
        {
            Initialize(
                Width,
                Height);
        }

        //--------------------------------------------------
        // Canvas Helpers
        //--------------------------------------------------

        protected void ClearCanvas()
        {
            Canvas.Children.Clear();
        }

        protected void Add(UIElement element)
        {
            Canvas.Children.Add(element);
        }

        protected void Add(
            UIElement element,
            double left,
            double top)
        {
            Canvas.SetLeft(
                element,
                left);

            Canvas.SetTop(
                element,
                top);

            Canvas.Children.Add(
                element);
        }

        //--------------------------------------------------
        // Theme Brushes
        //--------------------------------------------------

        protected Brush PrimaryBrush =>
            Theme.PrimaryBrush;

        protected Brush SecondaryBrush =>
            Theme.SecondaryBrush;

        protected Brush AccentBrush =>
            Theme.AccentBrush;

        protected Brush WarningBrush =>
            Theme.WarningBrush;

        protected Brush DangerBrush =>
            Theme.DangerBrush;

        protected Brush TextBrush =>
            Theme.TextBrush;

        //--------------------------------------------------
        // Shape Helpers
        //--------------------------------------------------

        protected Path CreatePath(
            Geometry geometry,
            double thickness,
            double opacity = 1.0)
        {
            return new Path
            {
                Data = geometry,
                Stroke = PrimaryBrush,
                StrokeThickness = thickness,
                Fill = Brushes.Transparent,
                Opacity = opacity,
                IsHitTestVisible = false
            };
        }

        protected Ellipse CreateRing(
            double radius,
            double thickness,
            double opacity = 1.0)
        {
            return new Ellipse
            {
                Width = radius * 2,
                Height = radius * 2,
                Stroke = PrimaryBrush,
                StrokeThickness = thickness,
                Fill = Brushes.Transparent,
                Opacity = opacity,
                IsHitTestVisible = false
            };
        }

        protected Ellipse CreateOrb(
            double radius,
            double opacity = 1.0)
        {
            return new Ellipse
            {
                Width = radius * 2,
                Height = radius * 2,
                Fill = PrimaryBrush,
                Opacity = opacity,
                IsHitTestVisible = false
            };
        }

        protected RotateTransform CreateRotation()
        {
            return new RotateTransform(
                0,
                Center.X,
                Center.Y);
        }

        //--------------------------------------------------
        // Effects
        //--------------------------------------------------

        protected void ApplyGlow(
            UIElement element)
        {
            Effects.ApplyGlow(element);
        }

        protected void ApplyWarningGlow(
            UIElement element)
        {
            Effects.ApplyWarningGlow(element);
        }

        protected void ApplyDangerGlow(
            UIElement element)
        {
            Effects.ApplyDangerGlow(element);
        }

        //--------------------------------------------------
        // Animations
        //--------------------------------------------------

        protected void Pulse(
            UIElement element)
        {
            Animations.Pulse(element);
        }

        protected void Rotate(
            UIElement element,
            double speed = 8.0)
        {
            Animations.Rotate(
                element,
                speed);
        }

        protected void Blink(
            UIElement element)
        {
            Animations.Blink(element);
        }

        //--------------------------------------------------

        private void ThemeChanged(
            object? sender,
            HudThemeChangedEventArgs e)
        {
            OnThemeChanged(
                e.NewTheme);
        }

        //--------------------------------------------------

        public virtual void Dispose()
        {
            HudThemeManager.Instance.ThemeChanged -=
                ThemeChanged;

            Canvas.Children.Clear();
        }
    }
}

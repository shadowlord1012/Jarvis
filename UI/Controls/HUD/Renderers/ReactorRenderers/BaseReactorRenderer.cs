using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UI.Controls.HUD.Configurations;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Managers;
using UI.Controls.HUD.Models;
using UI.Controls.HUD.Renderers.HudRenderers;

namespace UI.Controls.HUD.Renderers.ReactorRenderers
{
    public abstract class BaseReactorRenderer
    : BaseHudRenderer
    {
        protected ReactorManager Reactor { get; }

        protected ReactorState State => Reactor.State;

        protected BaseReactorRenderer(
            HudLayerManager layers,
            ReactorManager reactor)
            : base(layers, HudLayer.Reactor)
        {
            Reactor = reactor;
        }
    }
    /*
    public abstract class BaseReactorRenderer :
        IHudRenderer,
        IDisposable
    {
        protected Canvas Canvas { get; }

        protected ReactorManager Reactor { get; }

        protected ReactorState State => Reactor.State;

        protected HudTheme Theme =>
            HudThemeManager.Instance.CurrentTheme;

        protected HudEffectsManager Effects =>
            HudEffectsManager.Instance;

        protected HudAnimationManager Animations =>
            HudAnimationManager.Instance;

        protected HudLayoutCalculator Layout { get; }

        protected double Width { get; private set; }

        protected double Height { get; private set; }

        protected Point Center =>
            new(
                Width / 2,
                Height / 2);

        protected BaseReactorRenderer(
            HudLayerManager layerManager,
            ReactorManager reactor)
        {
            Canvas =
                layerManager.GetLayer(
                    HudLayer.Reactor);

            Reactor = reactor;

            Layout =
                new HudLayoutCalculator();

            HudThemeManager.Instance.ThemeChanged +=
                ThemeChanged;
        }

        //-------------------------------------------------

        public virtual void Initialize(
            double width,
            double height)
        {
            Width = width;
            Height = height;

            State.Center = Center;

            Canvas.Children.Clear();

            Build();
        }

        //-------------------------------------------------

        public virtual void Resize(
            double width,
            double height)
        {
            Initialize(width, height);
        }

        //-------------------------------------------------

        public abstract void Update(
            double deltaTime);

        //-------------------------------------------------

        protected abstract void Build();

        //-------------------------------------------------

        protected virtual void OnThemeChanged(
            HudTheme theme)
        {
            Initialize(
                Width,
                Height);
        }

        //-------------------------------------------------

        protected virtual void ClearCanvas()
        {
            Canvas.Children.Clear();
        }

        //-------------------------------------------------

        protected void Add(
            UIElement element)
        {
            Canvas.Children.Add(element);
        }

        //-------------------------------------------------

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

        //-------------------------------------------------

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

        //-------------------------------------------------

        protected void ApplyGlow(
            UIElement element)
        {
            Effects.ApplyGlow(
                element);
        }

        protected void ApplyWarningGlow(
            UIElement element)
        {
            Effects.ApplyWarningGlow(
                element);
        }

        protected void ApplyDangerGlow(
            UIElement element)
        {
            Effects.ApplyDangerGlow(
                element);
        }

        //-------------------------------------------------

        protected void Pulse(
            UIElement element)
        {
            Animations.Pulse(
                element);
        }

        protected void Rotate(
            UIElement element,
            double speed = 8)
        {
            Animations.Rotate(
                element,
                speed);
        }

        protected void Blink(
            UIElement element)
        {
            Animations.Blink(
                element);
        }

        //-------------------------------------------------

        private void ThemeChanged(
            object? sender,
            HudThemeChangedEventArgs e)
        {
            OnThemeChanged(
                e.NewTheme);
        }

        //-------------------------------------------------

        public virtual void Dispose()
        {
            HudThemeManager.Instance.ThemeChanged -=
                ThemeChanged;

            Canvas.Children.Clear();
        }
        protected Path CreatePath(
            Geometry geometry,
            Brush stroke,
            double thickness,
            Brush? fill = null,
            double opacity = 1.0)
        {
            return new Path
            {
                Data = geometry,
                Stroke = stroke,
                StrokeThickness = thickness,
                Fill = fill ?? Brushes.Transparent,
                Opacity = opacity
            };
        }
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
                Opacity = opacity
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
                Opacity = opacity
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
                Opacity = opacity
            };
        }
        protected Geometry CreateBeamGeometry(
            Point start,
            Point end)
        {
            PathFigure figure = new()
            {
                StartPoint = start
            };

            figure.Segments.Add(
                new LineSegment(end, true));

            return new PathGeometry(new[]
            {
        figure
    });
        }
        protected RotateTransform CreateRotation()
        {
            return new RotateTransform(
                0,
                Center.X,
                Center.Y);
        }

        protected void Rotate(
            RotateTransform transform,
            ref double angle,
            double speed,
            double deltaTime)
        {
            angle += speed * deltaTime;
            transform.Angle = angle;
        }

        protected Path AddPath(
            Geometry geometry,
            double thickness,
            double opacity,
            RotateTransform? rotation = null,
            bool glow = false)
        {
            Path path = CreatePath(
                geometry,
                thickness,
                opacity);

            if (rotation != null)
                path.RenderTransform = rotation;

            if (glow)
                ApplyGlow(path);

            Add(path);

            return path;
        }
    }
    */
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Factories;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Managers;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Renderers.HudRenderers
{
    /// <summary>
    /// Renders the animated HUD background.
    /// </summary>
    public sealed class BackgroundRenderer : BaseHudRenderer
    {
        private Rectangle? _background;

        private LinearGradientBrush? _brush;

        private double _offset;

        //----------------------------------------------------

        public BackgroundRenderer(
            HudLayerManager layerManager)
            : base(
                layerManager,
                HudLayer.Background)
        {
        }

        //----------------------------------------------------
        // Build
        //----------------------------------------------------

        protected override void Build()
        {
            _offset = 0;

            _brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1)
            };

            _brush.GradientStops.Add(
                new GradientStop(
                    Theme.BackgroundGradientStart,
                    0));

            _brush.GradientStops.Add(
                new GradientStop(
                    Theme.BackgroundGradientMiddle,
                    0.50));

            _brush.GradientStops.Add(
                new GradientStop(
                    Theme.BackgroundGradientEnd,
                    1.0));

            _background = new Rectangle
            {
                Width = Width,
                Height = Height,
                Fill = _brush,
                IsHitTestVisible = false
            };

            Add(_background);
        }

        //----------------------------------------------------
        // Update
        //----------------------------------------------------

        public override void Update(
            double deltaTime)
        {
            if (_brush == null)
                return;

            _offset +=
                deltaTime *
                Theme.BackgroundAnimationSpeed;

            _brush.StartPoint =
                new Point(
                    Theme.BackgroundAnimationAmplitude *
                    Math.Sin(_offset),
                    0);

            _brush.EndPoint =
                new Point(
                    1,
                    1 +
                    Theme.BackgroundAnimationAmplitude *
                    Math.Cos(_offset));
        }

        //----------------------------------------------------
        // Resize
        //----------------------------------------------------

        public override void Resize(
            double width,
            double height)
        {
            base.Resize(width, height);

            if (_background != null)
            {
                _background.Width = width;
                _background.Height = height;
            }
        }

        //----------------------------------------------------
        // Theme Changed
        //----------------------------------------------------

        protected override void OnThemeChanged(
            HudTheme theme)
        {
            base.OnThemeChanged(theme);

            _offset = 0;
        }

        //----------------------------------------------------

        public override void Dispose()
        {
            _background = null;
            _brush = null;

            base.Dispose();
        }
    }
    /*
    public class BackgroundRenderer : IHudRenderer
    {
        private readonly Canvas _canvas;

        private Rectangle? _background;

        private LinearGradientBrush? _brush;

        private double _offset;

        public BackgroundRenderer(HudLayerManager layers)
        {
            _canvas = layers.GetLayer(HudLayer.Background);
        }

        public void Initialize(double width, double height)
        {
            Build(width, height);
        }

        private void Build(double width, double height)
        {
            _canvas.Children.Clear();

            _brush = new LinearGradientBrush();

            _brush.StartPoint = new System.Windows.Point(0, 0);

            _brush.EndPoint = new System.Windows.Point(1, 1);

            _brush.GradientStops.Add(
                new GradientStop(Color.FromRgb(3, 8, 18), 0));

            _brush.GradientStops.Add(
                new GradientStop(Color.FromRgb(0, 15, 35), .50));

            _brush.GradientStops.Add(
                new GradientStop(Color.FromRgb(2, 5, 12), 1));

            _background = HudShapeFactory.Rectangle(
                width,
                height,
                _brush);

            _canvas.Children.Add(_background);
        }

        public void Update(double deltaTime)
        {
            if (_brush == null)
                return;

            _offset += deltaTime * .03;

            _brush.StartPoint =
                new System.Windows.Point(
                    .05 * System.Math.Sin(_offset),
                    0);

            _brush.EndPoint =
                new System.Windows.Point(
                    1,
                    1 + (.05 * System.Math.Cos(_offset)));
        }

        public void Resize(double width, double height)
        {
            if (_background == null)
                return;

            _background.Width = width;
            _background.Height = height;
        }

        public void Dispose()
        {
            _canvas.Children.Clear();
        }
    }
    */
}

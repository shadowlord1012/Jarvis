using System;
using System.Collections.Generic;
using System.Text;
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
    /// Renders the animated HUD grid.
    /// </summary>
    public sealed class GridRenderer : BaseHudRenderer
    {
        private readonly List<Line> _verticalLines = new();

        private readonly List<Line> _horizontalLines = new();

        private readonly TranslateTransform _transform =
            new();

        private double _offset;

        //----------------------------------------------------

        public GridRenderer(
            HudLayerManager layerManager)
            : base(
                layerManager,
                HudLayer.Grid)
        {
        }

        //----------------------------------------------------

        protected override void Build()
        {
            _verticalLines.Clear();
            _horizontalLines.Clear();

            Canvas.RenderTransform = _transform;

            //--------------------------------------------------
            // Vertical
            //--------------------------------------------------

            for (double x = 0;
                 x <= Width;
                 x += Theme.GridSpacing)
            {
                Line line = new()
                {
                    X1 = x,
                    Y1 = 0,
                    X2 = x,
                    Y2 = Height,

                    Stroke = Theme.GridBrush,
                    StrokeThickness = Theme.GridThickness,
                    Opacity = Theme.GridOpacity,

                    IsHitTestVisible = false
                };

                _verticalLines.Add(line);

                Add(line);
            }

            //--------------------------------------------------
            // Horizontal
            //--------------------------------------------------

            for (double y = 0;
                 y <= Height;
                 y += Theme.GridSpacing)
            {
                Line line = new()
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = Width,
                    Y2 = y,

                    Stroke = Theme.GridBrush,
                    StrokeThickness = Theme.GridThickness,
                    Opacity = Theme.GridOpacity,

                    IsHitTestVisible = false
                };

                _horizontalLines.Add(line);

                Add(line);
            }
        }

        //----------------------------------------------------

        public override void Update(
            double deltaTime)
        {
            _offset +=
                deltaTime *
                Theme.GridScrollSpeed;

            if (_offset >= Theme.GridSpacing)
                _offset = 0;

            _transform.X = _offset;
            _transform.Y = _offset;
        }

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
            _verticalLines.Clear();

            _horizontalLines.Clear();

            base.Dispose();
        }
    }
    /*
    public class GridRenderer : IHudRenderer
    {
        private readonly Canvas _canvas;
        private readonly HudTheme _theme;

        private readonly List<Line> _verticalLines = new();
        private readonly List<Line> _horizontalLines = new();


        public Brush GridBrush { get; set; } =
            new SolidColorBrush(Color.FromRgb(0, 255, 255));


        public GridRenderer(
                HudLayerManager layers,
                HudTheme theme)
        {
            _canvas = layers.GetLayer(HudLayer.Grid);

            _theme = theme;
        }

        public void Build(double width, double height)
        {
            _canvas.Children.Clear();

            _verticalLines.Clear();
            _horizontalLines.Clear();

            //--------------------------------------------------
            // Vertical Lines
            //--------------------------------------------------

            for (double x = 0; x <= width; x += _theme.GridSpacing)
            {
                var line = HudShapeFactory.Line(
                        x,
                        0,
                        x,
                        height,
                        GridBrush,
                        _theme.GridThickness,
                        _theme.GridOpacity);

                _verticalLines.Add(line);
                _canvas.Children.Add(line);
            }

            //--------------------------------------------------
            // Horizontal Lines
            //--------------------------------------------------

            for (double y = 0; y <= height; y += _theme.GridSpacing)
            {
                var line = HudShapeFactory.Line(
                    0,
                    y,
                    width,
                    y,
                    GridBrush,
                    _theme.GridThickness,
                    _theme.GridOpacity);
            }
        }

        public void Update(double offset)
        {
            //--------------------------------------------------
            // Move vertical grid
            //--------------------------------------------------

            foreach (var line in _verticalLines)
            {
                line.X1 += offset;
                line.X2 += offset;

                if (line.X1 > _canvas.ActualWidth)
                {
                    line.X1 = 0;
                    line.X2 = 0;
                }
            }

            //--------------------------------------------------
            // Move horizontal grid
            //--------------------------------------------------

            foreach (var line in _horizontalLines)
            {
                line.Y1 += offset;
                line.Y2 += offset;

                if (line.Y1 > _canvas.ActualHeight)
                {
                    line.Y1 = 0;
                    line.Y2 = 0;
                }
            }
        }

        void IHudRenderer.Initialize(double width, double height)
        {
            Build(width, height);
        }


        void IHudRenderer.Resize(double width, double height)
        {
            Build(width, height);
        }

        public void Dispose()
        {
            _canvas.Children.Clear();

            _verticalLines.Clear();

            _horizontalLines.Clear();
        }
    }
    */
}

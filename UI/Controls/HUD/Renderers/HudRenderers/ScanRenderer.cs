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
    /// Renders the animated HUD scan effect.
    /// </summary>
    public sealed class ScanRenderer : BaseHudRenderer
    {
        private sealed class ScanVisual
        {
            public Rectangle Rectangle { get; init; } = null!;

            public TranslateTransform Transform { get; init; } = null!;

            public ScanDefinition Definition { get; init; } = null!;

            public double Position;
        }

        private readonly List<ScanVisual> _scanVisuals =
            new();

        //----------------------------------------------------

        public ScanRenderer(
            HudLayerManager layerManager)
            : base(
                layerManager,
                HudLayer.Effects)
        {
        }

        //----------------------------------------------------

        protected override void Build()
        {

            _scanVisuals.Clear();

            foreach (ScanDefinition effect in Theme.ScanEffects)
            {
                CreateScan(effect);
            }
        }

        //----------------------------------------------------

        private void CreateScan(
    ScanDefinition definition)
        {
            TranslateTransform transform =
                new();

            Rectangle rectangle;

            switch (definition.Type)
            {
                case ScanType.Horizontal:

                    rectangle = new Rectangle
                    {
                        Width = Width,
                        Height = definition.Thickness,
                        Fill = definition.Brush,
                        Opacity = definition.Opacity,
                        RenderTransform = transform
                    };

                    break;

                case ScanType.Vertical:

                    rectangle = new Rectangle
                    {
                        Width = definition.Thickness,
                        Height = Height,
                        Fill = definition.Brush,
                        Opacity = definition.Opacity,
                        RenderTransform = transform
                    };

                    break;

                default:

                    return;
            }

            if (definition.Glow)
                ApplyGlow(rectangle);

            Add(rectangle);

            _scanVisuals.Add(
                new ScanVisual
                {
                    Rectangle = rectangle,
                    Transform = transform,
                    Definition = definition
                });
        }

       

        //----------------------------------------------------

        public override void Update(
            double deltaTime)
        {
            foreach (ScanVisual scan in _scanVisuals)
            {
                scan.Position +=
                    scan.Definition.Speed *
                    deltaTime;

                switch (scan.Definition.Type)
                {
                    case ScanType.Horizontal:

                        if (scan.Position > Height)
                            scan.Position =
                                -scan.Definition.Thickness;

                        scan.Transform.Y =
                            scan.Position;

                        break;

                    case ScanType.Vertical:

                        if (scan.Position > Width)
                            scan.Position =
                                -scan.Definition.Thickness;

                        scan.Transform.X =
                            scan.Position;

                        break;
                }
            }
        }

        //----------------------------------------------------

        protected override void OnThemeChanged(
            HudTheme theme)
        {
            base.OnThemeChanged(theme);

        }

        //----------------------------------------------------

        public override void Dispose()
        {
            _scanVisuals.Clear();
            base.Dispose();
        }
    }
    /*
    public class ScanRenderer : IHudRenderer
    {
        private readonly Canvas _canvas;

        private readonly HudTheme _theme;

        private Rectangle? _scanLine;

        private double _width;
        private double _height;

        private double _position;

        public ScanType ScanType { get; set; } = ScanType.Horizontal;


        public Color ScanColor { get; set; } =
            Color.FromRgb(0, 255, 255);

        public ScanRenderer(HudLayerManager layers, HudTheme theme)
        {
            _canvas = layers.GetLayer(HudLayer.Effects);
            _theme = theme;
        }

        //----------------------------------------------------
        // Initialize
        //----------------------------------------------------

        public void Initialize(double width, double height)
        {
            _width = width;
            _height = height;

            Build();
        }

        //----------------------------------------------------
        // Build
        //----------------------------------------------------

        private void Build()
        {
            _canvas.Children.Clear();

            switch (ScanType)
            {
                case ScanType.Horizontal:

                    _scanLine = HudShapeFactory.Rectangle( 
                        _width,
                        _theme.ScanThickness,
                        HudBrushFactory.HorizontalScan(ScanColor),                        
                        .45
                    );

                    break;

                case ScanType.Vertical:

                    _scanLine = HudShapeFactory.Rectangle(
                        _theme.ScanThickness,
                        _height,
                        HudBrushFactory.VerticalScan(ScanColor),
                        .45
                    );

                    break;
            }

            if (_scanLine != null)
                _canvas.Children.Add(_scanLine);
        }

        //----------------------------------------------------
        // Update
        //----------------------------------------------------

        public void Update(double deltaTime)
        {
            if (_scanLine == null)
                return;

            _position +=
                _theme.ScanSpeed *
                            deltaTime;

            switch (ScanType)
            {
                case ScanType.Horizontal:

                    Canvas.SetTop(_scanLine, _position);

                    if (_position > _height)
                        _position = -_theme.GridThickness;

                    break;

                case ScanType.Vertical:

                    Canvas.SetLeft(_scanLine, _position);

                    if (_position > _width)
                        _position = -_theme.GridThickness;

                    break;
            }
        }

        //----------------------------------------------------
        // Resize
        //----------------------------------------------------

        public void Resize(double width, double height)
        {
            _width = width;
            _height = height;

            Build();
        }

        //----------------------------------------------------
        // Dispose
        //----------------------------------------------------

        public void Dispose()
        {
            _canvas.Children.Clear();
        }
    }
    */
}

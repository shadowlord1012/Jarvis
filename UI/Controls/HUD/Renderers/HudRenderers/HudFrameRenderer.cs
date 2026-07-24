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
using Path = System.Windows.Shapes.Path;

namespace UI.Controls.HUD.Renderers.HudRenderers
{
    /// <summary>
    /// Renders the main HUD frame.
    /// </summary>
    public sealed class HudFrameRenderer : BaseHudRenderer
    {
        private readonly List<Path> _corners = new();

        private readonly List<Path> _edges = new();

        //-----------------------------------------------------

        public HudFrameRenderer(
            HudLayerManager layerManager)
            : base(
                layerManager,
                HudLayer.Overlay)
        {
        }

        //-----------------------------------------------------

        protected override void Build()
        {
            _corners.Clear();
            _edges.Clear();

            Rect frame =
                new Rect(
                    Theme.FrameMargin,
                    Theme.FrameMargin,
                    Width - Theme.FrameMargin * 2,
                    Height - Theme.FrameMargin * 2);

            CreateFrame(frame);
            CreateCorners(frame);
        }

        //-----------------------------------------------------
        // Frame
        //-----------------------------------------------------

        private void CreateFrame(
            Rect frame)
        {
            AddEdge(
                new Point(frame.Left, frame.Top),
                new Point(frame.Right, frame.Top));

            AddEdge(
                new Point(frame.Right, frame.Top),
                new Point(frame.Right, frame.Bottom));

            AddEdge(
                new Point(frame.Right, frame.Bottom),
                new Point(frame.Left, frame.Bottom));

            AddEdge(
                new Point(frame.Left, frame.Bottom),
                new Point(frame.Left, frame.Top));
        }

        //-----------------------------------------------------
        // Corners
        //-----------------------------------------------------

        private void CreateCorners(
            Rect frame)
        {
            AddCorner(
                frame.TopLeft,
                CornerOrientation.TopLeft);

            AddCorner(
                frame.TopRight,
                CornerOrientation.TopRight);

            AddCorner(
                frame.BottomRight,
                CornerOrientation.BottomRight);

            AddCorner(
                frame.BottomLeft,
                CornerOrientation.BottomLeft);
        }

        //-----------------------------------------------------

        private void AddEdge(
            Point start,
            Point end)
        {
            Path edge =
                CreatePath(
                    HudShapeFactory.CreateLine(
                        start,
                        end),
                    Theme.FrameThickness,
                    Theme.FrameOpacity);

            edge.Stroke =
                Theme.PrimaryBrush;

            ApplyGlow(edge);

            Add(edge);

            _edges.Add(edge);
        }

        //-----------------------------------------------------

        private void AddCorner(
            Point position,
            CornerOrientation orientation)
        {
            Path corner =
                CreatePath(
                    HudDecorFactory.CreateCorner(
                        position,
                        orientation,
                        Theme),
                    Theme.FrameThickness,
                    1.0);

            corner.Stroke =
                Theme.PrimaryBrush;

            ApplyGlow(corner);

            Add(corner);

            _corners.Add(corner);
        }

        //-----------------------------------------------------

        public override void Update(
            double deltaTime)
        {
            // Reserved for future frame animations
        }

        //-----------------------------------------------------

        protected override void OnThemeChanged(
            HudTheme theme)
        {
            base.OnThemeChanged(theme);
        }

        //-----------------------------------------------------

        public override void Dispose()
        {
            _corners.Clear();
            _edges.Clear();

            base.Dispose();
        }
    }

    /*
    public sealed class HudFrameRenderer : IHudRenderer
    {
        private readonly Canvas _canvas;
        private readonly HudTheme _theme;

        private Rectangle? _frame;


        public HudFrameRenderer(
            HudLayerManager layers,
            HudTheme theme)
        {
            _canvas =
                layers.GetLayer(HudLayer.Overlay);

            _theme = theme;
        }


        public void Initialize(
            double width,
            double height)
        {
            _canvas.Children.Clear();


            _frame =
                HudShapeFactory.Rectangle(
                    width - 80,
                    height - 80,
                    Brushes.Transparent);


            _frame.Stroke =
                HudBrushFactory.Solid(
                    _theme.PrimaryColor);


            _frame.StrokeThickness = 1;

            _frame.Opacity = .35;


            Canvas.SetLeft(
                _frame,
                40);

            Canvas.SetTop(
                _frame,
                40);


            _canvas.Children.Add(
                _frame);
        }



        public void Update(
            double deltaTime)
        {

        }



        public void Resize(
            double width,
            double height)
        {
            Initialize(
                width,
                height);
        }



        public void Dispose()
        {
            _canvas.Children.Clear();
        }
    }
    */
}

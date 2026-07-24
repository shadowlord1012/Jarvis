using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Factories;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Managers;
using UI.Controls.HUD.Models;
using UI.Controls.HUD.Renderers.HudRenderers;

namespace UI.Controls.HUD.Renderers.ReactorRenderers
{

    /// <summary>
    /// Composite renderer responsible for managing
    /// all reactor-specific renderers.
    /// </summary>
    public sealed class ReactorRenderer : BaseHudRenderer
    {
        private readonly ReactorManager _reactor;

        private readonly List<BaseReactorRenderer> _renderers =
            new();


        //----------------------------------------------------

        public ReactorRenderer(
            HudLayerManager layerManager,
            ReactorManager reactor)
            : base(
                layerManager,
                HudLayer.Reactor)
        {
            _reactor = reactor;

            BuildPipeline();
        }

        //----------------------------------------------------

        private void BuildPipeline()
        {
            _renderers.Clear();

            _renderers.Add(
                new ReactorOuterRingRenderer(
                    LayerManager,
                    _reactor));

            _renderers.Add(
                new ReactorSegmentRenderer(
                    LayerManager,
                    _reactor));

            _renderers.Add(
                new ReactorTickRenderer(
                    LayerManager,
                    _reactor));

            _renderers.Add(
                new ReactorOrbitRenderer(
                    LayerManager,
                    _reactor));

            _renderers.Add(
                new ReactorEnergyBeamRenderer(
                    LayerManager,
                    _reactor));

            _renderers.Add(
                new ReactorCoreRenderer(
                    LayerManager,
                    _reactor));

        }

        //----------------------------------------------------

        protected override void Build()
        {
            foreach (BaseReactorRenderer renderer in _renderers)
            {
                renderer.Initialize(
                    Width,
                    Height);
            }
        }

        //----------------------------------------------------

        public override void Update(
            double deltaTime)
        {
            foreach (BaseReactorRenderer renderer in _renderers)
            {
                renderer.Update(deltaTime);
            }
        }

        //----------------------------------------------------

        public override void Resize(
            double width,
            double height)
        {
            base.Resize(width, height);

            foreach (BaseReactorRenderer renderer in _renderers)
            {
                renderer.Resize(width, height);
            }
        }

        //----------------------------------------------------

        protected override void OnThemeChanged(
            HudTheme theme)
        {
            base.OnThemeChanged(theme);

            foreach (BaseReactorRenderer renderer in _renderers)
            {
                renderer.Initialize(
                    Width,
                    Height);
            }
        }

        //----------------------------------------------------

        public override void Dispose()
        {
            foreach (BaseReactorRenderer renderer in _renderers)
            {
                renderer.Dispose();
            }

            _renderers.Clear();

            base.Dispose();
        }
    }

    /*
    public sealed class ReactorRenderer : IHudRenderer
    {
        private readonly Canvas _canvas;

        private readonly HudTheme _theme;

        private readonly ReactorState _state;
        private readonly HudEffects _effects;

        private Ellipse? _outerRing;

        private Ellipse? _innerRing;

        private Ellipse? _core;

        private Line? _line;

        private RotateTransform? _outerRotation;


        private double _time;


        private double _centerX;

        private double _centerY;


        public ReactorRenderer(
            HudLayerManager layers,
            HudTheme theme,
            ReactorManager reactor, 
            HudEffectsManager effects)
        {
            _canvas =
                layers.GetLayer(HudLayer.Reactor);

            _theme = theme;
            _state = reactor.State;
            _effects = effects.Effects;
        }


        //--------------------------------------------------
        // Initialize
        //--------------------------------------------------

        public void Initialize(
            double width,
            double height)
        {
            _canvas.Children.Clear();

            _centerX = width / 2;

            _centerY = height / 2;
            _state.Center =
                new Point(
                    width / 2,
                    height / 2);

            CreateReactor();
        }



        //--------------------------------------------------
        // Create Reactor
        //--------------------------------------------------

        private void CreateReactor()
        {
            //--------------------------------------------------
            // Outer Ring
            //--------------------------------------------------

            _outerRing =
                HudShapeFactory.Ellipse(
                    _theme.ReactorRadius * 2,
                    _theme.ReactorRadius * 2,
                    Brushes.Transparent);


            _outerRing.Stroke =
                HudBrushFactory.Solid(
                    _theme.PrimaryColor);


            _outerRing.StrokeThickness =
                _theme.ReactorRingThickness;


            _outerRing.Opacity =
                .7;



            Canvas.SetLeft(
                _outerRing,
                _centerX -
                _theme.ReactorRadius);


            Canvas.SetTop(
                _outerRing,
                _centerY -
                _theme.ReactorRadius);



            //--------------------------------------------------
            // Rotation
            //--------------------------------------------------

            _outerRotation =
                new RotateTransform();


            _outerRing.RenderTransform =
                _outerRotation;


            _outerRing.RenderTransformOrigin =
                new Point(.5, .5);



            _canvas.Children.Add(
                _outerRing);



            //--------------------------------------------------
            // Inner Ring
            //--------------------------------------------------

            double innerRadius =
                _theme.ReactorRadius *
                .55;


            _innerRing =
                HudShapeFactory.Ellipse(
                    innerRadius * 2,
                    innerRadius * 2,
                    Brushes.Transparent);



            _innerRing.Stroke =
                HudBrushFactory.Solid(
                    _theme.SecondaryColor);


            _innerRing.StrokeThickness =
                2;

            

            Canvas.SetLeft(
                _innerRing,
                _centerX -
                innerRadius);



            Canvas.SetTop(
                _innerRing,
                _centerY -
                innerRadius);



            _canvas.Children.Add(
                _innerRing);



            //--------------------------------------------------
            // Core Glow
            //--------------------------------------------------

            double coreSize =
                _theme.ReactorRadius *
                .35;



            _core =
                HudShapeFactory.Ellipse(
                    coreSize,
                    coreSize,
                    HudBrushFactory.Glow(
                        _theme.PrimaryColor),
                    _theme.ReactorGlowStrength);



            Canvas.SetLeft(
                _core,
                _centerX -
                coreSize / 2);



            Canvas.SetTop(
                _core,
                _centerY -
                coreSize / 2);



            _canvas.Children.Add(
                _core);
        }



        //--------------------------------------------------
        // Update
        //--------------------------------------------------

        public void Update(
            double deltaTime)
        {
            _time += deltaTime;


            //--------------------------------------------------
            // Rotate Outer Ring
            //--------------------------------------------------

            if (_outerRotation != null)
            {
                _outerRotation.Angle =
                    _time * 45;
            }



            //--------------------------------------------------
            // Pulse Core
            //--------------------------------------------------

            if (_core != null)
            {
                _core.Opacity =
                    _theme.ReactorGlowStrength +
                    (
                        .05 *
                        Math.Sin(
                            _time * 3)
                    );
            }
        }



        //--------------------------------------------------
        // Resize
        //--------------------------------------------------

        public void Resize(
            double width,
            double height)
        {
            _centerX = width / 2;

            _centerY = height / 2;


            Initialize(
                width,
                height);
        }



        //--------------------------------------------------
        // Dispose
        //--------------------------------------------------

        public void Dispose()
        {
            _canvas.Children.Clear();
        }
    }
    */
}

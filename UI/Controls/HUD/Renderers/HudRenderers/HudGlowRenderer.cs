using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Factories;
using UI.Controls.HUD.Managers;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Renderers.HudRenderers
{
    /// <summary>
    /// Renders animated glow effects on the Glow layer.
    /// </summary>
    public sealed class HudGlowRenderer : BaseHudRenderer
    {
        private readonly List<Ellipse> _glows = new();

        private double _pulseTime;

        public HudGlowRenderer(
            HudLayerManager layerManager)
            : base(layerManager, HudLayer.Glow)
        {
        }

        //-----------------------------------------------------
        // Build
        //-----------------------------------------------------

        protected override void Build()
        {
            _glows.Clear();

            CreateOuterGlow();
            CreateCoreGlow();
        }

        //-----------------------------------------------------
        // Outer Glow
        //-----------------------------------------------------

        private void CreateOuterGlow()
        {
            Ellipse glow = CreateOrb(
                Theme.ReactorGlowRadius,
                Theme.GlowOpacity);

            glow.Fill = Theme.ReactorHighlightBrush;

            ApplyGlow(glow);

            Add(
                glow,
                Center.X - Theme.ReactorGlowRadius,
                Center.Y - Theme.ReactorGlowRadius);

            _glows.Add(glow);
        }

        //-----------------------------------------------------
        // Core Glow
        //-----------------------------------------------------

        private void CreateCoreGlow()
        {
            Ellipse glow = CreateOrb(
                Theme.ReactorCoreGlowRadius,
                Theme.GlowOpacity + .15);

            glow.Fill = Theme.ReactorCoreBrush;

            ApplyGlow(glow);

            Add(
                glow,
                Center.X - Theme.ReactorCoreGlowRadius,
                Center.Y - Theme.ReactorCoreGlowRadius);

            _glows.Add(glow);
        }

        //-----------------------------------------------------
        // Update
        //-----------------------------------------------------

        public override void Update(
            double deltaTime)
        {
            _pulseTime +=
                deltaTime *
                Theme.CorePulseSpeed;

            double pulse =
                0.75 +
                Math.Sin(_pulseTime) * 0.25;

            foreach (Ellipse glow in _glows)
            {
                glow.Opacity =
                    Theme.GlowOpacity *
                    pulse;

                double scale =
                    1.0 +
                    Math.Sin(_pulseTime) * 0.04;

                glow.RenderTransform =
                    new ScaleTransform(
                        scale,
                        scale,
                        glow.Width / 2,
                        glow.Height / 2);
            }
        }

        //-----------------------------------------------------

        protected override void OnThemeChanged(
            HudTheme theme)
        {
            base.OnThemeChanged(theme);

            _pulseTime = 0;
        }

        //-----------------------------------------------------

        public override void Dispose()
        {
            _glows.Clear();

            base.Dispose();
        }
    }
    /*
    public sealed class HudGlowRenderer : IHudRenderer
    {
        private readonly Canvas _canvas;

        private readonly HudTheme _theme;

        private readonly ReactorState _state;

        private readonly HudEffects _effects;

        private Ellipse? _outerGlow;

        private Ellipse? _coreGlow;

        public HudGlowRenderer(
            HudLayerManager layers,
            HudTheme theme,
            ReactorManager reactor,
            HudEffectManager effects)
        {
            _canvas = layers.GetLayer(HudLayer.Glow);

            _theme = theme;

            _state = reactor.State;

            _effects = effects.Effects;
        }

        public void Initialize(
            double width,
            double height)
        {
            BuildGlow();
        }

        //--------------------------------------------------

        private void BuildGlow()
        {
            Point center = _state.Center;

            _outerGlow = CreateGlowEllipse(
                _theme.ReactorGlowRadius);

            _coreGlow = CreateGlowEllipse(
                _theme.ReactorCoreGlowRadius);

            PositionGlow(
                _outerGlow,
                _theme.ReactorGlowRadius);

            PositionGlow(
                _coreGlow,
                _theme.ReactorCoreGlowRadius);

            _canvas.Children.Add(_outerGlow);

            _canvas.Children.Add(_coreGlow);
        }

        //--------------------------------------------------

        private Ellipse CreateGlowEllipse(
            double radius)
        {
            return new Ellipse
            {
                Width = radius * 2,

                Height = radius * 2,

                Fill =
                    HudBrushFactory.CreateGlow(
                        _theme.PrimaryColor,
                        .35),

                IsHitTestVisible = false,

                Opacity = _theme.GlowOpacity
            };
        }

        //--------------------------------------------------

        private void PositionGlow(
            Ellipse ellipse,
            double radius)
        {
            Canvas.SetLeft(
                ellipse,
                _state.Center.X - radius);

            Canvas.SetTop(
                ellipse,
                _state.Center.Y - radius);
        }

        //--------------------------------------------------

        public void Update(
            double deltaTime)
        {
            if (_outerGlow == null)
                return;

            double pulse =
                .85 +
                (_effects.Glow * .35);

            _outerGlow.Opacity =
                _theme.GlowOpacity *
                _effects.Glow;

            _coreGlow!.Opacity =
                (_theme.GlowOpacity + .15) *
                pulse;

            PositionGlow(
                _outerGlow,
                _theme.ReactorGlowRadius);

            PositionGlow(
                _coreGlow,
                _theme.ReactorCoreGlowRadius);
        }

        public void Resize(
            double width,
            double height)
        {
            _canvas.Children.Clear();

            BuildGlow();
        }

        public void Dispose()
        {
            _canvas.Children.Clear();
        }
    }
    */
}

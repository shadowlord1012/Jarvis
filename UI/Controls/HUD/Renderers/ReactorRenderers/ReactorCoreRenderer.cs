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

namespace UI.Controls.HUD.Renderers.ReactorRenderers
{
    /// <summary>
    /// Draws the animated reactor core.
    /// </summary>
    public sealed class ReactorCoreRenderer : BaseReactorRenderer
    {
        private RotateTransform? _triangleRotation;
        private RotateTransform? _hexRotation;

        private Ellipse? _centerOrb;
        private Ellipse? _energyRing;

        private double _pulseTime;

        public ReactorCoreRenderer(
            HudLayerManager layers,
            ReactorManager reactor)
            : base(layers, reactor)
        {
        }

        //----------------------------------------------------

        protected override void Build()
        {
            BuildOuterTriangle();
            BuildInnerTriangle();
            BuildHexagon();
            BuildEnergyRing();
            BuildCenterOrb();
        }

        //----------------------------------------------------

        private void BuildOuterTriangle()
        {
            Path triangle = CreatePath(
                HudShapeFactory.CreateTriangle(
                    Center,
                    Theme.CoreOuterTriangleRadius),

                Theme.CoreLineThickness,

                Theme.CoreOpacity);

            _triangleRotation = new RotateTransform(
                0,
                Center.X,
                Center.Y);

            triangle.RenderTransform =
                _triangleRotation;

            Add(triangle);
        }

        //----------------------------------------------------

        private void BuildInnerTriangle()
        {
            Path triangle = CreatePath(
                HudShapeFactory.CreateTriangle(
                    Center,
                    Theme.CoreInnerTriangleRadius,
                    90),

                1.5,

                .75);

            triangle.RenderTransform =
                _triangleRotation;

            Add(triangle);
        }

        //----------------------------------------------------

        private void BuildHexagon()
        {
            Path hex = CreatePath(
                HudShapeFactory.CreatePolygon(
                    Center,
                    Theme.CoreHexagonRadius,
                    6),

                1.5,

                .8);

            _hexRotation =
                new RotateTransform(
                    0,
                    Center.X,
                    Center.Y);

            hex.RenderTransform =
                _hexRotation;

            Add(hex);
        }

        //----------------------------------------------------

        private void BuildEnergyRing()
        {
            _energyRing = CreateRing(
                Theme.CoreEnergyRingRadius,
                2,
                .85);

            Add(
                _energyRing,
                Center.X - Theme.CoreEnergyRingRadius,
                Center.Y - Theme.CoreEnergyRingRadius);

            ApplyGlow(_energyRing);
        }

        //----------------------------------------------------

        private void BuildCenterOrb()
        {
            _centerOrb = CreateOrb(
                Theme.CoreOrbRadius,
                .80);

            Add(
                _centerOrb,
                Center.X - Theme.CoreOrbRadius,
                Center.Y - Theme.CoreOrbRadius);

            ApplyGlow(_centerOrb);
        }

        //----------------------------------------------------

        public override void Update(
            double deltaTime)
        {
            State.CoreRotation +=
                Theme.CoreRotationSpeed *
                deltaTime;

            if (_triangleRotation != null)
            {
                _triangleRotation.Angle =
                    State.CoreRotation;
            }

            if (_hexRotation != null)
            {
                _hexRotation.Angle =
                    -State.CoreRotation * .6;
            }

            _pulseTime +=
                deltaTime *
                Theme.CorePulseSpeed;

            double scale =
                1 +
                Math.Sin(_pulseTime) * .08;

            if (_centerOrb != null)
            {
                _centerOrb.RenderTransform =
                    new ScaleTransform(
                        scale,
                        scale,
                        Theme.CoreOrbRadius,
                        Theme.CoreOrbRadius);
            }

            if (_energyRing != null)
            {
                _energyRing.Opacity =
                    .55 +
                    Math.Sin(_pulseTime) * .25;
            }
        }

        //----------------------------------------------------

        protected override void OnThemeChanged(
            HudTheme theme)
        {
            base.OnThemeChanged(theme);

            _pulseTime = 0;
        }
    }
}

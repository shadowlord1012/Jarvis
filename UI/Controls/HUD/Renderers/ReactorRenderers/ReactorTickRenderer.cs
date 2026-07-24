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
using Path = System.Windows.Shapes.Path;

namespace UI.Controls.HUD.Renderers.ReactorRenderers
{
    /// <summary>
    /// Draws the rotating tick marks around the reactor.
    /// </summary>
    public sealed class ReactorTickRenderer
        : BaseReactorRenderer
    {
        private readonly List<Path> _ticks =
            new();

        private RotateTransform? _rotation;

        //----------------------------------------------------

        public ReactorTickRenderer(
            HudLayerManager layers,
            ReactorManager reactor)
            : base(
                layers,
                reactor)
        {
        }

        //----------------------------------------------------

        protected override void Build()
        {
            _ticks.Clear();

            _rotation = CreateRotation();

            foreach (
                Geometry geometry
                in HudShapeFactory.CreateTickRing(
                    Center,
                    Theme.ReactorTickRadius,
                    Theme.ReactorTickLength,
                    Theme.ReactorTickCount))
            {
                Path tick =
                    CreatePath(
                        geometry,
                        Theme.ReactorTickThickness,
                        Theme.ReactorTickOpacity);

                tick.RenderTransform =
                    _rotation;

                ApplyGlow(tick);

                Add(tick);

                _ticks.Add(tick);
            }

            /*
             * foreach (Geometry geometry in HudShapeFactory.CreateTickRing(...))
                {
                    _ticks.Add(
                        AddPath(
                            geometry,
                            Theme.ReactorTickThickness,
                            Theme.ReactorTickOpacity,
                            _rotation,
                            true));
                } */
        }

        //----------------------------------------------------

        public override void Update(
            double deltaTime)
        {
            if (_rotation == null)
                return;

            State.TickRotation +=
                Theme.ReactorTickRotationSpeed *
                deltaTime;

            if (State.TickRotation >= 360)
                State.TickRotation -= 360;

            _rotation.Angle =
                State.TickRotation;
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
            _ticks.Clear();

            base.Dispose();
        }
    }
}

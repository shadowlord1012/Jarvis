using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UI.Controls.HUD.Definitions;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Factories;
using UI.Controls.HUD.Managers;
using UI.Controls.HUD.Models;
using Path = System.Windows.Shapes.Path;

namespace UI.Controls.HUD.Renderers.ReactorRenderers
{
    /// <summary>
    /// Draws animated energy beams between the reactor core
    /// and the orbit nodes.
    /// </summary>
    public sealed class ReactorEnergyBeamRenderer
        : BaseReactorRenderer
    {
        private sealed class BeamVisual
        {
            public OrbitNodeDefinition Node = default!;

            public Path Beam = default!;
        }

        private readonly List<BeamVisual> _beams = new();

        private double _pulseTime;

        public ReactorEnergyBeamRenderer(
            HudLayerManager layers,
            ReactorManager reactor)
            : base(layers, reactor)
        {
        }

        //--------------------------------------------------

        protected override void Build()
        {
            _beams.Clear();

            foreach (OrbitNodeDefinition node in State.OrbitNodes)
            {
                Path beam = CreatePath(
                    Geometry.Empty,
                    Theme.BeamThickness,
                    Theme.BeamOpacity);

                beam.StrokeStartLineCap = PenLineCap.Round;
                beam.StrokeEndLineCap = PenLineCap.Round;
                beam.IsHitTestVisible = false;

                Add(beam);

                _beams.Add(new BeamVisual
                {
                    Node = node,
                    Beam = beam
                });
            }
        }

        //--------------------------------------------------

        public override void Update(
            double deltaTime)
        {
            _pulseTime +=
                deltaTime *
                Theme.BeamPulseSpeed;

            double pulse =
                (Math.Sin(_pulseTime) + 1.0) * 0.5;

            foreach (BeamVisual beam in _beams)
            {
                UpdateBeam(
                    beam,
                    pulse);
            }
        }

        //--------------------------------------------------

        private void UpdateBeam(
            BeamVisual visual,
            double pulse)
        {
            Point center = Center;

            double radians =
                visual.Node.Angle *
                Math.PI /
                180.0;

            Point orbit = new(
                center.X +
                visual.Node.Radius *
                Math.Cos(radians),

                center.Y +
                visual.Node.Radius *
                Math.Sin(radians));

            PathFigure figure = new()
            {
                StartPoint = center
            };

            figure.Segments.Add(
                new LineSegment(
                    orbit,
                    true));

            visual.Beam.Data =
                new PathGeometry(
                    new[]
                    {
                        figure
                    });

            visual.Beam.StrokeThickness =
                Theme.BeamThickness *
                (1.0 +
                 pulse *
                 Theme.BeamPulseStrength);

            visual.Beam.Opacity =
                Theme.BeamOpacity *
                (.70 +
                 pulse * .30);
        }

        //--------------------------------------------------

        protected override void OnThemeChanged(
            HudTheme theme)
        {
            base.OnThemeChanged(theme);

            _pulseTime = 0;
        }

        //--------------------------------------------------

        public override void Dispose()
        {
            _beams.Clear();

            base.Dispose();
        }
    }
}

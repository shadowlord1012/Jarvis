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
    /// Renders animated segmented reactor rings.
    /// </summary>
    public sealed class ReactorSegmentRenderer
        : BaseReactorRenderer
    {
        private sealed class RingVisual
        {
            public ReactorRingDefinition Definition
            {
                get;
                init;
            } = default!;

            public RotateTransform Rotation
            {
                get;
                init;
            } = default!;

            public List<Path> Paths
            {
                get;
            } = new();
        }

        private readonly List<RingVisual> _rings =
            new();

        //----------------------------------------------------

        public ReactorSegmentRenderer(
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
            _rings.Clear();

            foreach (
                ReactorRingDefinition ring
                in Theme.ReactorRings)
            {
                BuildRing(ring);
            }
        }

        //----------------------------------------------------

        private void BuildRing(
            ReactorRingDefinition definition)
        {
            RingVisual visual =
                new()
                {
                    Definition = definition,

                    Rotation =
                        new RotateTransform(
                            0,
                            Center.X,
                            Center.Y)
                };

            double step =
                360.0 /
                definition.SegmentCount;

            for (
                int i = 0;
                i < definition.SegmentCount;
                i++)
            {
                double start =
                    i * step +
                    definition.GapAngle / 2;

                Path path =
                    CreatePath(
                        HudShapeFactory.CreateArc(
                            Center,
                            definition.Radius,
                            start,
                            definition.SweepAngle),

                        definition.Thickness,

                        definition.Opacity);

                path.StrokeStartLineCap =
                    PenLineCap.Round;

                path.StrokeEndLineCap =
                    PenLineCap.Round;

                path.RenderTransform =
                    visual.Rotation;

                ApplyGlow(path);

                Add(path);

                visual.Paths.Add(path);
            }

            _rings.Add(visual);
        }

        //----------------------------------------------------

        public override void Update(
            double deltaTime)
        {
            foreach (
                RingVisual ring
                in _rings)
            {
                double direction =
                    ring.Definition.Clockwise
                        ? 1
                        : -1;

                ring.Rotation.Angle +=
                    direction *
                    ring.Definition.RotationSpeed *
                    deltaTime;
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
            _rings.Clear();

            base.Dispose();
        }
    }
}

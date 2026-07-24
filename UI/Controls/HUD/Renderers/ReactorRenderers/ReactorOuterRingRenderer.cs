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
    /// Renders the segmented outer reactor ring.
    /// </summary>
    public sealed class ReactorOuterRingRenderer
        : BaseReactorRenderer
    {
        private readonly List<Path> _segments = new();

        private RotateTransform? _rotation;

        public ReactorOuterRingRenderer(
            HudLayerManager layers,
            ReactorManager reactor)
            : base(layers, reactor)
        {
        }

        //----------------------------------------------------

        protected override void Build()
        {
            _segments.Clear();

            _rotation = new RotateTransform(
                0,
                Center.X,
                Center.Y);

            double sweep =
                360.0 /
                Theme.ReactorSegmentCount
                - Theme.ReactorGapAngle;

            for (int i = 0;
                 i < Theme.ReactorSegmentCount;
                 i++)
            {
                double start =
                    i *
                    (360.0 /
                     Theme.ReactorSegmentCount);

                Path segment =
                    CreatePath(
                        HudShapeFactory.CreateArc(
                            Center,
                            Theme.ReactorOuterRadius,
                            start,
                            sweep),

                        Theme.ReactorSegmentThickness,

                        Theme.ReactorForegroundOpacity);

                segment.RenderTransform =
                    _rotation;

                ApplyGlow(segment);

                Add(segment);

                _segments.Add(segment);
            }
        }

        //----------------------------------------------------

        public override void Update(
            double deltaTime)
        {
            State.OuterRotation +=
                Theme.ReactorOuterRotationSpeed *
                deltaTime;

            if (_rotation != null)
            {
                _rotation.Angle =
                    State.OuterRotation;
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
            _segments.Clear();

            base.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace UI.Controls.HUD.Renderers.ReactorRenderers
{
    public sealed class ReactorRingDefinition
    {
        public double Radius { get; init; }

        public int SegmentCount { get; init; }

        public double SweepAngle { get; init; }

        public double GapAngle { get; init; }

        public double Thickness { get; init; }

        public double Opacity { get; init; }

        public double RotationSpeed { get; init; }

        public bool Clockwise { get; init; }
    }
}

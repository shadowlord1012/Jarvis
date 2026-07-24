using System;
using System.Collections.Generic;
using System.Text;

namespace UI.Controls.HUD.Definitions
{
    public sealed class OrbitNodeDefinition
    {
        public string Id { get; init; } = "";

        public string Label { get; init; } = "";

        public double Angle { get; set; }

        public double Radius { get; init; }

        public double Size { get; init; } = 8;

        public bool Clockwise { get; init; } = true;

        public double OrbitSpeed { get; init; } = 5;

        public bool Visible { get; set; } = true;
        public double BeamIntensity { get; set; } = 1.0;
    }
}

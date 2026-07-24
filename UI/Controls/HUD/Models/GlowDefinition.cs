using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace UI.Controls.HUD.Models
{
    public sealed class GlowDefinition
    {
        public Point Center { get; init; }

        public double Radius { get; init; }

        public Brush Brush { get; init; }

        public double Opacity { get; init; }

        public bool Pulsing { get; init; }

        public double PulseSpeed { get; init; }
    }
}

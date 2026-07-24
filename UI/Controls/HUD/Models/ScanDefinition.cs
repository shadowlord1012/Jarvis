using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using UI.Controls.HUD.Enums;

namespace UI.Controls.HUD.Models
{
    public sealed class ScanDefinition
    {
        public ScanType Type { get; init; } = ScanType.Horizontal;

        public Brush Brush { get; init; } = Brushes.Cyan;

        public double Speed { get; init; } = 140;

        public double Thickness { get; init; } = 4;

        public double Opacity { get; init; } = 0.45;

        public bool Glow { get; init; } = true;
    }
}

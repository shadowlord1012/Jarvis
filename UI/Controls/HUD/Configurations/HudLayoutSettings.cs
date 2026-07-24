using System;
using System.Collections.Generic;
using System.Text;

namespace UI.Controls.HUD.Configurations
{
    public sealed class HudLayoutSettings
    {
        public double Margin { get; init; } = 20;

        public double Padding { get; init; } = 10;

        public double PanelSpacing { get; init; } = 16;

        public double WidgetSpacing { get; init; } = 8;

        public double SafeArea { get; init; } = 12;
    }
}

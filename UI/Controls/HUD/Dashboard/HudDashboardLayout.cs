using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace UI.Controls.HUD.Dashboard
{
    public sealed class HudDashboardLayout
    {
        public Rect LeftColumn { get; init; }

        public Rect RightColumn { get; init; }

        public Rect CenterViewport { get; init; }

        public Rect BottomConsole { get; init; }

        public Rect TopRibbon { get; init; }
    }
}

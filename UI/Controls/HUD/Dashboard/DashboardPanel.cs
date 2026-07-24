using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using UI.Controls.HUD.Interfaces;

namespace UI.Controls.HUD.Dashboard
{
    public sealed class DashboardPanel
    {
        public string Title { get; init; } = "";

        public Rect Bounds { get; set; }

        public bool Visible { get; set; } = true;

        public IHudWidget Widget { get; init; } = default!;
    }
}

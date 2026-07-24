using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Interfaces;

namespace UI.Controls.HUD.Dashboard
{
    public sealed class DashboardManager
    {
        private readonly List<DashboardPanel> _panels = new();

        public IReadOnlyList<DashboardPanel> Panels => _panels;

        public void Register(IHudWidget widget)
        {
            _panels.Add(
                new DashboardPanel
                {
                    Title = widget.Name,
                    Widget = widget
                });
        }
    }
}

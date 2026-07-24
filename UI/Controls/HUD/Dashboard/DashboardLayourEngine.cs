using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace UI.Controls.HUD.Dashboard
{
    public sealed class DashboardLayoutEngine
    {
        public void Layout(
            DashboardManager manager,
            double width,
            double height)
        {
            double margin = 40;

            double panelWidth = 280;

            double panelHeight = 130;

            double y = margin;

            foreach (var panel in manager.Panels)
            {
                panel.Bounds = new Rect(
                    width - panelWidth - margin,
                    y,
                    panelWidth,
                    panelHeight);

                y += panelHeight + 18;
            }
        }
    }
}

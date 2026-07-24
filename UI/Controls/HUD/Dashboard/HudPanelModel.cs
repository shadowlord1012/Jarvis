using System;
using System.Collections.Generic;
using System.Text;

namespace UI.Controls.HUD.Dashboard
{
    public sealed class HudPanelModel
    {
        public string Title { get; set; } = "";

        public double Width { get; set; }

        public double Height { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public bool Visible { get; set; } = true;
    }
}

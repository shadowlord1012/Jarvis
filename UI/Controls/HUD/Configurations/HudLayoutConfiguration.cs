using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Configurations
{
    public class HudLayoutConfiguration
    {
        public List<HudPanelDefinition> Panels { get; set; }
            = new();

        public string LayoutName { get; set; }
            = "Default";


        public string Version { get; set; }
            = "1.0";
    }
}

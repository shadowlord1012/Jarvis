using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace UI.Controls.HUD.Factories
{
    public enum PanelStyleEnum
    {
        Standard,

        Rounded,

        Reactor,

        Dashboard,

        Status,

        Popup,

        Minimal
    }
    public class PanelStyle
    {

        /// <summary>
        /// Optional custom panel background.
        /// If null, HudTheme.PanelBackground is used.
        /// </summary>
        public Brush Background { get; set; }



        /// <summary>
        /// Optional custom panel border.
        /// If null, HudTheme.BorderBrush is used.
        /// </summary>
        public Brush BorderBrush { get; set; }



        /// <summary>
        /// Panel border thickness.
        /// </summary>
        public Thickness BorderThickness { get; set; }
            = new Thickness(1);



        /// <summary>
        /// Panel corner radius.
        /// </summary>
        public CornerRadius CornerRadius { get; set; }
            = new CornerRadius(8);



        /// <summary>
        /// Panel opacity.
        /// </summary>
        public double Opacity { get; set; }
            = 1.0;

    }
}

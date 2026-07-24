using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UI.Controls.HUD.Abstract
{
    public class HudPanel : ContentControl
    {
        static HudPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(HudPanel),
                new FrameworkPropertyMetadata(typeof(HudPanel)));
        }

        //--------------------------------------------------------
        // Header
        //--------------------------------------------------------

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                nameof(Header),
                typeof(string),
                typeof(HudPanel),
                new PropertyMetadata("Panel"));

        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        //--------------------------------------------------------
        // Accent Color
        //--------------------------------------------------------

        public static readonly DependencyProperty AccentBrushProperty =
            DependencyProperty.Register(
                nameof(AccentBrush),
                typeof(System.Windows.Media.Brush),
                typeof(HudPanel));

        public System.Windows.Media.Brush AccentBrush
        {
            get => (System.Windows.Media.Brush)GetValue(AccentBrushProperty);
            set => SetValue(AccentBrushProperty, value);
        }


        public UIElement DecorationLayer { get; set; }

        public CornerRadius CornerRadius { get; set; }
        public string PanelId { get; set; }
    }
}

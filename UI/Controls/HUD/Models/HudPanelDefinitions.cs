using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace UI.Controls.HUD.Models
{
    public class HudPanelDefinition
    {
        public string Id { get; set; }

        public string Title { get; set; }


        public double Width { get; set; } = 400;

        public double Height { get; set; } = 250;


        public Thickness Margin { get; set; }


        public HorizontalAlignment HorizontalAlignment { get; set; }
            = HorizontalAlignment.Center;


        public VerticalAlignment VerticalAlignment { get; set; }
            = VerticalAlignment.Center;



        public Brush Background { get; set; }

        public Brush BorderBrush { get; set; }


        public Thickness? BorderThickness { get; set; }


        public CornerRadius? CornerRadius { get; set; }


        public double Opacity { get; set; } = 1;



        public bool ShowHeader { get; set; } = true;


        public double HeaderHeight { get; set; } = 35;



        public bool EnableDecorations { get; set; } = true;


        public string DecorationStyle { get; set; }
            = "Default";

        public HudPosition Position { get; set; }
           = new();


        public bool IsMovable { get; set; }
            = true;


        public bool IsVisible { get; set; }
            = true;
    }
}

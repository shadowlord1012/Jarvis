using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace UI.Controls.HUD.Models
{
    public static class HudThemes
    {
        public static HudTheme MarkXL { get; } = CreateMarkXL();

        public static HudTheme IronMan { get; } = CreateIronMan();

        public static HudTheme Tactical { get; } = CreateTactical();

        public static HudTheme NightVision { get; } = CreateNightVision();

        private static HudTheme CreateMarkXL()
        {
            var theme = new HudTheme
            {
                Name = "MARK XL",

                PrimaryBrush = Brushes.Cyan,

                SecondaryBrush = Brushes.DeepSkyBlue,

                AccentBrush = Brushes.White,

                BorderBrush = Brushes.Cyan,

                TextBrush = Brushes.White,

                GridBrush = new SolidColorBrush(Color.FromArgb(35, 0, 255, 255)),

                BackgroundBrush = new SolidColorBrush(Color.FromArgb(20, 0, 255, 255)),

                PanelBrush = new SolidColorBrush(Color.FromArgb(18, 0, 255, 255))
            };

            theme.Freeze();
            return theme;
        }

        private static HudTheme CreateIronMan()
        {
            var theme = new HudTheme
            {
                Name = "Iron Man",

                PrimaryBrush = Brushes.Orange,

                SecondaryBrush = Brushes.Gold,

                AccentBrush = Brushes.Red,

                BorderBrush = Brushes.Orange,

                TextBrush = Brushes.White
            };

            theme.Freeze();
            return theme;
        }

        private static HudTheme CreateTactical()
        {
            var theme = new HudTheme
            {
                Name = "Tactical",

                PrimaryBrush = Brushes.Lime,

                SecondaryBrush = Brushes.GreenYellow,

                AccentBrush = Brushes.White,

                BorderBrush = Brushes.Lime,

                TextBrush = Brushes.White
            };

            theme.Freeze();
            return theme;
        }

        private static HudTheme CreateNightVision()
        {
            var theme = new HudTheme
            {
                Name = "Night Vision",

                PrimaryBrush = Brushes.LawnGreen,

                SecondaryBrush = Brushes.Green,

                AccentBrush = Brushes.White,

                BorderBrush = Brushes.LawnGreen,

                TextBrush = Brushes.White
            };

            theme.Freeze();
            return theme;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Configurations
{
    public sealed class HudThemeChangedEventArgs : EventArgs
    {
        public HudThemeChangedEventArgs(
            HudTheme? oldTheme,
            HudTheme newTheme)
        {
            OldTheme = oldTheme;
            NewTheme = newTheme;
        }

        public HudTheme? OldTheme { get; }

        public HudTheme NewTheme { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Configurations;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Managers
{
    /// <summary>
    /// Manages all available HUD themes and the currently active theme.
    /// </summary>
    public sealed class HudThemeManager
    {
        private static readonly Lazy<HudThemeManager> _instance =
            new(() => new HudThemeManager());

        public static HudThemeManager Instance => _instance.Value;

        private readonly Dictionary<string, HudTheme> _themes =
            new(StringComparer.OrdinalIgnoreCase);

        private HudTheme _currentTheme;

        /// <summary>
        /// Raised whenever the active theme changes.
        /// </summary>
        public event EventHandler<HudThemeChangedEventArgs>? ThemeChanged;

        public HudTheme CurrentTheme
        {
            get => _currentTheme;
            private set
            {
                if (ReferenceEquals(_currentTheme, value))
                    return;

                var previous = _currentTheme;
                _currentTheme = value;

                ThemeChanged?.Invoke(
                    this,
                    new HudThemeChangedEventArgs(previous, _currentTheme));
            }
        }

        private HudThemeManager()
        {
            Register(HudThemes.MarkXL);
            Register(HudThemes.IronMan);
            Register(HudThemes.Tactical);
            Register(HudThemes.NightVision);

            _currentTheme = HudThemes.MarkXL;
        }

        /// <summary>
        /// Registers a new theme.
        /// Existing themes with the same name are replaced.
        /// </summary>
        public void Register(HudTheme theme)
        {
            ArgumentNullException.ThrowIfNull(theme);

            _themes[theme.Name] = theme;
        }

        /// <summary>
        /// Returns true if a theme exists.
        /// </summary>
        public bool Contains(string themeName)
        {
            return _themes.ContainsKey(themeName);
        }

        /// <summary>
        /// Gets a theme by name.
        /// </summary>
        public HudTheme GetTheme(string themeName)
        {
            if (!_themes.TryGetValue(themeName, out var theme))
                throw new InvalidOperationException(
                    $"HUD theme '{themeName}' does not exist.");

            return theme;
        }

        /// <summary>
        /// Sets the active theme.
        /// </summary>
        public void SetTheme(string themeName)
        {
            CurrentTheme = GetTheme(themeName);
        }

        /// <summary>
        /// Sets the active theme.
        /// </summary>
        public void SetTheme(HudTheme theme)
        {
            ArgumentNullException.ThrowIfNull(theme);

            Register(theme);

            CurrentTheme = theme;
        }

        /// <summary>
        /// Returns every registered theme.
        /// </summary>
        public IReadOnlyCollection<HudTheme> GetThemes()
        {
            return _themes.Values;
        }
    }
}

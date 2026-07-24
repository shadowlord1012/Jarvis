using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Managers;
using UI.Controls.HUD.Models;
using UI.Controls.HUD.Widgets;

namespace UI.Controls.HUD.Factories
{
    /// <summary>
    /// Creates HUD widgets.
    /// </summary>
    public sealed class HudWidgetFactory
    {
        private readonly Canvas _widgetLayer;

        private readonly HudTheme _theme;

        private readonly Dictionary<string, WidgetFactoryDelegate> _factories = new();

        private readonly SystemStatus _systemStatusService = new SystemStatus();
        private readonly ILogService _logService;

        public delegate IHudWidget WidgetFactoryDelegate(
            Canvas canvas,
            HudTheme theme,
            WidgetConfiguration configuration);

        public HudWidgetFactory(
            HudLayerManager layers,
            HudTheme theme, 
            ILogService logService)
        {
            _widgetLayer = layers.GetLayer(HudLayer.Widgets);

            _theme = theme;
            _logService = logService;

            RegisterBuiltInWidgets();
        }

        /// <summary>
        /// Registers a widget type.
        /// </summary>
        public void Register(
            string widgetType,
            WidgetFactoryDelegate factory)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(widgetType);
            ArgumentNullException.ThrowIfNull(factory);

            _factories[widgetType] = factory;
        }

        /// <summary>
        /// Returns true if the widget type exists.
        /// </summary>
        public bool IsRegistered(string widgetType)
        {
            return _factories.ContainsKey(widgetType);
        }

        /// <summary>
        /// Creates a widget.
        /// </summary>
        public IHudWidget Create(
            WidgetConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            if (!_factories.TryGetValue(
                    configuration.Type,
                    out WidgetFactoryDelegate? factory))
            {
                throw new InvalidOperationException(
                    $"Widget type '{configuration.Type}' is not registered.");
            }

            return factory(
                _widgetLayer,
                _theme,
                configuration);
        }

        /// <summary>
        /// Creates multiple widgets.
        /// </summary>
        public IEnumerable<IHudWidget> Create(
            IEnumerable<WidgetConfiguration> configurations)
        {
            foreach (WidgetConfiguration configuration in configurations)
            {
                if (!configuration.Enabled)
                    continue;

                yield return Create(configuration);
            }
        }

        /// <summary>
        /// Registers all built-in widgets.
        /// </summary>
        private void RegisterBuiltInWidgets()
        {
            Register(
                "Clock",
                (canvas, theme, config) =>
                    new ClockWidget(
                        canvas,
                        theme,
                        config));

            Register(
                "SystemStatus",
                (canvas, theme, config) =>
                    new SystemStatusWidget(
                        canvas,
                        theme,
                        config,
                        _systemStatusService,
                        _logService));
            /*
            Register(
                "Reactor",
                (canvas, theme, config) =>
                    new ReactorWidget(
                        canvas,
                        theme,
                        config));

            Register(
                "Mission",
                (canvas, theme, config) =>
                    new MissionWidget(
                        canvas,
                        theme,
                        config));

            Register(
                "Notification",
                (canvas, theme, config) =>
                    new NotificationWidget(
                        canvas,
                        theme,
                        config));

            Register(
                "Network",
                (canvas, theme, config) =>
                    new NetworkWidget(
                        canvas,
                        theme,
                        config));

            Register(
                "Weather",
                (canvas, theme, config) =>
                    new WeatherWidget(
                        canvas,
                        theme,
                        config));

            Register(
                "Audio",
                (canvas, theme, config) =>
                    new AudioWidget(
                        canvas,
                        theme,
                        config));
            */
        }
    }
}

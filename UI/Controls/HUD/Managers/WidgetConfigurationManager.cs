using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Widgets;

namespace UI.Controls.HUD.Managers
{
    public sealed class WidgetConfigurationManager
    {
        private List<WidgetConfiguration> _widgetConfigurations = new List<WidgetConfiguration>();
        private ILogService _logger;

        public WidgetConfigurationManager(ILogService logger) {     
            
            _logger = logger;
            LoadWidgetConfigurations();
        
        }

        private void LoadWidgetConfigurations()
        {
            // Load widget configurations from a data source (e.g., file, database, etc.)
            // For demonstration purposes, we'll create some sample configurations.
            WidgetConfiguration InputConfig = new()
            {
                Name = "Input",

                Type = "Input",

                Dock = WidgetDock.BottomCenter,

                Size = new Size(500, 100),

                Margin = new Thickness(15),

                Visible = true
            };
            WidgetConfiguration clockConfig = new()
            {
                Name = "Clock",

                Type = "Clock",

                Dock = WidgetDock.TopRight,

                Size = new Size(300, 140),

                Margin = new Thickness(15),

                Visible = true
            };
            WidgetConfiguration systemStatusConfig = new()
            {
                Name = "SystemStatus",

                Type = "SystemStatus",

                Dock = WidgetDock.BottomRight,

                Size = new Size(300, 140),

                Margin = new Thickness(15),

                Visible = true
            };
            WidgetConfiguration logConfig = new()
            {
                Name = "Log",

                Type = "Log",

                Dock = WidgetDock.BottomLeft,

                Size = new Size(300, 500),

                Margin = new Thickness(15),

                Visible = true
            };
            WidgetConfiguration configurationConfig = new()
            {
                Name = "Configuration",

                Type = "Configuration",

                Dock = WidgetDock.BottomLeft,

                Size = new Size(100, 50),

                Margin = new Thickness(15),

                Offset = new Point(300, 0),

                Visible = true
            };
            WidgetConfiguration configurationOverlayConfig = new()
            {
                Name = "ConfigurationOverlay",
                Type = "ConfigurationOverlay",
                Dock = WidgetDock.TopLeft,
                Size = new Size(800, 600),
                Margin = new Thickness(15),
                Visible = false // Start hidden, shown when config button is clicked
            };
            WidgetConfiguration documentationConfig = new()
            {
                Name = "Documentation",
                Type = "Documentation",
                Dock = WidgetDock.TopLeft,
                Size = new Size(250, 150),
                Margin = new Thickness(15),
                Offset = new Point(0, 150), // Offset down so it doesn't overlap with other widgets
                Visible = true 
            };


            _widgetConfigurations.Add(InputConfig);
            _widgetConfigurations.Add(clockConfig);
            _widgetConfigurations.Add(systemStatusConfig);
            _widgetConfigurations.Add(logConfig);
            _widgetConfigurations.Add(configurationConfig);
            _widgetConfigurations.Add(configurationOverlayConfig);
            _widgetConfigurations.Add(documentationConfig);

            foreach(var widgetConfiguration in _widgetConfigurations)
            {
                _logger.LogDebug("Widget Config",$"Loaded widget configuration: {widgetConfiguration.Name}");
            }

        }

        public WidgetConfiguration GetWidgetConfiguration(string widgetName)
        {


            if(string.IsNullOrEmpty(widgetName))
            {
                throw new ArgumentException("Widget name cannot be null or empty.", nameof(widgetName));
            }

            if(_widgetConfigurations == null || _widgetConfigurations.Count == 0)
            {
                throw new InvalidOperationException("Widget configurations have not been loaded.");
            }
            
            var config = _widgetConfigurations.Find(x => x.Name == widgetName);

            _logger.LogDebug("Widget Config", $"Retrieving widget configuration for: {widgetName}");

            if (config == null)
            {
                throw new KeyNotFoundException($"Widget configuration for '{widgetName}' not found.");
            }

            return config;
        }
    }
}

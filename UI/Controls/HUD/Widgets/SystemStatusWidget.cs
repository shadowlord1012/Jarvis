using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UI.Controls.HUD.Factories;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Widgets
{
    /// <summary>
    /// Displays system performance information.
    /// </summary>
    public sealed class SystemStatusWidget : HudWidgetBase
    {
        private readonly ISystemStatusService _statusService;
        private HudPanelFactory _panelFactory;
        private readonly ILogService _logService;

        private Canvas? _root;

        private TextBlock? _cpuValue;
        private TextBlock? _memoryValue;
        private TextBlock? _gpuValue;
        private TextBlock? _fpsValue;

        private double _updateTimer;

        private const double RefreshInterval = 0.50;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemStatusWidget"/> class.
        /// </summary>
        /// <param name="parentCanvas">The parent canvas on which the widget will be displayed.</param>
        /// <param name="theme">The theme to be applied to the widget.</param>
        /// <param name="configuration">The configuration settings for the widget.</param>
        /// <param name="statusService">The service providing system status information.</param>
        /// <param name="logService">The service used for logging system status warnings.</param>
        public SystemStatusWidget(
            Canvas parentCanvas,
            HudTheme theme,
            WidgetConfiguration configuration,
            ISystemStatusService statusService,
            ILogService logService)
            : base(parentCanvas, theme, configuration)
        {
            _panelFactory = new HudPanelFactory();
            _statusService = statusService;
            _logService = logService;
        }

        /// <summary>
        /// Creates the visual representation of the SystemStatusWidget, including the header and rows for CPU, Memory, GPU, and FPS values.
        /// </summary>
        /// <returns>The root element of the visual representation.</returns>
        protected override FrameworkElement CreateVisual()
        {
            _root = new Canvas
            {
                Width = Configuration.Size.Width,
                Height = Configuration.Size.Height
            };

            FrameworkElement panel =
                _panelFactory.CreatePanel(
                    new Rect(0,0,
                        Configuration.Size.Width,
                        Configuration.Size.Height),
                    Theme);

            _root.Children.Add(panel);

            AddHeader();

            _cpuValue = AddRow("CPU", 36);
            _memoryValue = AddRow("MEM", 60);
            _gpuValue = AddRow("GPU", 84);
            _fpsValue = AddRow("FPS", 108);

            return _root;
        }

        /// <summary>
        /// Updates the SystemStatusWidget with the latest system status information.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update.</param>
        public override void Update(double deltaTime)
        {
            _updateTimer += deltaTime;

            if (_updateTimer < RefreshInterval)
                return;

            _updateTimer = 0;

            SystemStatus status = _statusService.GetStatus();

            if (_cpuValue != null)
            {
                // Update CPU usage text and color based on threshold
                _cpuValue.Text = $"{status.CpuUsage:0}%";

                // Change color to red if CPU usage exceeds 80%
                if (status.CpuUsage > 80)
                {
                    _cpuValue.Foreground = new SolidColorBrush(Colors.Red);

                    // Log a warning message for high CPU usage
                    _logService.LogWarning("System Status Warning",$"High CPU usage detected: {status.CpuUsage:0}%");
                }
                else
                {
                    _cpuValue.Foreground = new SolidColorBrush(Theme.PrimaryColor);
                }
            }

            if (_memoryValue != null)
            {
                // Update Memory usage text and color based on threshold
                _memoryValue.Text =
                    $"{status.MemoryLeft:0}MB / {status.TotalMemoryMB:0}MB";

                // Change color to red if Memory left is below 500MB
                if (status.MemoryLeft < 500)
                {
                    _memoryValue.Foreground = new SolidColorBrush(Colors.Red);

                    // Log a warning message for low memory
                    _logService.LogWarning("System Status Warning", $"Low Memory detected: {status.MemoryLeft:0}MB left");
                }
                else
                {
                    _memoryValue.Foreground = new SolidColorBrush(Theme.PrimaryColor);
                }
            }

            if (_gpuValue != null)
            {
                // Update GPU usage text and color based on threshold
                _gpuValue.Text =
                    $"{status.GpuUsage:0}%";

                // Change color to red if GPU usage exceeds 90%
                if (status.GpuUsage > 90)
                {
                    _gpuValue.Foreground = new SolidColorBrush(Colors.Red);
                    // Log a warning message for high GPU usage
                    _logService.LogWarning("System Status Warning", $"High GPU usage detected: {status.GpuUsage:0}%");
                }
                else
                {
                    _gpuValue.Foreground = new SolidColorBrush(Theme.PrimaryColor);
                }

            }
            if (_fpsValue != null)
                _fpsValue.Text =
                    $"{status.FramesPerSecond:0}";
        }

        private void AddHeader()
        {

            // Add the header text "SYSTEM STATUS" to the widget
            TextBlock title = new()
            {
                Text = "SYSTEM STATUS",
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground =
                    new SolidColorBrush(
                        Theme.PrimaryColor)
            };

            Canvas.SetLeft(title, 16);
            Canvas.SetTop(title, 10);

            _root!.Children.Add(title);

            Line divider =
                HudShapeFactory.CreateLine(
                    new Point(16, 28),
                    new Point(
                        Configuration.Size.Width - 16,
                        28),
                    Theme);

            _root.Children.Add(divider);
        }

        private TextBlock AddRow(
            string label,
            double y)
        {
            TextBlock left = new()
            {
                Text = label,
                FontSize = 12,
                Foreground =
                    new SolidColorBrush(
                        Theme.PrimaryColor)
            };

            Canvas.SetLeft(left, 18);
            Canvas.SetTop(left, y);

            _root!.Children.Add(left);

            TextBlock value = new()
            {
                Width = 170,
                TextAlignment = TextAlignment.Right,
                FontFamily =
                    new FontFamily("Consolas"),
                FontSize = 12,
                Foreground =
                    new SolidColorBrush(
                        Theme.PrimaryColor),
                Text = "--"
            };

            Canvas.SetLeft(
                value,
                Configuration.Size.Width - 190);

            Canvas.SetTop(value, y);

            _root.Children.Add(value);

            return value;
        }
    }
}

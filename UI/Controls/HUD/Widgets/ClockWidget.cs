using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UI.Controls.HUD.Factories;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Widgets
{
    /// <summary>
    /// Displays the current system time.
    /// </summary>
    public sealed class ClockWidget : HudWidgetBase
    {
        private Canvas? _root;

        private TextBlock? _timeText;

        private TextBlock? _dateText;
        private HudPanelFactory _panelFactory;

        private string _lastTime = string.Empty;

        private string _lastDate = string.Empty;

        public ClockWidget(
            Canvas parentCanvas,
            HudTheme theme,
            WidgetConfiguration configuration)
            : base(parentCanvas, theme, configuration)
        {
            _panelFactory = new HudPanelFactory();
        }

        protected override FrameworkElement CreateVisual()
        {
            _root = new Canvas
            {
                Width = Configuration.Size.Width,
                Height = Configuration.Size.Height
            };

            FrameworkElement panel = _panelFactory.CreatePanel(
                new Rect(0, 0, Configuration.Size.Width, Configuration.Size.Height),
                Theme);

            _root.Children.Add(panel);

            _timeText = new TextBlock
            {
                FontSize = 28,
                FontWeight = FontWeights.SemiBold,
                Foreground = new SolidColorBrush(Theme.PrimaryColor),
                FontFamily = new FontFamily("Consolas"),
                Text = "--:--:--"
            };

            Canvas.SetLeft(_timeText, 20);
            Canvas.SetTop(_timeText, 18);

            _root.Children.Add(_timeText);

            _dateText = new TextBlock
            {
                FontSize = 13,
                Foreground = new SolidColorBrush(Theme.PrimaryColor),
                Opacity = 0.75,
                FontFamily = new FontFamily("Consolas"),
                Text = "-- --- ----"
            };

            Canvas.SetLeft(_dateText, 22);
            Canvas.SetTop(_dateText, 58);

            _root.Children.Add(_dateText);

            Line divider = HudShapeFactory.CreateLine(
                new Point(18, 88),
                new Point(Configuration.Size.Width - 18, 88),
                Theme);

            _root.Children.Add(divider);

            TextBlock label = new TextBlock
            {
                Text = "SYSTEM CLOCK",
                FontSize = 11,
                Foreground = new SolidColorBrush(Theme.PrimaryColor),
                Opacity = 0.55,
                FontFamily = new FontFamily("Consolas")
            };

            Canvas.SetLeft(label, 20);
            Canvas.SetTop(label, 96);

            _root.Children.Add(label);

            UpdateClock();

            return _root;
        }

        public override void Update(double deltaTime)
        {
            UpdateClock();
        }

        private void UpdateClock()
        {
            DateTime now = DateTime.Now;

            string time = now.ToString("HH:mm:ss");
            string date = now.ToString("ddd dd MMM yyyy");

            if (_timeText != null && time != _lastTime)
            {
                _timeText.Text = time;
                _lastTime = time;
            }

            if (_dateText != null && date != _lastDate)
            {
                _dateText.Text = date;
                _lastDate = date;
            }
        }
    }
}

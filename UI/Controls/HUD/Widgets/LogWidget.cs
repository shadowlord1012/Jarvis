using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Factories;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Widgets
{
    /// <summary>
    /// Displays live Jarvis activity, errors, summaries and debug information.
    /// </summary>
    public sealed class LogWidget : HudWidgetBase
    {
        private readonly ILogService _logService;

        private Canvas? _root;

        private ScrollViewer? _scrollViewer;

        private StackPanel? _logPanel;
        private LogEntry? _activeAIResponse;

        private const int MaxEntries = 20;
        private HudPanelFactory _panelFactory;

        public LogWidget(
            Canvas parentCanvas,
            HudTheme theme,
            WidgetConfiguration configuration,
            ILogService logService)
            : base(parentCanvas, theme, configuration)
        {
            _logService = logService;
            _panelFactory = new HudPanelFactory();
        }

        protected override FrameworkElement CreateVisual()
        {
            _root = new Canvas()
            {
                Width = Configuration.Size.Width,
                Height = Configuration.Size.Height
            };

            Canvas panel =
                _panelFactory.CreatePanel(
                    new Rect(
                        0,
                        0,
                        Configuration.Size.Width,
                        Configuration.Size.Height),
                    Theme);

            _root.Children.Add(panel);

            TextBlock title = _panelFactory.CreateHeader(
                "JARVIS ACTIVITY",
                Theme);

            _root.Children.Add(title);

            Line divider =
                _panelFactory.CreateHeaderDivider(
                    Configuration.Size.Width,
                    Theme);

            _root.Children.Add(divider);

            _logPanel = new StackPanel();

            _scrollViewer = new ScrollViewer
            {
                Width = Configuration.Size.Width - 20,
                Height = Configuration.Size.Height - 42,

                VerticalScrollBarVisibility =
                    ScrollBarVisibility.Hidden,

                HorizontalScrollBarVisibility =
                    ScrollBarVisibility.Disabled,

                Content = _logPanel
            };

            Canvas.SetLeft(_scrollViewer, 10);
            Canvas.SetTop(_scrollViewer, 36);

            _root.Children.Add(_scrollViewer);

            _logService.EntryAdded += OnEntryAdded;

            return _root;
        }

        private void OnEntryAdded(
            object? sender,
            LogEntry entry)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                AddEntry(entry);
            });
        }

        private void AddEntry(LogEntry entry)
        {
            if (_logPanel == null)
                return;

            if (entry.Severity == LogSeverity.Info || entry.Severity == LogSeverity.Summary)
            {
                TextBlock row = new()
                {
                    FontFamily = new FontFamily("Consolas"),

                    FontSize = 11,

                    Text =
                        $"[{entry.Timestamp:HH:mm:ss}] " +
                        $"{entry.Source} - {entry.Message}",

                    Foreground =
                        GetBrush(entry.Severity),

                    Margin = new Thickness(0, 1, 0, 1),

                    TextWrapping = TextWrapping.Wrap
                };

                _logPanel.Children.Add(row);

                while (_logPanel.Children.Count > MaxEntries)
                {
                    _logPanel.Children.RemoveAt(0);
                }

                _scrollViewer?.ScrollToEnd();
            }
        }

        public void Log(
            string message,            
            string source = "JARVIS")
        {
            _logService.LogInfo(source,message);
        }


        private Brush GetBrush(LogSeverity severity)
        {
            return severity switch
            {
                LogSeverity.Success =>
                    Brushes.Lime,

                LogSeverity.Warning =>
                    Brushes.Gold,

                LogSeverity.Error =>
                    Brushes.OrangeRed,

                LogSeverity.Debug =>
                    Brushes.DeepSkyBlue,

                LogSeverity.Summary =>
                    Brushes.Cyan,

                _ =>
                    Theme.PrimaryBrush
            };
        }

        public override void Dispose()
        {
            _logService.EntryAdded -= OnEntryAdded;

            base.Dispose();
        }

        public void BeginAIResponse()
        {
            _activeAIResponse =
                new LogEntry
                {
                    Timestamp =
                        DateTime.Now,

                    Severity =
                        LogSeverity.Info,

                    Message =
                        "JARVIS > "
                };


            AddEntry(_activeAIResponse);
        }
        public void AppendAIResponse(
            string text)
        {
            if (_activeAIResponse is null)
            {
                BeginAIResponse();
            }


            _activeAIResponse!.Message +=
                text;


            RefreshDisplay();
        }
        public void EndAIResponse()
        {
            _activeAIResponse =
                null;
        }

        private void RefreshDisplay()
        {
            if (_activeAIResponse is null || _logPanel is null)
                return;

            var lastElement = _logPanel.Children[_logPanel.Children.Count - 1] as TextBlock;
            if (lastElement is not null)
            {
                lastElement.Text =
                    $"[{_activeAIResponse.Timestamp:HH:mm:ss}] " +
                    $"{_activeAIResponse.Source} - {_activeAIResponse.Message}";
            }

            _scrollViewer?.ScrollToEnd();
        }
    }
}

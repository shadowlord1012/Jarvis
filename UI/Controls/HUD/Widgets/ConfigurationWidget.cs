using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UI.Controls.HUD.Factories;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Widgets
{
    internal class ConfigurationWidget : HudWidgetBase
    {

        //--------------------------------------------------------------
        // Private Fields
        //--------------------------------------------------------------

        private Canvas? _root;

        private Button? _configButton;

        private readonly HudPanelFactory _panelFactory;

        //Added a logger to log events and errors, will use if needed.
        private readonly ILogService _logger;

        //--------------------------------------------------------------
        // Events
        //--------------------------------------------------------------

        public event EventHandler? ButtonClicked;



        public ConfigurationWidget(
            Canvas parentCanvas, 
            HudTheme theme, 
            WidgetConfiguration configuration,
            ILogService logger) 
            : base(parentCanvas, theme, configuration)
        {
            _logger = logger;
            _panelFactory = new HudPanelFactory();
        }

        //--------------------------------------------------------------
        // Root Canvas
        //--------------------------------------------------------------

        protected override FrameworkElement CreateVisual()
        {
            // Create the root canvas for the widget
            _root = new Canvas
            {
                Width = Configuration.Size.Width,
                Height = Configuration.Size.Height,
            };

            // Create the panel for the widget
            Canvas panel = 
                _panelFactory.CreatePanel(
                    new Rect(
                        0,
                        0,
                        Configuration.Size.Width,
                        Configuration.Size.Height),
                    Theme);

            // Add the panel to the root canvas
            _root.Children.Add(panel);

            _configButton = new Button
            {
                Content = "Config",
                FontFamily = new FontFamily("Algerian"),
                FontSize = 13,
                Width = 80,
                Height = 30,
                Margin = new Thickness(10),
                Foreground = Theme.TextBrush,
                Background = Theme.SecondaryBrush
            };

            _configButton.Click += OnButtonClicked;

            Canvas.SetLeft(_configButton, 0);
            Canvas.SetTop(_configButton, 0);

            // Add the config button to the root canvas
            _root.Children.Add(_configButton);

            return _root;
        }

        //--------------------------------------------------------------
        // Event Handlers
        //--------------------------------------------------------------

        public void OnButtonClicked(object sender, RoutedEventArgs e)
        {

            //TODO: Add logic to handle the button click event, such as opening a configuration dialog or performing some action

            ButtonClicked?.Invoke(this, EventArgs.Empty);


        }

        //--------------------------------------------------------------
        // Activation
        //--------------------------------------------------------------

        public void Activate()
        {
            Show();
        }


        //--------------------------------------------------------------
        // Deactivation
        //--------------------------------------------------------------

        public void Deactivate()
        {
            Hide();
        }

        //--------------------------------------------------------------
        // Dispose
        //--------------------------------------------------------------

        public override void Dispose()
        {
            base.Dispose();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using UI.Controls.HUD.Factories;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Models;
using UI.Controls.HUD.Widgets;

namespace Jarvis.UI.Controls.HUD.Widgets
{
    /// <summary>
    /// Provides interactive text input for communicating with Jarvis.
    /// 
    /// The widget follows the same positioning and rendering structure
    /// as LogWidget. HudWidgetBase and WidgetConfiguration control the
    /// overall widget position, while this class controls the internal
    /// layout.
    /// </summary>
    public sealed class HudInputWidget : HudWidgetBase
    {
        // ============================================================
        // PRIVATE FIELDS
        // ============================================================

        private Canvas? _root;

        private TextBox? _inputBox;

        private TextBlock? _promptText;

        private TextBlock? _statusText;

        private readonly HudPanelFactory _panelFactory;

        private readonly ILogService _logger;


        // ============================================================
        // EVENTS
        // ============================================================

        /// <summary>
        /// Raised when the user presses Enter with valid input.
        /// </summary>
        public event EventHandler<string>? InputSubmitted;

        /// <summary>
        /// Raised when the user presses Escape.
        /// </summary>
        public event EventHandler? InputCancelled;


        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public HudInputWidget(
            Canvas parentCanvas,
            HudTheme theme,
            WidgetConfiguration configuration,
            ILogService logger)
            : base(
                parentCanvas,
                theme,
                configuration)
        {
            _logger = logger;
            _panelFactory =
                new HudPanelFactory();
        }


        // ============================================================
        // CREATE VISUAL
        // ============================================================

        protected override FrameworkElement CreateVisual()
        {
            // --------------------------------------------------------
            // ROOT CANVAS
            // --------------------------------------------------------

            _root =
                new Canvas
                {
                    Width =
                        Configuration.Size.Width,

                    Height =
                        Configuration.Size.Height
                };


            // --------------------------------------------------------
            // MAIN PANEL
            // --------------------------------------------------------

            Canvas panel =
                _panelFactory.CreatePanel(
                    new Rect(
                        0,
                        0,
                        Configuration.Size.Width,
                        Configuration.Size.Height),
                    Theme);

            _root.Children.Add(
                panel);


            // --------------------------------------------------------
            // PROMPT
            // --------------------------------------------------------

            _promptText =
                new TextBlock
                {
                    Text =
                        "JARVIS >",

                    FontFamily = Theme.AccentFontFamily,

                    FontSize =
                        20,

                    FontWeight =
                        FontWeights.Bold,

                    Foreground =
                        Theme.PrimaryBrush,

                    VerticalAlignment =
                        VerticalAlignment.Center
                };

            Canvas.SetLeft(
                _promptText,
                15);

            Canvas.SetTop(
                _promptText,
                17);

            _root.Children.Add(
                _promptText);


            // --------------------------------------------------------
            // INPUT BOX
            // --------------------------------------------------------

            _inputBox =
                new TextBox
                {
                    Width =
                        Math.Max(
                            100,
                            Configuration.Size.Width - 20),

                    Height =
                        Configuration.Size.Height - 10,

                    Background =
                        Brushes.Transparent,

                    BorderThickness =
                        new Thickness(0),

                    Foreground =
                        Brushes.White,

                    CaretBrush =
                        Theme.PrimaryBrush,

                    FontFamily = Theme.BodyFontFamily,

                    FontSize =
                        14,

                    VerticalContentAlignment =
                        VerticalAlignment.Center,

                    Padding =
                        new Thickness(
                            5,
                            0,
                            5,
                            0),

                    AcceptsReturn =
                        false,

                    TextWrapping =
                        TextWrapping.NoWrap
                };

            _inputBox.KeyDown +=
                InputBox_KeyDown;

            Canvas.SetLeft(
                _inputBox,
                10);

            Canvas.SetTop(
                _inputBox,
                25);

            _root.Children.Add(
                _inputBox);


            // --------------------------------------------------------
            // STATUS
            // --------------------------------------------------------

            _statusText =
                new TextBlock
                {
                    Text =
                        "ENTER",

                    FontFamily =
                        new FontFamily(
                            "Consolas"),

                    FontSize =
                        10,

                    Foreground =
                        Theme.PrimaryBrush
                };

            Canvas.SetRight(
                _statusText,
                15);

            Canvas.SetTop(
                _statusText,
                20);

            _root.Children.Add(
                _statusText);


            // --------------------------------------------------------
            // RETURN ROOT
            // --------------------------------------------------------

            return _root;
        }


        // ============================================================
        // KEYBOARD INPUT
        // ============================================================

        private void InputBox_KeyDown(
            object sender,
            KeyEventArgs e)
        {
            // --------------------------------------------------------
            // ENTER
            // --------------------------------------------------------

            if (e.Key ==
                Key.Enter)
            {
                e.Handled =
                    true;

                SubmitInput();

                return;
            }


            // --------------------------------------------------------
            // ESCAPE
            // --------------------------------------------------------

            if (e.Key ==
                Key.Escape)
            {
                e.Handled =
                    true;

                Clear();

                SetStatus(
                    "READY");

                InputCancelled?
                    .Invoke(
                        this,
                        EventArgs.Empty);
            }
        }


        // ============================================================
        // SUBMIT INPUT
        // ============================================================

        private void SubmitInput()
        {
            if (_inputBox == null)
            {
                return;
            }

            string input =
                _inputBox.Text.Trim();

            _logger.LogInfo("Input -> ", input);

            if (string.IsNullOrWhiteSpace(
                    input))
            {
                return;
            }

            // --------------------------------------------------------
            // CLEAR INPUT
            // --------------------------------------------------------

            _inputBox.Clear();


            // --------------------------------------------------------
            // UPDATE STATUS
            // --------------------------------------------------------

            SetStatus(
                "THINKING");


            // --------------------------------------------------------
            // RAISE EVENT
            // --------------------------------------------------------

            InputSubmitted?
                .Invoke(
                    this,
                    input);
        }


        // ============================================================
        // ACTIVATE
        // ============================================================

        public void Activate()
        {
            Show();

            if (_inputBox == null)
            {
                return;
            }

            Application.Current.Dispatcher.BeginInvoke(
                new Action(
                    () =>
                    {
                        _inputBox.Focus();

                        Keyboard.Focus(
                            _inputBox);
                    }));
        }


        // ============================================================
        // DEACTIVATE
        // ============================================================

        public void Deactivate()
        {
            Hide();
        }


        // ============================================================
        // CLEAR INPUT
        // ============================================================

        public void Clear()
        {
            _inputBox?.Clear();
        }


        // ============================================================
        // GET CURRENT TEXT
        // ============================================================

        public string GetText()
        {
            return _inputBox?
                       .Text
                       .Trim()
                   ?? string.Empty;
        }


        // ============================================================
        // SET STATUS
        // ============================================================

        public void SetStatus(
            string status)
        {
            if (_statusText == null)
            {
                return;
            }

            _statusText.Text =
                status;
        }


        // ============================================================
        // DISPOSE
        // ============================================================

        public override void Dispose()
        {
            if (_inputBox != null)
            {
                _inputBox.KeyDown -=
                    InputBox_KeyDown;
            }

            InputSubmitted =
                null;

            InputCancelled =
                null;

            base.Dispose();
        }
    }
}

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using UI.Controls.HUD.Factories;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Widgets
{
    public class ConfigurationOverlayWidget : HudWidgetBase
    {
        //--------------------------------------------------------------
        // Private Fields
        //--------------------------------------------------------------
        private Canvas? _root;

        #region Whisper Buttons

        private TextBlock? _whisperTextBlock;
        private Button? _whisperTinyButton;
        private Button? _whisperBaseButton;
        private Button? _whisperSmallButton;
        private Button? _whisperMedButton;
        private Button? _whisperLargeButton;
        private Button? _whisperLargeV2Button;
        private ToggleButton? _translateToEnglish;

        /* will make an advanced settings section later, for now just keep it simple.
        private Button? _useGPU;
        private TextBox? _threadNumberTextBox;
        */
        #endregion

        #region LLM Buttons

        private TextBlock? _llmTextBlock;
        private TextBox? _ollamaModelTextBox;
        private TextBox? _ollamaBaseURL;

        /* will make an advanced settings section later, for now just keep it simple.
        private TextBox? _timeoutTextBox;
        private TextBox? _maxRetriesTextBox;
        private Button? _keepAliveButton;
        private TextBox? _temperatureTextBox;
        private TextBox? _maxTokensTextBox;
        private TextBox? _topPTextBox;
        private Button? _enableStreamingButton;
        private Button? _enableToolCallingButton;        
        */
        #endregion

        #region TTS Buttons

        private TextBlock? _ttsTextBlock;
        private Button? _edgeButton;
        private Button? _elevenLabsButton;
        private TextBlock? _voiceTextBlock;
        private TextBox? _voiceTextBox;
        private TextBlock? _elevenLabsAPIKeyTextBlock;
        private TextBox? _elevenLabsAPIKey;
        private TextBlock? _elevenLabsVoiceTextBlock;
        private TextBox? _elevenLabsVoiceID;
        private TextBlock? _modelIDTextBlock;
        private TextBox? _modelIDTextBox;

        #endregion

        #region Other buttons / controls

        private Button? _saveButton;
        private Button? _cancelButton;

        #endregion


        private readonly HudPanelFactory _panelFactory;
        //Added a logger to log events and errors, will use if needed.
        private readonly ILogService _logger;
        private readonly IConfiguration _configuration;

        // Track original values to detect changes
        private string? _originalOllamaModel;
        private string? _originalOllamaBaseUrl;
        private string? _originalWhisperModel;
        private bool _originalTranslateToEnglish;
        private string? _originalTtsProvider;
        private string? _originalEdgeVoice;
        private string? _originalElevenLabsApiKey;
        private string? _originalElevenLabsVoiceId;
        private string? _originalElevenLabsModelId;

        public ConfigurationOverlayWidget(Canvas parentCanvas, HudTheme theme, WidgetConfiguration configuration, ILogService logger, IConfiguration appConfiguration) : base(parentCanvas, theme, configuration)
        {
            _panelFactory = new HudPanelFactory();
            _logger = logger;
            _configuration = appConfiguration;
        }

        //--------------------------------------------------------------
        // Events
        //--------------------------------------------------------------
        public event EventHandler? ButtonClicked;

        protected override FrameworkElement CreateVisual()
        {
            _root = new Canvas
            {
                Width = Configuration.Size.Width,
                Height = Configuration.Size.Height,
            };

            Canvas panel = _panelFactory.CreatePanel(
                new Rect(0, 0, Configuration.Size.Width, Configuration.Size.Height), Theme);

            panel.Background = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0)); // Semi-transparent black background

            _root.Children.Add(panel);

            AddHeader();
            addLLM();
            addWhisper();
            addTTS();
            addOther();

            return _root;
        }

        //--------------------------------------------------------------
        // Activiation
        //--------------------------------------------------------------

        public void Activate()
        {
            _logger.LogDebug("ConfigurationOverlayWidget", "Activate() called - showing overlay");
            Show();
            // Load settings after showing to ensure all controls are created
            _logger.LogDebug("ConfigurationOverlayWidget", "Calling LoadSettings()");
            LoadSettings();
        }

        public override void Show()
        {
            _logger.LogDebug("ConfigurationOverlayWidget", "Show() called");
            base.Show();
            // Ensure settings are loaded whenever the widget is shown
            if (IsInitialized && RootElement != null)
            {
                _logger.LogDebug("ConfigurationOverlayWidget", "Widget is initialized, calling LoadSettings() from Show()");
                LoadSettings();
            }
            else
            {
                _logger.LogWarning("ConfigurationOverlayWidget", $"Widget not ready - IsInitialized: {IsInitialized}, RootElement: {(RootElement != null ? "EXISTS" : "NULL")}");
            }
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

        #region Event Handlers

        private void ToggleButton_IsClicked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                toggleButton.Background = Brushes.Red; // Selected color
            }
        }
        private void ToggleButton_IsUnClicked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                toggleButton.Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)); // Default color
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button clickedButton)
                return;
            
            // Find all buttons in the same parent container
            if (clickedButton.Parent is Panel parent)
            {
                foreach (UIElement child in parent.Children)
                {
                    if (child is Button button)
                    {
                        if (button.Content.ToString() == "English" || button.Content.ToString() == "Edge TTS" || button.Content.ToString() == "ElevenLabs")
                        {
                            continue; // Skip the "English" and TTS provider buttons
                        }
                        // Reset all buttons to default
                        button.Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30));
                    }
                }
            }

            // Set the clicked button to the selected colour
            clickedButton.Background = Brushes.Red;
        }
        #endregion

        private void ttsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button clickedButton)
                return;
            // Reset all TTS buttons to default
            if (_edgeButton != null)
                _edgeButton.Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30));
            if (_elevenLabsButton != null)
                _elevenLabsButton.Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30));
            // Set the clicked button to the selected colour
            clickedButton.Background = Brushes.Red;
        }

        #region Private Methods

        private void addOther()
        {
            _saveButton = new Button
            {
                Content = "Save",
                Width = 100,
                Height = 30,
                FontSize = 14,
                FontFamily = Theme.AccentFontFamily,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                Margin = new Thickness(10, 10, 0, 0)
            };

            Canvas.SetRight(_saveButton, 200);
            Canvas.SetBottom(_saveButton, 40);

            _saveButton.Click += SaveButton_Click;

            _root!.Children.Add(_saveButton);

            _cancelButton = new Button
            {
                Content = "Cancel",
                Width = 100,
                Height = 30,
                FontSize = 14,
                FontFamily = Theme.AccentFontFamily,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                Margin = new Thickness(10, 10, 0, 0)
            };
            Canvas.SetRight(_cancelButton, 50);
            Canvas.SetBottom(_cancelButton, 40);

            _cancelButton.Click += CancelButton_Click;

            _root!.Children.Add(_cancelButton);
        }

        /// <summary>
        /// Adds the TTS settings section to the configuration overlay.
        /// </summary>
        private void addTTS()
        {
            _ttsTextBlock = new TextBlock
            {
                Text = "TTS Settings",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Theme.PrimaryBrush,
                Margin = new Thickness(10, 10, 0, 0)
            };

            Canvas.SetLeft(_ttsTextBlock, 10);
            Canvas.SetTop(_ttsTextBlock, 250);

            _root!.Children.Add(_ttsTextBlock);

            _edgeButton = new Button
            {
                Content = "Edge TTS",
                Width = 100,
                Height = 30,
                FontSize = 14,
                FontFamily = Theme.AccentFontFamily,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                Margin = new Thickness(10, 10, 0, 0)
            };

            _edgeButton.Click += ttsButton_Click;

            Canvas.SetLeft(_edgeButton, 40);
            Canvas.SetTop(_edgeButton, 280);

            _root!.Children.Add(_edgeButton);

            _elevenLabsButton = new Button
            {
                Content = "Eleven Labs TTS",
                Width = 150,
                Height = 30,
                FontSize = 14,
                FontFamily = Theme.AccentFontFamily,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                Margin = new Thickness(10, 10, 0, 0)
            };

            _elevenLabsButton.Click += ttsButton_Click;

            Canvas.SetLeft(_elevenLabsButton, 160);
            Canvas.SetTop(_elevenLabsButton, 280);

            _root!.Children.Add(_elevenLabsButton);

            _voiceTextBlock = new TextBlock
            {
                Text = "Edge TTS Voice ID:",
                FontSize = 14,
                Foreground = Theme.PrimaryBrush,
                Margin = new Thickness(10, 10, 0, 0)
            };

            Canvas.SetLeft(_voiceTextBlock, 40);
            Canvas.SetTop(_voiceTextBlock, 320);

            _root!.Children.Add(_voiceTextBlock);

            _voiceTextBox = new TextBox
            {
                Text = "en-GB-RyanNeural",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                FontFamily = Theme.AccentFontFamily,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                Width = 200,
                Height = 20,
                Margin = new Thickness(10, 10, 0, 0),
                TextWrapping = TextWrapping.NoWrap,
            };

            Canvas.SetLeft(_voiceTextBox, 200);
            Canvas.SetTop(_voiceTextBox, 320);

            _root!.Children.Add(_voiceTextBox);

            _elevenLabsAPIKeyTextBlock = new TextBlock
            {
                Text = "Eleven Labs API Key:",
                FontSize = 14,
                Foreground = Theme.PrimaryBrush,
                Margin = new Thickness(10, 10, 0, 0)
            };

            Canvas.SetLeft(_elevenLabsAPIKeyTextBlock, 40);
            Canvas.SetTop(_elevenLabsAPIKeyTextBlock, 360);

            _root!.Children.Add(_elevenLabsAPIKeyTextBlock);

            _elevenLabsAPIKey = new TextBox
            {
                Text = "",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                FontFamily = Theme.AccentFontFamily,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                Width = 200,
                Height = 20,
                Margin = new Thickness(10, 10, 0, 0),
                TextWrapping = TextWrapping.NoWrap,
            };

            Canvas.SetLeft(_elevenLabsAPIKey, 200);
            Canvas.SetTop(_elevenLabsAPIKey, 360);

            _root!.Children.Add(_elevenLabsAPIKey);

            _elevenLabsVoiceTextBlock = new TextBlock
            {
                Text = "Eleven Labs Voice ID:",
                FontSize = 14,
                Foreground = Theme.PrimaryBrush,
                Margin = new Thickness(10, 10, 0, 0)
            };

            Canvas.SetLeft(_elevenLabsVoiceTextBlock, 40);
            Canvas.SetTop(_elevenLabsVoiceTextBlock, 400);

            _root!.Children.Add(_elevenLabsVoiceTextBlock);

            _elevenLabsVoiceID  = new TextBox
            {
                Text = "",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                FontFamily = Theme.AccentFontFamily,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                Width = 200,
                Height = 20,
                Margin = new Thickness(10, 10, 0, 0),
                TextWrapping = TextWrapping.NoWrap,
            };

            Canvas.SetLeft(_elevenLabsVoiceID, 200);
            Canvas.SetTop(_elevenLabsVoiceID, 400);

            _root!.Children.Add(_elevenLabsVoiceID);

            _modelIDTextBlock = new TextBlock
            {
                Text = "Eleven Labs Model ID:",
                FontSize = 14,
                Foreground = Theme.PrimaryBrush,
                Margin = new Thickness(10, 10, 0, 0)
            };

            Canvas.SetLeft(_modelIDTextBlock, 40);
            Canvas.SetTop(_modelIDTextBlock, 440);

            _root!.Children.Add(_modelIDTextBlock);

            _modelIDTextBox = new TextBox
            {
                Text = "eleven_multilingual_v1",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                FontFamily = Theme.AccentFontFamily,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                Width = 200,
                Height = 20,
                Margin = new Thickness(10, 10, 0, 0),
                TextWrapping = TextWrapping.NoWrap,
            };

            Canvas.SetLeft(_modelIDTextBox, 200);
            Canvas.SetTop(_modelIDTextBox, 440);

            _root!.Children.Add(_modelIDTextBox);
        }

        /// <summary>
        /// Adds the Whisper settings section to the configuration overlay.
        /// </summary>
        private void addWhisper()
        {
            _whisperTextBlock = new TextBlock
            {
                Text = "Whisper Settings",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                FontFamily = Theme.AccentFontFamily,
                Foreground = Theme.PrimaryBrush,
                Margin = new Thickness(10, 10, 0, 0)
            };
            Canvas.SetLeft(_whisperTextBlock, 10);
            Canvas.SetTop(_whisperTextBlock, 150);

            _root!.Children.Add(_whisperTextBlock);

            _translateToEnglish = new ToggleButton
            {
                Content = "English",
                Width = 100,
                Height = 25,
                FontSize = 14,
                FontFamily = Theme.AccentFontFamily,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                Margin = new Thickness(10, 10, 0, 0),
            };
            _translateToEnglish.Checked += ToggleButton_IsClicked;
            _translateToEnglish.Unchecked += ToggleButton_IsUnClicked;

            Canvas.SetLeft(_translateToEnglish, 160);
            Canvas.SetTop(_translateToEnglish, 150);

            _root!.Children.Add(_translateToEnglish);

            _whisperTinyButton = new Button
            {
                Content = "Tiny",
                Width = 80,
                Height = 30,
                FontSize = 14,
                FontFamily = Theme.AccentFontFamily,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                Margin = new Thickness(10, 10, 0, 0)
            };
            _whisperTinyButton.Click += Button_Click;

            Canvas.SetLeft(_whisperTinyButton, 40);
            Canvas.SetTop(_whisperTinyButton, 180);

            _root!.Children.Add(_whisperTinyButton);

            _whisperBaseButton = new Button
            {
                Content = "Base",
                Width = 80,
                Height = 30,
                FontSize = 14,
                FontFamily = Theme.AccentFontFamily,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                Margin = new Thickness(10, 10, 0, 0)
            };
            _whisperBaseButton.Click += Button_Click;

            Canvas.SetLeft(_whisperBaseButton, 130);
            Canvas.SetTop(_whisperBaseButton, 180);

            _root!.Children.Add(_whisperBaseButton);

            _whisperSmallButton = new Button
            {
                Content = "Small",
                Width = 80,
                Height = 30,
                FontSize = 14,
                FontFamily = Theme.AccentFontFamily,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Background = Brushes.Red,
                Margin = new Thickness(10, 10, 0, 0)
            };
            _whisperSmallButton.Click += Button_Click;

            Canvas.SetLeft(_whisperSmallButton, 220);
            Canvas.SetTop(_whisperSmallButton, 180);

            _root!.Children.Add(_whisperSmallButton);

            _whisperMedButton = new Button
            {
                Content = "Medium",
                Width = 80,
                Height = 30,
                FontSize = 14,
                FontFamily = Theme.AccentFontFamily,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                Margin = new Thickness(10, 10, 0, 0)
            };
            _whisperMedButton.Click += Button_Click;

            Canvas.SetLeft(_whisperMedButton, 310);
            Canvas.SetTop(_whisperMedButton, 180);

            _root!.Children.Add(_whisperMedButton);

            _whisperLargeButton = new Button
            {
                Content = "Large",
                Width = 80,
                Height = 30,
                FontSize = 14,
                FontFamily = Theme.AccentFontFamily,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                Margin = new Thickness(10, 10, 0, 0)
            };
            _whisperLargeButton.Click += Button_Click;

            Canvas.SetLeft(_whisperLargeButton, 400);
            Canvas.SetTop(_whisperLargeButton, 180);

            _root!.Children.Add(_whisperLargeButton);

            _whisperLargeV2Button = new Button
            {
                Content = "Large V2",
                Width = 80,
                Height = 30,
                FontSize = 14,
                FontFamily = Theme.AccentFontFamily,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                Margin = new Thickness(10, 10, 0, 0)
            };
            _whisperLargeV2Button.Click += Button_Click;

            Canvas.SetLeft(_whisperLargeV2Button, 490);
            Canvas.SetTop(_whisperLargeV2Button, 180);

            _root!.Children.Add(_whisperLargeV2Button);

            
        }


        /// <summary>
        /// Adds the LLM settings section to the configuration overlay.
        /// </summary>
        private void addLLM()
        {

            _llmTextBlock = new TextBlock
            {
                Text = "LLM Settings",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Theme.PrimaryBrush,
                Margin = new Thickness(10, 10, 0, 0)
            };

            Canvas.SetLeft(_llmTextBlock, 10);
            Canvas.SetTop(_llmTextBlock, 40);

            _root!.Children.Add(_llmTextBlock);

            TextBlock _llmModelText = new TextBlock
            {
                Text = "Ollama Model:",
                FontSize = 14,
                Foreground = Theme.PrimaryBrush,
                Margin = new Thickness(10, 40, 0, 0)
            };

            Canvas.SetLeft(_llmModelText, 40);
            Canvas.SetTop(_llmModelText, 50);

            _root!.Children.Add(_llmModelText);

            _ollamaModelTextBox = new TextBox
            {
                Text = "qwen2.5:7b",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                FontFamily = Theme.AccentFontFamily,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                Width = 200,
                Height = 20,
                Margin = new Thickness(10, 70, 0, 0),
                TextWrapping = TextWrapping.NoWrap,
            };

            Canvas.SetLeft(_ollamaModelTextBox, 160);
            Canvas.SetTop(_ollamaModelTextBox, 20);

            _root!.Children.Add(_ollamaModelTextBox);

            TextBlock _ollamaBaseURLText = new TextBlock
            {
                Text = "Ollama Base URL:",
                FontSize = 14,
                Foreground = Theme.PrimaryBrush,
                Margin = new Thickness(10, 40, 0, 0)
            };

            Canvas.SetLeft(_ollamaBaseURLText, 40);
            Canvas.SetTop(_ollamaBaseURLText, 80);

            _root!.Children.Add(_ollamaBaseURLText);

            _ollamaBaseURL = new TextBox
            {
                Text = "http://localhost:11434",
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                FontFamily = Theme.AccentFontFamily,
                Foreground = Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)),
                Width = 200,
                Height = 20,
                Margin = new Thickness(10, 70, 0, 0),
                TextWrapping = TextWrapping.NoWrap,
            };

            Canvas.SetLeft(_ollamaBaseURL, 160);
            Canvas.SetTop(_ollamaBaseURL, 50);

            _root!.Children.Add(_ollamaBaseURL);


        }

        /// <summary>
        /// Adds the header section to the configuration overlay.
        /// </summary>
        private void AddHeader()
        {

            // Add the header text "JARVIS CONFIGURATION" to the widget
            TextBlock title = new()
            {
                Text = "JARVIS CONFIGURATION",
                FontSize = 16,
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
                    new Point(16, 30),
                    new Point(
                        Configuration.Size.Width - 16,
                        30),
                    Theme);

            _root.Children.Add(divider);
        }

        /// <summary>
        /// Loads settings from appsettings.json into the UI controls.
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                // Load Ollama settings
                var ollamaSection = _configuration.GetSection("Ollama");
                if (ollamaSection.Exists())
                {
                    _originalOllamaModel = ollamaSection["DefaultModel"] ?? "qwen2.5:7b";
                    _originalOllamaBaseUrl = ollamaSection["BaseUrl"] ?? "http://localhost:11434";

                    if (_ollamaModelTextBox != null)
                        _ollamaModelTextBox.Text = _originalOllamaModel;

                    if (_ollamaBaseURL != null)
                        _ollamaBaseURL.Text = _originalOllamaBaseUrl;
                }

                // Load Whisper settings
                var whisperSection = _configuration.GetSection("Whisper");
                if (whisperSection.Exists())
                {
                    _originalWhisperModel = whisperSection["ModelName"] ?? "small";
                    var modelName = _originalWhisperModel.ToLowerInvariant();

                    // Set the appropriate Whisper model button as selected
                    ResetWhisperButtons();
                    switch (modelName)
                    {
                        case "tiny":
                            if (_whisperTinyButton != null)
                                _whisperTinyButton.Background = Brushes.Red;
                            break;
                        case "base":
                            if (_whisperBaseButton != null)
                                _whisperBaseButton.Background = Brushes.Red;
                            break;
                        case "small":
                            if (_whisperSmallButton != null)
                                _whisperSmallButton.Background = Brushes.Red;
                            break;
                        case "medium":
                            if (_whisperMedButton != null)
                                _whisperMedButton.Background = Brushes.Red;
                            break;
                        case "large":
                            if (_whisperLargeButton != null)
                                _whisperLargeButton.Background = Brushes.Red;
                            break;
                        case "large-v2":
                            if (_whisperLargeV2Button != null)
                                _whisperLargeV2Button.Background = Brushes.Red;
                            break;
                    }

                    // Set translate to English toggle
                    if (_translateToEnglish != null)
                    {
                        var translateValue = whisperSection["TranslateToEnglish"];
                        _originalTranslateToEnglish = translateValue != null && bool.Parse(translateValue);
                        _translateToEnglish.IsChecked = _originalTranslateToEnglish;
                    }
                }

                // Load TTS settings
                var ttsSection = _configuration.GetSection("TTS");
                if (ttsSection.Exists())
                {
                    _originalTtsProvider = ttsSection["Provider"] ?? "EdgeTTS";

                    // Set the appropriate TTS provider button as selected
                    ResetTtsButtons();
                    if (_originalTtsProvider.Equals("EdgeTTS", StringComparison.OrdinalIgnoreCase))
                    {
                        if (_edgeButton != null)
                            _edgeButton.Background = Brushes.Red;
                    }
                    else if (_originalTtsProvider.Equals("ElevenLabs", StringComparison.OrdinalIgnoreCase))
                    {
                        if (_elevenLabsButton != null)
                            _elevenLabsButton.Background = Brushes.Red;
                    }

                    // Load Edge TTS settings
                    var edgeTtsSection = ttsSection.GetSection("EdgeTTS");
                    if (edgeTtsSection.Exists() && _voiceTextBox != null)
                    {
                        _originalEdgeVoice = edgeTtsSection["Voice"] ?? "en-GB-RyanNeural";
                        _voiceTextBox.Text = _originalEdgeVoice;
                    }

                    // Load ElevenLabs settings
                    var elevenLabsSection = ttsSection.GetSection("ElevenLabs");
                    if (elevenLabsSection.Exists())
                    {
                        _originalElevenLabsApiKey = elevenLabsSection["ApiKey"] ?? "";
                        _originalElevenLabsVoiceId = elevenLabsSection["VoiceId"] ?? "";
                        _originalElevenLabsModelId = elevenLabsSection["ModelId"] ?? "eleven_multilingual_v2";

                        _logger.LogDebug("ConfigurationOverlayWidget", $"ElevenLabs section found - ApiKey: {(string.IsNullOrEmpty(_originalElevenLabsApiKey) ? "EMPTY" : "EXISTS")}, VoiceId: {(string.IsNullOrEmpty(_originalElevenLabsVoiceId) ? "EMPTY" : "EXISTS")}, ModelId: {_originalElevenLabsModelId}");
                        _logger.LogDebug("ConfigurationOverlayWidget", $"Controls status - _elevenLabsAPIKey: {(_elevenLabsAPIKey != null ? "EXISTS" : "NULL")}, _elevenLabsVoiceID: {(_elevenLabsVoiceID != null ? "EXISTS" : "NULL")}, _modelIDTextBox: {(_modelIDTextBox != null ? "EXISTS" : "NULL")}");

                        if (_elevenLabsAPIKey != null)
                        {
                            _elevenLabsAPIKey.Text = _originalElevenLabsApiKey;
                            _logger.LogDebug("ConfigurationOverlayWidget", $"Set API Key: {_elevenLabsAPIKey.Text}");
                        }

                        if (_elevenLabsVoiceID != null)
                        {
                            _elevenLabsVoiceID.Text = _originalElevenLabsVoiceId;
                            _logger.LogDebug("ConfigurationOverlayWidget", $"Set Voice ID: {_elevenLabsVoiceID.Text}");
                        }

                        if (_modelIDTextBox != null)
                        {
                            _modelIDTextBox.Text = _originalElevenLabsModelId;
                            _logger.LogDebug("ConfigurationOverlayWidget", $"Set Model ID: {_modelIDTextBox.Text}");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("ConfigurationOverlayWidget", "ElevenLabs section does not exist in configuration");
                    }
                }

                _logger.LogDebug("ConfigurationOverlayWidget", "Configuration settings loaded successfully from appsettings.json");
            }
            catch (Exception ex)
            {
                _logger.LogError("ConfigurationOverlayWidget", $"Error loading settings from appsettings.json: {ex.Message}");
            }
        }

        /// <summary>
        /// Resets all Whisper model buttons to default background.
        /// </summary>
        private void ResetWhisperButtons()
        {
            var defaultBrush = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30));
            if (_whisperTinyButton != null) _whisperTinyButton.Background = defaultBrush;
            if (_whisperBaseButton != null) _whisperBaseButton.Background = defaultBrush;
            if (_whisperSmallButton != null) _whisperSmallButton.Background = defaultBrush;
            if (_whisperMedButton != null) _whisperMedButton.Background = defaultBrush;
            if (_whisperLargeButton != null) _whisperLargeButton.Background = defaultBrush;
            if (_whisperLargeV2Button != null) _whisperLargeV2Button.Background = defaultBrush;
        }

        /// <summary>
        /// Resets all TTS provider buttons to default background.
        /// </summary>
        private void ResetTtsButtons()
        {
            var defaultBrush = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30));
            if (_edgeButton != null) _edgeButton.Background = defaultBrush;
            if (_elevenLabsButton != null) _elevenLabsButton.Background = defaultBrush;
        }

        /// <summary>
        /// Handles the Save button click event - saves changes to appsettings.json.
        /// </summary>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger.LogInfo("ConfigurationOverlayWidget", "Save button clicked - checking for changes");

                if (!HasChanges())
                {
                    _logger.LogInfo("ConfigurationOverlayWidget", "No changes detected - skipping save");
                    Deactivate();
                    return;
                }

                _logger.LogInfo("ConfigurationOverlayWidget", "Changes detected - saving to appsettings.json");
                await SaveSettingsAsync();

                _logger.LogSuccess("ConfigurationOverlayWidget", "Settings saved successfully");
                Deactivate();
            }
            catch (Exception ex)
            {
                _logger.LogError("ConfigurationOverlayWidget", $"Failed to save settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the Cancel button click event - discards changes and closes overlay.
        /// Shows a confirmation dialog if there are unsaved changes.
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _logger.LogInfo("ConfigurationOverlayWidget", "Cancel button clicked");

            // Check if there are any unsaved changes
            if (HasChanges())
            {
                // Show confirmation dialog
                var result = MessageBox.Show(
                    "You have unsaved changes. Are you sure you want to cancel?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                {
                    _logger.LogInfo("ConfigurationOverlayWidget", "User chose to continue editing");
                    return; // User chose to continue editing
                }

                _logger.LogInfo("ConfigurationOverlayWidget", "User confirmed - discarding changes");
            }
            else
            {
                _logger.LogInfo("ConfigurationOverlayWidget", "No changes detected - closing overlay");
            }

            Deactivate();
        }

        /// <summary>
        /// Checks if any settings have been changed from their original values.
        /// </summary>
        private bool HasChanges()
        {
            // Check Ollama changes
            if (_ollamaModelTextBox?.Text != _originalOllamaModel)
            {
                _logger.LogDebug("ConfigurationOverlayWidget", $"Ollama Model changed: '{_originalOllamaModel}' -> '{_ollamaModelTextBox?.Text}'");
                return true;
            }

            if (_ollamaBaseURL?.Text != _originalOllamaBaseUrl)
            {
                _logger.LogDebug("ConfigurationOverlayWidget", $"Ollama BaseUrl changed: '{_originalOllamaBaseUrl}' -> '{_ollamaBaseURL?.Text}'");
                return true;
            }

            // Check Whisper changes
            var currentWhisperModel = GetSelectedWhisperModel();
            if (currentWhisperModel != _originalWhisperModel)
            {
                _logger.LogDebug("ConfigurationOverlayWidget", $"Whisper Model changed: '{_originalWhisperModel}' -> '{currentWhisperModel}'");
                return true;
            }

            if (_translateToEnglish?.IsChecked != _originalTranslateToEnglish)
            {
                _logger.LogDebug("ConfigurationOverlayWidget", $"TranslateToEnglish changed: '{_originalTranslateToEnglish}' -> '{_translateToEnglish?.IsChecked}'");
                return true;
            }

            // Check TTS changes
            var currentTtsProvider = GetSelectedTtsProvider();
            if (currentTtsProvider != _originalTtsProvider)
            {
                _logger.LogDebug("ConfigurationOverlayWidget", $"TTS Provider changed: '{_originalTtsProvider}' -> '{currentTtsProvider}'");
                return true;
            }

            if (_voiceTextBox?.Text != _originalEdgeVoice)
            {
                _logger.LogDebug("ConfigurationOverlayWidget", $"Edge Voice changed: '{_originalEdgeVoice}' -> '{_voiceTextBox?.Text}'");
                return true;
            }

            if (_elevenLabsAPIKey?.Text != _originalElevenLabsApiKey)
            {
                _logger.LogDebug("ConfigurationOverlayWidget", $"ElevenLabs API Key changed");
                return true;
            }

            if (_elevenLabsVoiceID?.Text != _originalElevenLabsVoiceId)
            {
                _logger.LogDebug("ConfigurationOverlayWidget", $"ElevenLabs Voice ID changed: '{_originalElevenLabsVoiceId}' -> '{_elevenLabsVoiceID?.Text}'");
                return true;
            }

            if (_modelIDTextBox?.Text != _originalElevenLabsModelId)
            {
                _logger.LogDebug("ConfigurationOverlayWidget", $"ElevenLabs Model ID changed: '{_originalElevenLabsModelId}' -> '{_modelIDTextBox?.Text}'");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the currently selected Whisper model from the button states.
        /// </summary>
        private string GetSelectedWhisperModel()
        {
            var redBrush = Brushes.Red;

            if (_whisperTinyButton?.Background == redBrush) return "tiny";
            if (_whisperBaseButton?.Background == redBrush) return "base";
            if (_whisperSmallButton?.Background == redBrush) return "small";
            if (_whisperMedButton?.Background == redBrush) return "medium";
            if (_whisperLargeButton?.Background == redBrush) return "large";
            if (_whisperLargeV2Button?.Background == redBrush) return "large-v2";

            return "small"; // default
        }

        /// <summary>
        /// Gets the currently selected TTS provider from the button states.
        /// </summary>
        private string GetSelectedTtsProvider()
        {
            var redBrush = Brushes.Red;

            if (_elevenLabsButton?.Background == redBrush) return "ElevenLabs";
            if (_edgeButton?.Background == redBrush) return "EdgeTTS";

            return "EdgeTTS"; // default
        }

        /// <summary>
        /// Saves the current settings to appsettings.json file.
        /// </summary>
        private async Task SaveSettingsAsync()
        {
            var appSettingsPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            _logger.LogInfo("ConfigurationOverlayWidget", $"Loading appsettings.json from: {appSettingsPath}");

            if (!File.Exists(appSettingsPath))
            {
                _logger.LogError("ConfigurationOverlayWidget", $"appsettings.json not found at: {appSettingsPath}");
                return;
            }

            // Read the existing JSON
            var jsonText = await File.ReadAllTextAsync(appSettingsPath);
            var jsonDoc = System.Text.Json.JsonDocument.Parse(jsonText);
            var root = jsonDoc.RootElement.Clone();

            // Build the updated JSON using System.Text.Json
            using var stream = new MemoryStream();
            using (var writer = new System.Text.Json.Utf8JsonWriter(stream, new System.Text.Json.JsonWriterOptions { Indented = true }))
            {
                writer.WriteStartObject();

                // Copy all existing properties and update only the changed ones
                foreach (var property in root.EnumerateObject())
                {
                    if (property.Name == "Ollama")
                    {
                        WriteOllamaSection(writer, property.Value);
                    }
                    else if (property.Name == "Whisper")
                    {
                        WriteWhisperSection(writer, property.Value);
                    }
                    else if (property.Name == "TTS")
                    {
                        WriteTtsSection(writer, property.Value);
                    }
                    else if (property.Name == "VoicePipeline")
                    {
                        WriteVoicePipelineSection(writer, property.Value);
                    }
                    else
                    {
                        // Copy unchanged sections as-is
                        property.WriteTo(writer);
                    }
                }

                writer.WriteEndObject();
            }

            // Write back to file
            var updatedJson = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            await File.WriteAllTextAsync(appSettingsPath, updatedJson);

            _logger.LogInfo("ConfigurationOverlayWidget", "appsettings.json updated successfully");
        }

        private void WriteOllamaSection(System.Text.Json.Utf8JsonWriter writer, System.Text.Json.JsonElement existing)
        {
            writer.WriteStartObject("Ollama");

            foreach (var property in existing.EnumerateObject())
            {
                if (property.Name == "DefaultModel")
                {
                    writer.WriteString("DefaultModel", _ollamaModelTextBox?.Text ?? "qwen2.5:7b");
                }
                else if (property.Name == "BaseUrl")
                {
                    writer.WriteString("BaseUrl", _ollamaBaseURL?.Text ?? "http://localhost:11434");
                }
                else
                {
                    property.WriteTo(writer);
                }
            }

            writer.WriteEndObject();
        }

        private void WriteWhisperSection(System.Text.Json.Utf8JsonWriter writer, System.Text.Json.JsonElement existing)
        {
            writer.WriteStartObject("Whisper");

            foreach (var property in existing.EnumerateObject())
            {
                if (property.Name == "ModelName")
                {
                    writer.WriteString("ModelName", GetSelectedWhisperModel());
                }
                else if (property.Name == "TranslateToEnglish")
                {
                    writer.WriteBoolean("TranslateToEnglish", _translateToEnglish?.IsChecked ?? false);
                }
                else
                {
                    property.WriteTo(writer);
                }
            }

            writer.WriteEndObject();
        }

        private void WriteTtsSection(System.Text.Json.Utf8JsonWriter writer, System.Text.Json.JsonElement existing)
        {
            writer.WriteStartObject("TTS");

            foreach (var property in existing.EnumerateObject())
            {
                if (property.Name == "Provider")
                {
                    writer.WriteString("Provider", GetSelectedTtsProvider());
                }
                else if (property.Name == "EdgeTTS")
                {
                    WriteEdgeTtsSection(writer, property.Value);
                }
                else if (property.Name == "ElevenLabs")
                {
                    WriteElevenLabsSection(writer, property.Value);
                }
                else
                {
                    property.WriteTo(writer);
                }
            }

            writer.WriteEndObject();
        }

        private void WriteEdgeTtsSection(System.Text.Json.Utf8JsonWriter writer, System.Text.Json.JsonElement existing)
        {
            writer.WriteStartObject("EdgeTTS");

            foreach (var property in existing.EnumerateObject())
            {
                if (property.Name == "Voice")
                {
                    writer.WriteString("Voice", _voiceTextBox?.Text ?? "en-GB-RyanNeural");
                }
                else
                {
                    property.WriteTo(writer);
                }
            }

            writer.WriteEndObject();
        }

        private void WriteElevenLabsSection(System.Text.Json.Utf8JsonWriter writer, System.Text.Json.JsonElement existing)
        {
            writer.WriteStartObject("ElevenLabs");

            foreach (var property in existing.EnumerateObject())
            {
                if (property.Name == "ApiKey")
                {
                    writer.WriteString("ApiKey", _elevenLabsAPIKey?.Text ?? "");
                }
                else if (property.Name == "VoiceId")
                {
                    writer.WriteString("VoiceId", _elevenLabsVoiceID?.Text ?? "");
                }
                else if (property.Name == "ModelId")
                {
                    writer.WriteString("ModelId", _modelIDTextBox?.Text ?? "eleven_multilingual_v2");
                }
                else
                {
                    property.WriteTo(writer);
                }
            }

            writer.WriteEndObject();
        }

        /// <summary>
        /// Writes the VoicePipeline section to JSON, updating the TtsProvider to match the TTS Provider.
        /// </summary>
        private void WriteVoicePipelineSection(System.Text.Json.Utf8JsonWriter writer, System.Text.Json.JsonElement existing)
        {
            writer.WriteStartObject("VoicePipeline");

            var currentTtsProvider = GetSelectedTtsProvider();

            foreach (var property in existing.EnumerateObject())
            {
                if (property.Name == "TtsProvider")
                {
                    writer.WriteString("TtsProvider", currentTtsProvider);
                }
                else
                {
                    property.WriteTo(writer);
                }
            }

            writer.WriteEndObject();
        }

        #endregion
    }
}

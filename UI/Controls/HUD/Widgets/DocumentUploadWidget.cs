using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using UI.Controls.HUD.Factories;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Models;
using UI.Controls.HUD.Widgets;

namespace Jarvis.UI.Controls.HUD.Widgets
{
    /// <summary>
    /// Provides document upload functionality for the HUD.
    /// Allows users to select and upload documents for processing.
    /// </summary>
    public sealed class DocumentUploadWidget : HudWidgetBase
    {
        // ============================================================
        // PRIVATE FIELDS
        // ============================================================

        private Canvas? _root;
        private Button? _uploadButton;
        private TextBlock? _statusText;
        private TextBlock? _fileNameText;
        private readonly HudPanelFactory _panelFactory;
        private readonly ILogService _logger;

        private string? _uploadedFilePath;
        private string? _uploadedFileContent;


        // ============================================================
        // EVENTS
        // ============================================================

        /// <summary>
        /// Raised when a document is successfully uploaded.
        /// </summary>
        public event EventHandler<DocumentUploadedEventArgs>? DocumentUploaded;


        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public DocumentUploadWidget(
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
            _panelFactory = new HudPanelFactory();
            _logger?.LogBackground("DocumentUploadWidget", "Widget created and initialized");
        }


        // ============================================================
        // CREATE VISUAL
        // ============================================================

        protected override FrameworkElement CreateVisual()
        {
            _logger?.LogBackground("DocumentUploadWidget", $"Creating visual - Size: {Configuration.Size.Width}x{Configuration.Size.Height}, Visible: {Configuration.Visible}");

            // --------------------------------------------------------
            // ROOT CANVAS
            // --------------------------------------------------------

            _root = new Canvas
            {
                Width = Configuration.Size.Width,
                Height = Configuration.Size.Height
            };


            // --------------------------------------------------------
            // MAIN PANEL
            // --------------------------------------------------------

            Canvas panel = _panelFactory.CreatePanel(
                new Rect(
                    0,
                    0,
                    Configuration.Size.Width,
                    Configuration.Size.Height),
                Theme);

            _root.Children.Add(panel);


            // --------------------------------------------------------
            // TITLE TEXT
            // --------------------------------------------------------

            var titleText = new TextBlock
            {
                Text = "DOCUMENT UPLOAD",
                FontFamily = Theme.AccentFontFamily,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Theme.PrimaryBrush,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            Canvas.SetLeft(titleText, 15);
            Canvas.SetTop(titleText, 10);
            _root.Children.Add(titleText);


            // --------------------------------------------------------
            // UPLOAD BUTTON
            // --------------------------------------------------------

            _uploadButton = new Button
            {
                Content = "SELECT FILE",
                Width = 150,
                Height = 35,
                Background = new SolidColorBrush(Color.FromArgb(80, 0, 255, 255)),
                Foreground = Theme.PrimaryBrush,
                BorderBrush = Theme.PrimaryBrush,
                BorderThickness = new Thickness(1),
                FontFamily = Theme.BodyFontFamily,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Cursor = Cursors.Hand
            };

            _uploadButton.Click += UploadButton_Click;

            Canvas.SetLeft(_uploadButton, (Configuration.Size.Width - 150) / 2);
            Canvas.SetTop(_uploadButton, 40);
            _root.Children.Add(_uploadButton);


            // --------------------------------------------------------
            // FILE NAME TEXT
            // --------------------------------------------------------

            _fileNameText = new TextBlock
            {
                Text = "No file selected",
                FontFamily = Theme.BodyFontFamily,
                FontSize = 11,
                Foreground = Brushes.LightGray,
                TextAlignment = TextAlignment.Center,
                Width = Configuration.Size.Width - 30,
                TextWrapping = TextWrapping.Wrap
            };

            Canvas.SetLeft(_fileNameText, 15);
            Canvas.SetTop(_fileNameText, 85);
            _root.Children.Add(_fileNameText);


            // --------------------------------------------------------
            // STATUS TEXT
            // --------------------------------------------------------

            _statusText = new TextBlock
            {
                Text = "READY",
                FontFamily = new FontFamily("Consolas"),
                FontSize = 10,
                Foreground = Theme.PrimaryBrush
            };

            Canvas.SetRight(_statusText, 15);
            Canvas.SetBottom(_statusText, 10);
            _root.Children.Add(_statusText);


            return _root;
        }


        // ============================================================
        // EVENT HANDLERS
        // ============================================================

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _logger?.LogBackground("DocumentUploadWidget", "Upload button clicked");

                // Create OpenFileDialog
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Select a document",
                    Filter = "Text Files (*.txt)|*.txt|" +
                            "Markdown Files (*.md)|*.md|" +
                            "All Files (*.*)|*.*",
                    FilterIndex = 1,
                    Multiselect = false
                };

                _logger?.LogBackground("DocumentUploadWidget", "Showing file dialog...");

                // Show dialog
                bool? result = openFileDialog.ShowDialog();

                _logger?.LogBackground("DocumentUploadWidget", $"File dialog result: {result}");

                if (result == true)
                {
                    UpdateStatus("LOADING...");

                    string filePath = openFileDialog.FileName;
                    string fileName = Path.GetFileName(filePath);

                    _logger?.LogBackground("DocumentUploadWidget", $"Reading file: {fileName}");

                    // Read file content
                    string content = await File.ReadAllTextAsync(filePath);

                    _logger?.LogBackground("DocumentUploadWidget", $"File read successfully: {content.Length} characters");

                    // Store the uploaded file info
                    _uploadedFilePath = filePath;
                    _uploadedFileContent = content;

                    // Update UI
                    if (_fileNameText != null)
                    {
                        _fileNameText.Text = $"File: {fileName}\nSize: {FormatFileSize(content.Length)}";
                        _fileNameText.Foreground = Brushes.White;
                    }

                    UpdateStatus("READY");

                    _logger?.LogBackground("DocumentUploadWidget", $"About to raise DocumentUploaded event for: {fileName}");

                    // Raise event
                    DocumentUploaded?.Invoke(this, new DocumentUploadedEventArgs
                    {
                        FilePath = filePath,
                        FileName = fileName,
                        Content = content
                    });

                    _logger?.LogBackground("DocumentUploadWidget", $"DocumentUploaded event raised for: {fileName}");
                }
                else
                {
                    _logger?.LogBackground("DocumentUploadWidget", "File selection cancelled by user");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus("ERROR");
                _logger?.LogError("DocumentUploadWidget", $"Failed to upload document: {ex.Message}\nStack: {ex.StackTrace}");

                if (_fileNameText != null)
                {
                    _fileNameText.Text = "Failed to load file";
                    _fileNameText.Foreground = Brushes.Red;
                }
            }
        }


        // ============================================================
        // PUBLIC METHODS
        // ============================================================

        /// <summary>
        /// Gets the currently uploaded file path.
        /// </summary>
        public string? GetUploadedFilePath() => _uploadedFilePath;

        /// <summary>
        /// Gets the currently uploaded file content.
        /// </summary>
        public string? GetUploadedFileContent() => _uploadedFileContent;

        /// <summary>
        /// Clears the uploaded file.
        /// </summary>
        public void ClearUploadedFile()
        {
            _uploadedFilePath = null;
            _uploadedFileContent = null;

            if (_fileNameText != null)
            {
                _fileNameText.Text = "No file selected";
                _fileNameText.Foreground = Brushes.LightGray;
            }

            UpdateStatus("READY");
        }


        // ============================================================
        // PRIVATE METHODS
        // ============================================================

        private void UpdateStatus(string status)
        {
            if (_statusText != null)
            {
                _statusText.Text = status;
            }
        }

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }


        // ============================================================
        // DISPOSE
        // ============================================================

        public override void Dispose()
        {
            if (_uploadButton != null)
            {
                _uploadButton.Click -= UploadButton_Click;
            }

            _uploadedFilePath = null;
            _uploadedFileContent = null;

            base.Dispose();
        }
    }


    // ============================================================
    // EVENT ARGS
    // ============================================================

    /// <summary>
    /// Event arguments for document upload events.
    /// </summary>
    public sealed class DocumentUploadedEventArgs : EventArgs
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}

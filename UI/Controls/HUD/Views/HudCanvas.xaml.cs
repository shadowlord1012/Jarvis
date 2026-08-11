using AI.Interfaces;
using Jarvis.AI.Interfaces;
using Jarvis.AI.Tool;
using Jarvis.AI.Tool.DocumentProcessing;
using Jarvis.AI.Services;
using Jarvis.UI.Controls.HUD.Models;
using Jarvis.UI.Controls.HUD.Widgets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Managers;
using UI.Controls.HUD.Models;
using UI.Controls.HUD.Renderers;
using UI.Controls.HUD.Renderers.HudRenderers;
using UI.Controls.HUD.Renderers.ReactorRenderers;
using UI.Controls.HUD.Widgets;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace UI.Controls.HUD
{
    /// <summary>
    /// Interaction logic for HudCanvas.xaml
    /// </summary>
    public partial class HudCanvas : UserControl
    {
        private HudLayerManager? _layerManager;
        private HudRenderer? _renderer;
        private ReactorManager _reactorManager;
        private ReactorRenderer _reactorRenderer;
        private HudWidgetManager _hudWidgetManager;
        private readonly ILogService _logService;

        private readonly IAIService _aiService;

        private readonly DocumentProcessingTool _documentProcessingTool;

        private readonly IConversationService _conversationService;

        private readonly DocumentContextService _documentContextService;

        private HudInputWidget _inputWidget;

        private AIInputController _aiInputController;

        private AIResponseLogger _aiResponseLogger;

        private WidgetConfigurationManager _widgetConfigurationManager;

        private readonly IConfiguration _configuration;

        private DateTime _lastFrame;

        // Store uploaded document info
        private string? _uploadedDocumentContent;
        private string? _uploadedDocumentFileName;

        public AIInputController AIInputController => _aiInputController;

        public HudCanvas(
            IAIService aiService, 
            ILogService logService, 
            IConfiguration configuration,
            DocumentProcessingTool documentProcessingTool,
            IConversationService conversationService,
            DocumentContextService documentContextService)
        {
            _aiService = aiService;
            _logService = logService;
            _configuration = configuration;
            _documentProcessingTool = documentProcessingTool;
            _conversationService = conversationService;
            _documentContextService = documentContextService;
            _widgetConfigurationManager = new WidgetConfigurationManager(_logService);
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            SizeChanged += OnSizeChanged;

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Start async initialization without blocking
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            //----------------------------------------
            // Create Layer Manager (fast, synchronous)
            //----------------------------------------

            _layerManager = new HudLayerManager();

            _layerManager.Register(HudLayer.Background, BackgroundLayer);
            _layerManager.Register(HudLayer.Grid, GridLayer);
            _layerManager.Register(HudLayer.Glow, GlowLayer);
            _layerManager.Register(HudLayer.Effects, EffectsLayer);
            _layerManager.Register(HudLayer.Reactor, ReactorLayer);
            _layerManager.Register(HudLayer.Overlay, OverlayLayer);
            _layerManager.Register(HudLayer.Plugin, PluginLayer);
            _layerManager.Register(HudLayer.Widgets, WidgetsLayer);
            _layerManager.Register(HudLayer.Input, InputLayer);
            _layerManager.Register(HudLayer.ConfigOverlay, ConfigOverlayLayer);

            //----------------------------------------
            // Create Renderer Manager
            //----------------------------------------

            var theme = new HudTheme();

            _renderer = new HudRenderer(
                _layerManager,
                theme);

            //----------------------------------------
            // Register Renderers
            //----------------------------------------

            //Note to self always make sure that when adding the renderers that they are in a set order

            _renderer.RegisterRenderer(new BackgroundRenderer(_layerManager));
            _renderer.RegisterRenderer(new GridRenderer(_layerManager));
            _renderer.RegisterRenderer(new GlowRenderer(_layerManager, theme));
            _renderer.RegisterRenderer(new HudGlowRenderer(_layerManager));

            //----------------------------------------
            // Reactor Renderers
            //----------------------------------------

            _reactorManager = new ReactorManager(theme);
            _reactorRenderer = new ReactorRenderer(_layerManager, _reactorManager);
            
            //-----------------------------------------
            // Other Renderers
            //-----------------------------------------

            _renderer.RegisterRenderer(new ScanRenderer(_layerManager));            
            _renderer.RegisterRenderer(new HudFrameRenderer(_layerManager));
            _renderer.RegisterRenderer(new StatusRibbonRenderer(_layerManager, theme));

            _inputWidget = new HudInputWidget(_layerManager.GetLayer(HudLayer.Input), theme, _widgetConfigurationManager.GetWidgetConfiguration("Input"), _logService);
            _aiInputController = new AIInputController(_aiService, _inputWidget);

            //-----------------------------------------
            // HUD Widget Manager
            //-----------------------------------------

            _hudWidgetManager = new HudWidgetManager(_layerManager, theme);
            _hudWidgetManager.Register(new ClockWidget(_layerManager.GetLayer(HudLayer.Widgets),theme,_widgetConfigurationManager.GetWidgetConfiguration("Clock")));
            _hudWidgetManager.Register(new SystemStatusWidget(_layerManager.GetLayer(HudLayer.Widgets),theme,_widgetConfigurationManager.GetWidgetConfiguration("SystemStatus"), new SystemStatus(),_logService));
            _hudWidgetManager.Register(new LogWidget(_layerManager.GetLayer(HudLayer.Widgets), theme, _widgetConfigurationManager.GetWidgetConfiguration("Log"), _logService));

            var configWidget = new ConfigurationWidget(_layerManager.GetLayer(HudLayer.Widgets), theme, _widgetConfigurationManager.GetWidgetConfiguration("Configuration"), _logService);
            var configOverlayWidget = new ConfigurationOverlayWidget(_layerManager.GetLayer(HudLayer.ConfigOverlay), theme, _widgetConfigurationManager.GetWidgetConfiguration("ConfigurationOverlay"), _logService, _configuration);
            var docWidget = new DocumentUploadWidget(_layerManager.GetLayer(HudLayer.Widgets), theme, _widgetConfigurationManager.GetWidgetConfiguration("Documentation"), _logService);

            // Wire up the configuration button to show/hide the overlay
            configWidget.ButtonClicked += (sender, e) =>
            {
                if (configOverlayWidget.IsVisible)
                {
                    configOverlayWidget.Deactivate();
                }
                else
                {
                    configOverlayWidget.Activate();
                }
            };

            // Wire up document upload widget
            docWidget.DocumentUploaded += async (sender, e) =>
            {
                try
                {
                    _logService.LogInfo("HudCanvas", "=== DocumentUploaded EVENT RECEIVED ===");
                    _logService.LogInfo("HudCanvas", $"Document uploaded: {e.FileName} ({e.Content.Length} chars)");

                    // Store the document content and filename
                    _uploadedDocumentContent = e.Content;
                    _uploadedDocumentFileName = e.FileName;

                    // Save to a temporary file for the tool to access
                    string tempPath = Path.Combine(Path.GetTempPath(), "jarvis_uploaded_document.txt");
                    await File.WriteAllTextAsync(tempPath, e.Content);
                    _logService.LogInfo("HudCanvas", $"Document saved to temp file: {tempPath}");

                    // Set the document context in the service (will be picked up by ContextManager)
                    _documentContextService.SetUploadedDocument(tempPath, e.FileName);
                    _logService.LogInfo("HudCanvas", "Document context set in DocumentContextService");

                    // Try to add context to the conversation (non-blocking if DB fails)
                    try
                    {
                        await _conversationService.AddAssistantMessageAsync(
                            $"[SYSTEM: User uploaded document '{e.FileName}' ({e.Content.Length} chars). " +
                            $"File saved to: {tempPath}. When user asks to process it, use document_processing tool with filePath parameter.]");
                    }
                    catch (Exception dbEx)
                    {
                        // Log but don't fail the upload if conversation persistence fails
                        _logService.LogWarning("HudCanvas", $"Could not persist document context to conversation: {dbEx.Message}");
                    }

                    // Create a simple acknowledgment message (no AI inference needed)
                    string responseMessage = $"Document '{e.FileName}' uploaded successfully with {e.Content.Length} characters. What would you like me to do with it? I can summarize it, translate it to another language, or analyze its contents.";

                    // Trigger response started event for voice pipeline
                    _aiInputController.GetType()
                        .GetEvent("ResponseStarted")
                        ?.GetRaiseMethod(true)
                        ?.Invoke(_aiInputController, new object[] { _aiInputController, EventArgs.Empty });

                    // Send the message directly through the response events (bypass AI to avoid tool loops)
                    _aiInputController.GetType()
                        .GetEvent("ResponseChunkReceived")
                        ?.GetRaiseMethod(true)
                        ?.Invoke(_aiInputController, new object[] { _aiInputController, responseMessage });

                    // Fire ResponseCompleted
                    _aiInputController.GetType()
                        .GetEvent("ResponseCompleted")
                        ?.GetRaiseMethod(true)
                        ?.Invoke(_aiInputController, new object[] { _aiInputController, EventArgs.Empty });

                    _logService.LogSuccess("HudCanvas", $"Document '{e.FileName}' loaded and ready for processing");
                }
                catch (Exception ex)
                {
                    _logService.LogError("HudCanvas", $"Error in DocumentUploaded handler: {ex.Message}\nStack: {ex.StackTrace}");
                }
            };

            _hudWidgetManager.Register(configWidget);
            _hudWidgetManager.Register(configOverlayWidget);
            _hudWidgetManager.Register(docWidget);
            _hudWidgetManager.Register(_inputWidget);



            //----------------------------------------
            // AI Response Logger
            //----------------------------------------

            _aiResponseLogger = new AIResponseLogger(_hudWidgetManager.GetWidget<LogWidget>());
            _aiResponseLogger.Connect(_aiInputController);


            //----------------------------------------
            // Initialize
            //----------------------------------------

            // Allow UI to update before starting render loop
            await Dispatcher.InvokeAsync(() => { }, 
                System.Windows.Threading.DispatcherPriority.Render);

            //----------------------------------------
            // Initialize Renderers
            //----------------------------------------

            _renderer.Initialize(1600,900);
            _reactorRenderer.Initialize(1600, 900);
            _hudWidgetManager.Initialize(1600, 900);

            _lastFrame = DateTime.Now;

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //_renderer?.Resize();
            //_reactorManager?.Resize();
        }

        private void CompositionTarget_Rendering(object? sender, EventArgs e)
        {
            if (_renderer == null)
                return;

            var now = DateTime.Now;

            var delta = (now - _lastFrame).TotalSeconds;

            _lastFrame = now;

            foreach(var renderer in _renderer.Renderers)
            {
                renderer.Update(delta);
            }

            _reactorManager.Update(delta);
            _reactorRenderer.Update(delta);
            _hudWidgetManager.Update(delta);
        }
    }
}

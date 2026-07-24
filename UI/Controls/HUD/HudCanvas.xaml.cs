using AI;
using AI.Interfaces;
using Jarvis.UI.Controls.HUD.Models;
using Jarvis.UI.Controls.HUD.Widgets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UI.Controls.HUD.Configurations;
using UI.Controls.HUD.Dashboard;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Managers;
using UI.Controls.HUD.Models;
using UI.Controls.HUD.Renderers;
using UI.Controls.HUD.Renderers.HudRenderers;
using UI.Controls.HUD.Renderers.ReactorRenderers;
using UI.Controls.HUD.Widgets;

namespace UI.Controls.HUD
{
    /// <summary>
    /// Interaction logic for HudCanvas.xaml
    /// </summary>
    public partial class HudCanvas : UserControl
    {
        private HudLayerManager? _layerManager;
        private HudRenderer? _renderer;
        //private DashboardManager _dashboard;
        private ReactorManager _reactorManager;
        private ReactorRenderer _reactorRenderer;
        private HudWidgetManager _hudWidgetManager;
        private readonly ILogService _logService;

        private readonly IAIService _aiService;

        private HudInputWidget _inputWidget;

        private AIInputController _aiInputController;

        private AIResponseLogger _aiResponseLogger;

        private WidgetConfigurationManager _widgetConfigurationManager;

        private DateTime _lastFrame;

        public AIInputController AIInputController => _aiInputController;

        public HudCanvas(IAIService aiService, ILogService logService)
        {
            _aiService = aiService;
            _logService = logService;
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

            //----------------------------------------
            // Create Renderer Manager
            //----------------------------------------

            var theme = new HudTheme();

            _renderer = new HudRenderer(
                _layerManager,
                theme);

            //----------------------------------------
            // Register Widgets
            //----------------------------------------

            //_dashboard = new DashboardManager();

            //----------------------------------------
            // Register Renderers
            //----------------------------------------

            //Note to self always make sure that when adding the renderers that they are in a set order

            _renderer.RegisterRenderer(new BackgroundRenderer(_layerManager));
            _renderer.RegisterRenderer(new GridRenderer(_layerManager));
            _renderer.RegisterRenderer(new GlowRenderer(_layerManager, theme));
            _renderer.RegisterRenderer(new HudGlowRenderer(_layerManager));
            //_renderer.RegisterRenderer(new DashboardPanelRenderer(_layerManager, _dashboard));

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
            _hudWidgetManager.Register(new ConfigurationWidget(_layerManager.GetLayer(HudLayer.Widgets), theme, _widgetConfigurationManager.GetWidgetConfiguration("Configuration"), _logService));
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

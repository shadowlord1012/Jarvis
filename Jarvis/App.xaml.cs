using AI;
using AI.Interfaces;
using AI.Tool.WebSearch;
using Common.Events.Interfaces;
using Jarvis.AI.Interfaces;
using Jarvis.Diagnostics;
using Jarvis.Speech;
using Loader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using UI.Controls.HUD;
using UI.Controls.HUD.Interfaces;

namespace Jarvis
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;
        private StartupStatusService _startupStatusService;
        private ILogService _logger;

        protected override async void OnStartup(StartupEventArgs e)
        {
            var builder = Host.CreateApplicationBuilder();

            BootStrapper.Configure(builder);

            _host = builder.Build();

            // Print configuration diagnostics after building the host
            {
                var logger = _host.Services.GetRequiredService<ILogService>();
                var configDiag = new Jarvis.Diagnostics.ConfigurationDiagnostics(logger);
                configDiag.PrintConfigurationDiagnostics(builder.Configuration);
            }

            // Run diagnostic to verify bridge registration
            BridgeDiagnostic.TestRegistration(_host);

            await _host.StartAsync();

            _startupStatusService = _host.Services.GetRequiredService<StartupStatusService>();
            _logger = _host.Services.GetRequiredService<ILogService>();

            var window = _host.Services.GetRequiredService<MainWindow>();

            // Show window immediately for faster perceived startup
            _startupStatusService.Info("Starting JARVIS...");
            window.Show();

            // Force UI to render before continuing with heavy initialization
            await Application.Current.Dispatcher.InvokeAsync(() => { }, 
                System.Windows.Threading.DispatcherPriority.Render);

            // Connect VoicePipelineBridge to AIInputController after UI is fully loaded
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    var hudCanvas = _host.Services.GetRequiredService<HudCanvas>();
                    var voicePipelineBridge = _host.Services.GetRequiredService<VoicePipelineBridge>();

                    // Wait for HudCanvas to finish initializing (it initializes AIInputController in OnLoaded)
                    // We'll use a small delay to ensure the async initialization completes
                    Task.Delay(1000).ContinueWith(_ =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (hudCanvas.AIInputController != null)
                            {
                                voicePipelineBridge.Connect(hudCanvas.AIInputController);
                                _startupStatusService.Success("Voice pipeline connected");
                            }
                            else
                            {
                                _startupStatusService.Error("AIInputController not ready");
                            }
                        });
                    });
                }
                catch (Exception ex)
                {
                    _startupStatusService.Error($"Failed to connect voice pipeline: {ex.Message}");
                }
            });

            // Run heavy initialization in background
            _ = Task.Run(async () =>
            {
                try
                {
                    _logger.LogInfo("App", "=== Background initialization task STARTED ===");
                    await InitializeBackgroundServicesAsync();
                    _logger.LogInfo("App", "=== Background initialization task COMPLETED ===");
                }
                catch (Exception ex)
                {
                    _logger.LogError("App", "=== BACKGROUND INIT ERROR ===");
                    _logger.LogError("App", $"Error: {ex.Message}");
                    _logger.LogError("App", $"Stack: {ex.StackTrace}");
                    _startupStatusService.Error($"Initialization error: {ex.Message}");
                }
            });

            base.OnStartup(e);
        }

        private async Task InitializeBackgroundServicesAsync()
        {
            _logger.LogInfo("App", "=== InitializeBackgroundServicesAsync STARTED ===");
            _startupStatusService.Info("Initializing background services...");

            // Initialize services that don't block UI
            var aiService = _host.Services.GetRequiredService<IAIService>();
            var loader = _host.Services.GetRequiredService<PluginLoader>();
            var toolRegistry = _host.Services.GetRequiredService<IToolRegistry>();

            // VoicePipelineBridge will be connected to AIInputController later,
            // after HudCanvas creates the AIInputController instance
            _startupStatusService.Info("Voice pipeline bridge ready (will connect after UI initialization)");

            // Register built-in tools
            var webSearchTool = _host.Services.GetRequiredService<WebSearchTool>();
            toolRegistry.Register(webSearchTool);
            _startupStatusService.Info($"  Registered tool: {webSearchTool.Name}");

            var fileProcessingTool = _host.Services.GetRequiredService<AI.Tool.FileProcessing.FileProcessingTool>();
            toolRegistry.Register(fileProcessingTool);
            _startupStatusService.Info($"  Registered tool: {fileProcessingTool.Name}");

            var documentProcessingTool = _host.Services.GetRequiredService<AI.Tool.DocumentProcessing.DocumentProcessingTool>();
            toolRegistry.Register(documentProcessingTool);
            _startupStatusService.Info($"  Registered tool: {documentProcessingTool.Name}");

            var pluginPath = Path.Combine(AppContext.BaseDirectory, "Plugins");

            // Run plugin loading and AI initialization in parallel
            _startupStatusService.Info("Loading AI services and plugins...");

            var tasks = new List<Task>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        // AI service warmup happens in background automatically
                        _startupStatusService.Success("AI service ready");
                    }
                    catch (Exception ex)
                    {
                        _startupStatusService.Error($"AI init failed: {ex.Message}");
                    }
                }),

                Task.Run(async () =>
                {
                    try
                    {
                        _startupStatusService.Info("Loading plugins...");
                        await loader.LoadPluginsAsync(pluginPath);

                        foreach (var plugin in loader.Plugins)
                        {
                            _logger.LogInfo("App", $"Plugin loaded: {plugin.Name}");
                            _startupStatusService.Success($"Loaded {plugin.Name}");

                            // Register tools from the plugin
                            foreach (var tool in plugin.GetTools())
                            {
                                toolRegistry.Register(tool);
                                _startupStatusService.Info($"  Registered tool: {tool.Name}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _startupStatusService.Error($"Plugin loading failed: {ex.Message}");
                    }
                })
            };

            await Task.WhenAll(tasks);

            _startupStatusService.Success("JARVIS ready");
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            try
            {
                _startupStatusService?.Info("Shutting down JARVIS...");

                // Allow services to clean up gracefully
                if (_host is not null)
                {
                    try
                    {
                        // Get AI service and end conversation
                        var aiService = _host.Services.GetService<IAIService>();
                        if (aiService is not null)
                        {
                            await aiService.EndConversationAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError("App", $"Error ending conversation: {ex.Message}");
                    }

                    await _host.StopAsync(TimeSpan.FromSeconds(5));
                    _host.Dispose();
                }

                _startupStatusService?.Success("JARVIS shutdown complete");
            }
            catch (Exception ex)
            {
                _logger?.LogError("App", $"Error during shutdown: {ex.Message}");
            }

            base.OnExit(e);
        }


    }

}

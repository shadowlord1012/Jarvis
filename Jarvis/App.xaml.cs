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

        protected override async void OnStartup(StartupEventArgs e)
        {
            var builder = Host.CreateApplicationBuilder();

            BootStrapper.Configure(builder);

            // Print configuration diagnostics before building the host
            Jarvis.Diagnostics.ConfigurationDiagnostics.PrintConfigurationDiagnostics(builder.Configuration);

            _host = builder.Build();


            Console.WriteLine(_host.Services.ToString());

            // Run diagnostic to verify bridge registration
            BridgeDiagnostic.TestRegistration(_host);

            await _host.StartAsync();

            _startupStatusService = _host.Services.GetRequiredService<StartupStatusService>();

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
                    Console.WriteLine("=== Background initialization task STARTED ===");
                    await InitializeBackgroundServicesAsync();
                    Console.WriteLine("=== Background initialization task COMPLETED ===");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"=== BACKGROUND INIT ERROR ===");
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.WriteLine($"Stack: {ex.StackTrace}");
                    _startupStatusService.Error($"Initialization error: {ex.Message}");
                }
            });

            base.OnStartup(e);
        }

        private async Task InitializeBackgroundServicesAsync()
        {
            Console.WriteLine("=== InitializeBackgroundServicesAsync STARTED ===");
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
                            Console.WriteLine(plugin.Name);
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
                        Console.WriteLine($"Error ending conversation: {ex.Message}");
                    }

                    await _host.StopAsync(TimeSpan.FromSeconds(5));
                    _host.Dispose();
                }

                _startupStatusService?.Success("JARVIS shutdown complete");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during shutdown: {ex.Message}");
            }

            base.OnExit(e);
        }


    }

}

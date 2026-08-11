using AI;
using AI.Interfaces;
using AI.Tool.WebSearch;
using Jarvis.AI.Conversation;
using Jarvis.AI.Factories;
using Jarvis.AI.Interfaces;
using Jarvis.AI.Manager;
using Jarvis.AI.Memory;
using Jarvis.AI.Ollama;
using Jarvis.AI.Options;
using Jarvis.AI.Prompt;
using Jarvis.AI.Providers;
using Jarvis.AI.Services;
using Jarvis.AI.Storage.MariaDB;
using Jarvis.AI.Tool;
using Jarvis.AI.Tool.FileProcessing;
using Jarvis.AI.Tool.DocumentProcessing;
using Jarvis.Audio;
using Jarvis.Interfaces;
using Jarvis.Speech;
using Jarvis.UI.Controls.HUD.Models;
using Jarvis.UI.Controls.HUD.Widgets;
using Loader;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UI.Controls.HUD;
using UI.Controls.HUD.Configurations;
using UI.Controls.HUD.Factories;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Managers;
using UI.Controls.HUD.Models;
using UI.Controls.HUD.Renderers;
using UI.Controls.HUD.Renderers.HudRenderers;

namespace Jarvis
{
    public static class BootStrapper
    {

        public static void Configure(HostApplicationBuilder builder)
        {
            // Add appsettings.json configuration
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

            ConfigureConfiguration(builder);

            ConfigureServices(builder);
        }

        private static void ConfigureConfiguration(HostApplicationBuilder builder)
        {
            builder.Services.Configure<Common.Configuration.AppSettings>(builder.Configuration);
            builder.Services.Configure<MariaDbOptions>(builder.Configuration.GetSection("MariaDB"));
            builder.Services.Configure<OllamaOptions>(builder.Configuration.GetSection("Ollama"));
            builder.Services.Configure<ContextOptions>(builder.Configuration.GetSection("AI:Context"));
            builder.Services.Configure<TtsOptions>(builder.Configuration.GetSection("TTS"));
            builder.Services.Configure<VoicePipelineOptions>(builder.Configuration.GetSection("VoicePipeline"));
            builder.Services.Configure<WhisperOptions>(builder.Configuration.GetSection("Whisper"));
        }

        private static void ConfigureServices(HostApplicationBuilder builder)
        {
            builder.Services.AddJarvis();
            builder.Services.AddSingleton<StartupStatusService>();
            builder.Services.AddSingleton<PluginLoader>();
            builder.Services.AddSingleton<MainWindow>();
            builder.Services.AddSingleton<HudPanelFactory>();
            builder.Services.AddSingleton<HudShapeFactory>();
            builder.Services.AddSingleton<HudPanelManager>();
            builder.Services.AddSingleton<HudRenderer>();
            builder.Services.AddSingleton<HudConfigurationLoader>();
            builder.Services.AddSingleton<HudLayoutManager>();
            builder.Services.AddSingleton<HudEditorManager>();
            builder.Services.AddSingleton<HudDragBehavior>();
            builder.Services.AddSingleton<HudLayerManager>();
            builder.Services.AddSingleton<HudTheme>();
            builder.Services.AddSingleton<ReactorManager>();
            builder.Services.AddSingleton(new ConversationOptions
            {
                MaximumMessages = 100,
                TrimOldMessages = true,
                IncludeSystemPrompt = true
            });
            builder.Services.AddSingleton<IAIStateManager, AIStateManager>();
            builder.Services.AddSingleton(new PromptOptions
            {
                IncludeSystemPrompt = true,
                IncludeConversation = true,
                IncludeMemory = true,
                IncludeToolDefinitions = true,
                MaximumConversationMessages = 20
            });
            builder.Services.AddSingleton<SystemPromptProvider>();
            builder.Services.AddSingleton<IPromptBuilder, PromptBuilder>();
            builder.Services.AddSingleton(new ContextOptions
            {
                MaximumConversationMessages = 20,
                MaximumMemories = 10,
                IncludeConversation = true,
                IncludeMemory = true,
                IncludeTools = true,
                IncludeSystemInformation = true
            });
            builder.Services.AddSingleton<IContextManager, ContextManager>();
            builder.Services.AddSingleton(new MemoryOptions
            {
                MaximumItems = 10000,
                EnableAutomaticCleanup = true
            });
            builder.Services.AddSingleton<IMemoryService, MemoryService>();
            builder.Services.AddSingleton(sp =>
                sp.GetRequiredService<
                    Microsoft.Extensions.Options.IOptions<MariaDbOptions>>().Value);
            builder.Services.AddSingleton<IMemorySqlProvider, MemorySqlProvider>();
            builder.Services.AddSingleton<IConversationService,ConversationService>();
            builder.Services.AddSingleton<IAIService,AIService>();
            builder.Services.AddSingleton<ILogService, LogService>();
            builder.Services.AddSingleton<MemoryCache>();
            builder.Services.AddSingleton<IDatabaseConnectionFactory,DatabaseConnectionFactory>();
            builder.Services.AddSingleton<IMemoryRepository,MariaDbMemoryRepository>();
            builder.Services.AddSingleton<IToolRegistry, ToolRegistry>();
            builder.Services.AddSingleton<IToolExecutor, ToolExecutor>();

            // WebSearch tool dependencies
            builder.Services.AddHttpClient<IDuckDuckGoSearchClient, DuckDuckGoSearchClient>();
            builder.Services.AddSingleton<IWebSearchService, WebSearchService>();
            builder.Services.AddSingleton<WebSearchTool>();

            // File processing tool
            builder.Services.AddSingleton<FileProcessingTool>();

            // Document processing tool
            builder.Services.AddSingleton<DocumentProcessingTool>();

            // Document context service
            builder.Services.AddSingleton<DocumentContextService>();

            builder.Services.AddHttpClient<ILLMProvider, OllamaProvider>(
                (serviceProvider, client) =>
                {
                    var options =
                        serviceProvider
                            .GetRequiredService<
                                IOptions<OllamaOptions>>()
                            .Value;

                    client.BaseAddress =
                        new Uri(options.BaseUrl);

                    client.Timeout =
                        TimeSpan.FromSeconds(
                            options.TimeoutSeconds);

                    client.DefaultRequestHeaders
                        .Accept.Clear();

                    client.DefaultRequestHeaders
                        .Accept.Add(
                            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(
                                "application/json"));
                });

            // TTS Provider Services
            builder.Services.AddHttpClient<ElevenLabsTtsProvider>();
            builder.Services.AddSingleton<ITtsProvider, EdgeTtsProvider>();
            builder.Services.AddSingleton<ITtsProvider, ElevenLabsTtsProvider>();
            builder.Services.AddSingleton<TtsProviderFactory>();

            // Speech Services  
            // Register WhisperModelDownloader with HttpClient support
            builder.Services.AddHttpClient<WhisperModelDownloader>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(30));

            builder.Services.AddSingleton<IWhisperService, WhisperService>();
            builder.Services.AddSingleton<IAudioService, AudioService>();
            builder.Services.AddSingleton<IAudioPlaybackQueue, AudioPlaybackQueue>();
            builder.Services.AddSingleton<IVoicePipelineService, VoicePipelineService>();

            // UI Services
            builder.Services.AddSingleton<HudCanvas>();

            // Bridge (will be connected to AIInputController later, after UI is created)
            builder.Services.AddSingleton<VoicePipelineBridge>();
        }
    }
}

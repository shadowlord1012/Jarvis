using Jarvis.Interfaces;
using Jarvis.Speech;
using Jarvis.UI.Controls.HUD.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using UI.Controls.HUD.Interfaces;

namespace Jarvis.Speech
{
    /// <summary>
    /// Bridges AI Input Controller with Voice Pipeline to enable TTS
    /// </summary>
    public sealed class VoicePipelineBridge
    {
        private AIInputController? _aiInputController;
        private readonly IVoicePipelineService _voicePipelineService;
        private readonly ILogger<VoicePipelineBridge> _logger;
        private readonly ILogService _logService;
        private readonly VoicePipelineOptions _options;
        private readonly SemaphoreSlim _processingLock = new SemaphoreSlim(1, 1);

        public VoicePipelineBridge(
            IVoicePipelineService voicePipelineService,
            IOptions<VoicePipelineOptions> options,
            ILogger<VoicePipelineBridge> logger,
            ILogService logService)
        {
            _voicePipelineService = voicePipelineService;
            _options = options.Value;
            _logger = logger;
            _logService = logService;

            _logger.LogInformation(
                "VoicePipelineBridge constructor called. Enabled={Enabled}, EnableTts={EnableTts}",
                _options.Enabled,
                _options.EnableTts);

            _logService.LogDebug(
                $"VoicePipelineBridge initialized. Enabled={_options.Enabled}, EnableTts={_options.EnableTts}",
                "VoicePipelineBridge");
        }

        /// <summary>
        /// Connects the bridge to an AIInputController instance
        /// </summary>
        public void Connect(AIInputController aiInputController)
        {
            if (_aiInputController != null)
            {
                // Disconnect from previous controller
                _aiInputController.InputSubmittedEvent -= OnInputSubmitted;
            }

            _aiInputController = aiInputController;

            // Subscribe to input events
            _aiInputController.InputSubmittedEvent += OnInputSubmitted;

            _logger.LogInformation(
                "VoicePipelineBridge connected to AIInputController");

            _logService.LogDebug("VoicePipelineBridge",
                "VoicePipelineBridge connected to AIInputController"
                );
        }

        private async void OnInputSubmitted(object? sender, string input)
        {
            if (!_options.Enabled)
            {
                _logger.LogDebug("Voice pipeline is disabled");
                return;
            }

            // Prevent concurrent processing - use TryWait to avoid blocking
            if (!await _processingLock.WaitAsync(0))
            {
                _logger.LogWarning("Duplicate input submission detected - already processing. Ignoring duplicate.");
                _logService.LogWarning("VoicePipelineBridge", "Duplicate input submission ignored - already processing");
                return;
            }

            try
            {
                _logger.LogInformation(
                    "Voice pipeline processing input: {Input}",
                    input);

                _logService.LogDebug("VoicePipelineBridge",
                    $"Voice pipeline processing input: {input}");

                await _voicePipelineService.ProcessTextAsync(
                    input,
                    CancellationToken.None);

                _logger.LogInformation(
                    "Voice pipeline completed processing");

                _logService.LogDebug("VoicePipelineBridge",
                    "Voice pipeline completed processing");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Voice pipeline failed to process input");

                _logService.LogError("VoicePipelineBridge",
                    $"Voice pipeline failed: {ex.Message}");
            }
            finally
            {
                _processingLock.Release();
            }
        }
    }
}

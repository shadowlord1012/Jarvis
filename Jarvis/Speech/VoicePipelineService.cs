using AI.Interfaces;
using Jarvis.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Interfaces;

namespace Jarvis.Speech
{
    public sealed class VoicePipelineService
    : IVoicePipelineService
    {
        private readonly IWhisperService _whisper;
        private readonly IAIService _aiService;
        private readonly TtsProviderFactory _ttsFactory;
        private readonly IAudioService _audioService;
        private readonly ILogService _logService;

        private readonly VoicePipelineOptions _options;

        private readonly ILogger<VoicePipelineService> _logger;

        public VoicePipelineService(
            IWhisperService whisper,
            IAIService aiService,
            TtsProviderFactory ttsFactory,
            IAudioService audioService,
            ILogService logService,
            IOptions<VoicePipelineOptions> options,
            ILogger<VoicePipelineService> logger)
        {
            _whisper = whisper;
            _aiService = aiService;
            _ttsFactory = ttsFactory;
            _audioService = audioService;
            _logService = logService;

            _options = options.Value;

            _logger = logger;
        }

        public async Task ProcessAudioAsync(
            string audioFilePath,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logService.LogInfo(
                    "VoicePipeline",
                    "Starting Whisper transcription.");

                var transcription =
                    await _whisper.TranscribeAsync(
                        audioFilePath,
                        cancellationToken);

                if (transcription.IsEmpty)
                {
                    _logService.LogInfo(
                        "VoicePipeline",
                        "No speech detected.");

                    return;
                }

                _logService.LogInfo(
                    "VoicePipeline",
                    $"User said: {transcription.Text}");

                await ProcessTextAsync(
                    transcription.Text,
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logService.LogInfo(
                    "VoicePipeline",
                    "Cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Voice pipeline failed.");

                _logService.LogError(
                    "VoicePipeline",
                    $"Failed: {ex.Message}");

                throw;
            }
        }

        public async Task ProcessTextAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var ttsProvider =
                _ttsFactory.GetProvider(
                    _options.TtsProvider);

            var sentenceBuffer =
                new TtsSentenceBuffer();

            await foreach (
                var chunk in _aiService
                    .ProcessStreamingAsync(
                        text,
                        cancellationToken)
                    .WithCancellation(
                        cancellationToken))
            {
                if (string.IsNullOrWhiteSpace(chunk.Content))
                {
                    continue;
                }

                sentenceBuffer.Append(chunk.Content);

                while (
                    sentenceBuffer.TryGetSentence(
                        out var sentence))
                {
                    await SpeakAsync(
                        ttsProvider,
                        sentence,
                        cancellationToken);
                }
            }

            var remaining =
                sentenceBuffer.Flush();

            if (!string.IsNullOrWhiteSpace(remaining))
            {
                await SpeakAsync(
                    ttsProvider,
                    remaining,
                    cancellationToken);
            }
        }

        private async Task SpeakAsync(
            ITtsProvider provider,
            string text,
            CancellationToken cancellationToken)
        {
            if (!_options.EnableTts)
            {
                return;
            }

            var audio =
                await provider.SynthesizeAsync(
                    text,
                    cancellationToken);

            if (audio.Length == 0)
            {
                return;
            }

            await _audioService.PlayAsync(
                audio,
                cancellationToken);
        }
    }
}

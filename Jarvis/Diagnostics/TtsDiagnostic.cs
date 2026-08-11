using Jarvis.Interfaces;
using Jarvis.Speech;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using UI.Controls.HUD.Interfaces;

namespace Jarvis.Diagnostics
{
    /// <summary>
    /// Diagnostic tool to test TTS and Audio playback
    /// </summary>
    public sealed class TtsDiagnostic
    {
        private readonly TtsProviderFactory _ttsFactory;
        private readonly IAudioService _audioService;
        private readonly ILogService _logger;
        private readonly VoicePipelineOptions _voiceOptions;
        private readonly TtsOptions _ttsOptions;

        public TtsDiagnostic(
            TtsProviderFactory ttsFactory,
            IAudioService audioService,
            IOptions<VoicePipelineOptions> voiceOptions,
            IOptions<TtsOptions> ttsOptions,
            ILogService logger)
        {
            _ttsFactory = ttsFactory;
            _audioService = audioService;
            _voiceOptions = voiceOptions.Value;
            _ttsOptions = ttsOptions.Value;
            _logger = logger;
        }

        public async Task RunDiagnosticsAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInfo("TtsDiagnostic", "=== TTS DIAGNOSTICS START ===");

            // Check configuration
            _logger.LogInfo("TtsDiagnostic", $"Voice Pipeline Enabled: {_voiceOptions.Enabled}");
            _logger.LogInfo("TtsDiagnostic", $"TTS Enabled: {_voiceOptions.EnableTts}");
            _logger.LogInfo("TtsDiagnostic", $"TTS Provider: {_voiceOptions.TtsProvider}");
            _logger.LogInfo("TtsDiagnostic", $"Configured Provider: {_ttsOptions.Provider}");

            try
            {
                // Get the TTS provider
                _logger.LogInfo("TtsDiagnostic", $"Getting TTS provider: {_voiceOptions.TtsProvider}");
                var provider = _ttsFactory.GetProvider(_voiceOptions.TtsProvider);
                _logger.LogInfo("TtsDiagnostic", $"TTS Provider loaded: {provider.ProviderName}");

                // Test TTS synthesis
                var testText = "This is a test of the text to speech system.";
                _logger.LogInfo("TtsDiagnostic", $"Synthesizing test text: {testText}");

                var audioData = await provider.SynthesizeAsync(testText, cancellationToken);

                _logger.LogInfo("TtsDiagnostic", $"Audio data generated. Size: {audioData.Length} bytes");

                if (audioData.Length == 0)
                {
                    _logger.LogError("TtsDiagnostic", "TTS provider returned empty audio data!");
                    return;
                }

                // Test audio playback
                _logger.LogInfo("TtsDiagnostic", "Playing audio...");
                await _audioService.PlayAsync(audioData, cancellationToken);

                _logger.LogSuccess("TtsDiagnostic", "Audio playback completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError("TtsDiagnostic", $"TTS Diagnostic failed: {ex.Message}");
            }

            _logger.LogInfo("TtsDiagnostic", "=== TTS DIAGNOSTICS END ===");
        }

        public void PrintConfiguration()
        {
            _logger.LogInfo("TtsDiagnostic", "=== TTS CONFIGURATION ===");
            _logger.LogInfo("TtsDiagnostic", $"Voice Pipeline Enabled: {_voiceOptions.Enabled}");
            _logger.LogInfo("TtsDiagnostic", $"TTS Enabled: {_voiceOptions.EnableTts}");
            _logger.LogInfo("TtsDiagnostic", $"TTS Provider: {_voiceOptions.TtsProvider}");
            _logger.LogInfo("TtsDiagnostic", $"Stream Responses: {_voiceOptions.StreamResponses}");
            _logger.LogInfo("TtsDiagnostic", $"Speak Sentence by Sentence: {_voiceOptions.SpeakSentenceBySentence}");
            _logger.LogInfo("TtsDiagnostic", $"Configured Provider in TtsOptions: {_ttsOptions.Provider}");
            _logger.LogInfo("TtsDiagnostic", $"EdgeTTS Voice: {_ttsOptions.EdgeTTS.Voice}");
            _logger.LogInfo("TtsDiagnostic", $"ElevenLabs VoiceId: {_ttsOptions.ElevenLabs.VoiceId}");
            _logger.LogInfo("TtsDiagnostic", $"ElevenLabs API Key Length: {_ttsOptions.ElevenLabs.ApiKey?.Length ?? 0}");
            _logger.LogInfo("TtsDiagnostic", "========================");
        }
    }
}

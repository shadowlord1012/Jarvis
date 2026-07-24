using Jarvis.Interfaces;
using Jarvis.Speech;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jarvis.Diagnostics
{
    /// <summary>
    /// Diagnostic tool to test TTS and Audio playback
    /// </summary>
    public sealed class TtsDiagnostic
    {
        private readonly TtsProviderFactory _ttsFactory;
        private readonly IAudioService _audioService;
        private readonly ILogger<TtsDiagnostic> _logger;
        private readonly VoicePipelineOptions _voiceOptions;
        private readonly TtsOptions _ttsOptions;

        public TtsDiagnostic(
            TtsProviderFactory ttsFactory,
            IAudioService audioService,
            IOptions<VoicePipelineOptions> voiceOptions,
            IOptions<TtsOptions> ttsOptions,
            ILogger<TtsDiagnostic> logger)
        {
            _ttsFactory = ttsFactory;
            _audioService = audioService;
            _voiceOptions = voiceOptions.Value;
            _ttsOptions = ttsOptions.Value;
            _logger = logger;
        }

        public async Task RunDiagnosticsAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("=== TTS DIAGNOSTICS START ===");

            // Check configuration
            _logger.LogInformation("Voice Pipeline Enabled: {Enabled}", _voiceOptions.Enabled);
            _logger.LogInformation("TTS Enabled: {Enabled}", _voiceOptions.EnableTts);
            _logger.LogInformation("TTS Provider: {Provider}", _voiceOptions.TtsProvider);
            _logger.LogInformation("Configured Provider: {Provider}", _ttsOptions.Provider);

            try
            {
                // Get the TTS provider
                _logger.LogInformation("Getting TTS provider: {Provider}", _voiceOptions.TtsProvider);
                var provider = _ttsFactory.GetProvider(_voiceOptions.TtsProvider);
                _logger.LogInformation("TTS Provider loaded: {ProviderName}", provider.ProviderName);

                // Test TTS synthesis
                var testText = "This is a test of the text to speech system.";
                _logger.LogInformation("Synthesizing test text: {Text}", testText);

                var audioData = await provider.SynthesizeAsync(testText, cancellationToken);

                _logger.LogInformation("Audio data generated. Size: {Size} bytes", audioData.Length);

                if (audioData.Length == 0)
                {
                    _logger.LogError("TTS provider returned empty audio data!");
                    return;
                }

                // Test audio playback
                _logger.LogInformation("Playing audio...");
                await _audioService.PlayAsync(audioData, cancellationToken);

                _logger.LogInformation("Audio playback completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TTS Diagnostic failed");
            }

            _logger.LogInformation("=== TTS DIAGNOSTICS END ===");
        }

        public void PrintConfiguration()
        {
            Console.WriteLine("=== TTS CONFIGURATION ===");
            Console.WriteLine($"Voice Pipeline Enabled: {_voiceOptions.Enabled}");
            Console.WriteLine($"TTS Enabled: {_voiceOptions.EnableTts}");
            Console.WriteLine($"TTS Provider: {_voiceOptions.TtsProvider}");
            Console.WriteLine($"Stream Responses: {_voiceOptions.StreamResponses}");
            Console.WriteLine($"Speak Sentence by Sentence: {_voiceOptions.SpeakSentenceBySentence}");
            Console.WriteLine($"Configured Provider in TtsOptions: {_ttsOptions.Provider}");
            Console.WriteLine($"EdgeTTS Voice: {_ttsOptions.EdgeTTS.Voice}");
            Console.WriteLine($"ElevenLabs VoiceId: {_ttsOptions.ElevenLabs.VoiceId}");
            Console.WriteLine($"ElevenLabs API Key Length: {_ttsOptions.ElevenLabs.ApiKey?.Length ?? 0}");
            Console.WriteLine("========================");
        }
    }
}

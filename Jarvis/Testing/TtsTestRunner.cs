using Jarvis.Audio;
using Jarvis.Interfaces;
using Jarvis.Speech;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using UI.Controls.HUD.Interfaces;

namespace Jarvis.Testing
{
    /// <summary>
    /// Simple console app to test TTS audio output
    /// Usage: Call TestTtsAsync from your main application
    /// </summary>
    public static class TtsTestRunner
    {
        public static async Task TestTtsAsync(IServiceProvider serviceProvider)
        {
            // Get logger service
            var logger = serviceProvider.GetRequiredService<ILogService>();

            logger.LogInfo("TtsTestRunner", "======================");
            logger.LogInfo("TtsTestRunner", "TTS AUDIO TEST STARTED");
            logger.LogInfo("TtsTestRunner", "======================");

            try
            {
                // Get services
                var ttsFactory = serviceProvider.GetRequiredService<TtsProviderFactory>();
                var audioService = serviceProvider.GetRequiredService<IAudioService>();
                var voiceOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<VoicePipelineOptions>>();

                logger.LogInfo("TtsTestRunner", $"TTS Provider: {voiceOptions.Value.TtsProvider}");
                logger.LogInfo("TtsTestRunner", $"TTS Enabled: {voiceOptions.Value.EnableTts}");

                // Get the provider
                logger.LogInfo("TtsTestRunner", "Getting TTS provider...");
                var provider = ttsFactory.GetProvider(voiceOptions.Value.TtsProvider);
                logger.LogInfo("TtsTestRunner", $"Provider loaded: {provider.ProviderName}");

                // Generate audio
                var testText = "Hello, this is a test of the audio playback system.";
                logger.LogInfo("TtsTestRunner", $"Generating audio for: \"{testText}\"");

                var audioData = await provider.SynthesizeAsync(testText);

                logger.LogInfo("TtsTestRunner", $"Audio generated: {audioData.Length} bytes");

                if (audioData.Length == 0)
                {
                    logger.LogError("TtsTestRunner", "ERROR: No audio data generated!");
                    return;
                }

                // Play audio
                logger.LogInfo("TtsTestRunner", "Playing audio...");
                await audioService.PlayAsync(audioData);

                logger.LogInfo("TtsTestRunner", "======================");
                logger.LogSuccess("TtsTestRunner", "TTS AUDIO TEST SUCCESS");
                logger.LogInfo("TtsTestRunner", "======================");
            }
            catch (Exception ex)
            {
                logger.LogInfo("TtsTestRunner", "======================");
                logger.LogError("TtsTestRunner", "TTS AUDIO TEST FAILED");
                logger.LogInfo("TtsTestRunner", "======================");
                logger.LogError("TtsTestRunner", $"Error: {ex.Message}");
                logger.LogError("TtsTestRunner", $"Stack: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    logger.LogError("TtsTestRunner", $"Inner: {ex.InnerException.Message}");
                }
            }
        }
    }
}

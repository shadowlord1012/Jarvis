using Jarvis.Audio;
using Jarvis.Interfaces;
using Jarvis.Speech;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

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
            Console.WriteLine("======================");
            Console.WriteLine("TTS AUDIO TEST STARTED");
            Console.WriteLine("======================");
            Console.WriteLine();

            try
            {
                // Get services
                var ttsFactory = serviceProvider.GetRequiredService<TtsProviderFactory>();
                var audioService = serviceProvider.GetRequiredService<IAudioService>();
                var logger = serviceProvider.GetRequiredService<ILogger<AudioService>>();
                var voiceOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<VoicePipelineOptions>>();

                Console.WriteLine($"TTS Provider: {voiceOptions.Value.TtsProvider}");
                Console.WriteLine($"TTS Enabled: {voiceOptions.Value.EnableTts}");
                Console.WriteLine();

                // Get the provider
                Console.WriteLine("Getting TTS provider...");
                var provider = ttsFactory.GetProvider(voiceOptions.Value.TtsProvider);
                Console.WriteLine($"Provider loaded: {provider.ProviderName}");
                Console.WriteLine();

                // Generate audio
                var testText = "Hello, this is a test of the audio playback system.";
                Console.WriteLine($"Generating audio for: \"{testText}\"");

                var audioData = await provider.SynthesizeAsync(testText);

                Console.WriteLine($"Audio generated: {audioData.Length} bytes");
                Console.WriteLine();

                if (audioData.Length == 0)
                {
                    Console.WriteLine("ERROR: No audio data generated!");
                    return;
                }

                // Play audio
                Console.WriteLine("Playing audio...");
                await audioService.PlayAsync(audioData);

                Console.WriteLine();
                Console.WriteLine("======================");
                Console.WriteLine("TTS AUDIO TEST SUCCESS");
                Console.WriteLine("======================");
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("======================");
                Console.WriteLine("TTS AUDIO TEST FAILED");
                Console.WriteLine("======================");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner: {ex.InnerException.Message}");
                }
            }
        }
    }
}

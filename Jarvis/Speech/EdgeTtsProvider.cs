using Jarvis.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Jarvis.Speech
{
    public sealed class EdgeTtsProvider : ITtsProvider
    {
        private readonly EdgeTtsOptions _options;
        private readonly ILogger<EdgeTtsProvider> _logger;

        public string ProviderName { get; } = "EdgeTTS";

        public EdgeTtsProvider(
            IOptions<TtsOptions> options,
            ILogger<EdgeTtsProvider> logger)
        {
            _options = options.Value.EdgeTTS;
            _logger = logger;
        }

        public async Task<byte[]> SynthesizeAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Array.Empty<byte>();
            }

            var tempFile = Path.Combine(
                Path.GetTempPath(),
                $"jarvis_tts_{Guid.NewGuid():N}.mp3");

            try
            {
                _logger.LogDebug(
                    "Generating Edge TTS audio.");

                var psi = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments =
                        $"-m edge_tts " +
                        $"--voice \"{_options.Voice}\" " +
                        $"--text \"{EscapeArgument(text)}\" " +
                        $"--write-media \"{tempFile}\"",

                    RedirectStandardOutput = true,
                    RedirectStandardError = true,

                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process =
                    new Process
                    {
                        StartInfo = psi
                    };

                process.Start();

                await process.WaitForExitAsync(
                    cancellationToken);

                if (process.ExitCode != 0)
                {
                    var error =
                        await process.StandardError.ReadToEndAsync(
                            cancellationToken);

                    throw new InvalidOperationException(
                        $"Edge TTS failed: {error}");
                }

                return await File.ReadAllBytesAsync(
                    tempFile,
                    cancellationToken);
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        private static string EscapeArgument(
            string text)
        {
            return text
                .Replace("\"", "\\\"")
                .Replace("\r", " ")
                .Replace("\n", " ");
        }
    }
}

using Jarvis.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Jarvis.Speech
{
    public sealed class ElevenLabsTtsProvider : ITtsProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ElevenLabsOptions _options;
        private readonly ILogger<ElevenLabsTtsProvider> _logger;

        public string ProviderName { get; } = "ElevenLabs";

        public ElevenLabsTtsProvider(
            HttpClient httpClient,
            IOptions<TtsOptions> options,
            ILogger<ElevenLabsTtsProvider> logger)
        {
            _httpClient = httpClient;
            _options = options.Value.ElevenLabs;
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

            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                throw new InvalidOperationException(
                    "ElevenLabs API key is not configured.");
            }

            var url =
                $"https://api.elevenlabs.io/v1/text-to-speech/" +
                $"{_options.VoiceId}";

            var requestBody = new
            {
                text,
                model_id = _options.ModelId
            };

            using var request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    url);

            request.Headers.Add(
                "xi-api-key",
                _options.ApiKey);

            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue(
                    "audio/mpeg"));

            request.Content =
                new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");

            _logger.LogDebug(
                "Sending text to ElevenLabs TTS.");

            using var response =
                await _httpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error =
                    await response.Content.ReadAsStringAsync(
                        cancellationToken);

                throw new HttpRequestException(
                    $"ElevenLabs TTS failed " +
                    $"({response.StatusCode}): {error}");
            }

            return await response.Content.ReadAsByteArrayAsync(
                cancellationToken);
        }
    }
}

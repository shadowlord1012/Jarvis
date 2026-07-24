using Jarvis.AI.Interfaces;
using Jarvis.AI.Ollama;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using UI.Controls.HUD.Interfaces;

namespace Jarvis.AI.Providers
{
    public sealed class OllamaProvider : ILLMProvider
    {
        private readonly HttpClient _httpClient;

        private readonly ILogService _logger;

        private readonly OllamaOptions _options;

        private readonly JsonSerializerOptions _jsonOptions;

        private string _currentModel;

        public OllamaProvider(
            HttpClient httpClient,
            IOptions<OllamaOptions> options,
            ILogService logger)
        {
            _httpClient = httpClient;

            _logger = logger;

            _options = options.Value;

            _currentModel = _options.DefaultModel;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public string ProviderName =>
            "Ollama";

        public string CurrentModel =>
            _currentModel;

        // ------------------------------------------------------------
        // INITIALIZATION
        // ------------------------------------------------------------

        public async Task InitializeAsync(
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Ollama",
                "Initializing Ollama provider.");

            var healthy =
                await CheckHealthAsync(cancellationToken);

            if (!healthy)
            {
                throw new InvalidOperationException(
                    "Unable to connect to Ollama.");
            }

            _logger.LogDebug("Ollama",String.Format("Current Model {0}", _currentModel));

            // Pre-warm the model in background if enabled
            if (_options.EnableWarmup)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await WarmupModelAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Ollama",
                            $"Background model warmup failed: {ex.Message}");
                    }
                }, cancellationToken);

                _logger.LogDebug("Ollama",
                    "Model warmup started in background.");
            }
        }

        // ------------------------------------------------------------
        // WARMUP MODEL
        // ------------------------------------------------------------

        private async Task WarmupModelAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("Ollama", 
                    $"Pre-loading model '{_currentModel}' into memory...");

                var warmupRequest = new OllamaChatRequest
                {
                    Model = _currentModel,
                    Stream = false,
                    KeepAlive = _options.KeepAlive ? _options.KeepAliveDuration : null,
                    Messages = new List<OllamaMessage>
                    {
                        new OllamaMessage
                        {
                            Role = "user",
                            Content = "Hi"
                        }
                    },
                    Options = new OllamaGenerateOptions
                    {
                        NumPredict = 1  // Only generate 1 token
                    }
                };

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                using var response = await _httpClient.PostAsJsonAsync(
                    OllamaEndpoints.Chat,
                    warmupRequest,
                    _jsonOptions,
                    cancellationToken);

                stopwatch.Stop();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogDebug("Ollama",
                        $"Model pre-loaded in {stopwatch.Elapsed.TotalSeconds:F2}s. Ready for fast responses.");
                }
                else
                {
                    _logger.LogWarning("Ollama",
                        $"Model warmup returned status {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Ollama",
                    $"Model warmup failed (non-critical): {ex.Message}");
            }
        }

        // ------------------------------------------------------------
        // HEALTH CHECK
        // ------------------------------------------------------------

        public async Task<bool> CheckHealthAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var response =
                    await _httpClient.GetAsync(
                        OllamaEndpoints.Version,
                        cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Ollama",String.Format("Ollama health check failed with status {0}.",response.StatusCode));

                    return false;
                }

                _logger.LogDebug("Ollama","Ollama health check successful.");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ollama",String.Format("Unable to connect: {0}", ex.Message));

                return false;
            }
        }

        // ------------------------------------------------------------
        // GENERATE RESPONSE
        // ------------------------------------------------------------

        public async Task<LLMResponse> GenerateAsync(
            LLMRequest request,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Ollama", "Building Ollama request...");

            var ollamaRequest =
                BuildOllamaRequest(request);

            ollamaRequest.Stream = false;

            _logger.LogDebug("Ollama",String.Format("Sending request to Ollama. Model: {0}",ollamaRequest.Model));

            for (var attempt = 1;
                 attempt <= _options.MaxRetries + 1;
                 attempt++)
            {
                try
                {
                    var stopwatch =
                        System.Diagnostics.Stopwatch.StartNew();

                    _logger.LogDebug("Ollama", $"POST request starting (attempt {attempt})...");

                    using var response =
                        await _httpClient.PostAsJsonAsync(
                            OllamaEndpoints.Chat,
                            ollamaRequest,
                            _jsonOptions,
                            cancellationToken);

                    stopwatch.Stop();

                    _logger.LogDebug("Ollama", $"POST request completed in {stopwatch.Elapsed.TotalSeconds:F2}s");

                    Console.WriteLine(
                        $"Ollama request completed in " +
                        $"{stopwatch.Elapsed.TotalSeconds:F2} seconds.");
                    if (response.IsSuccessStatusCode)
                    {
                        _logger.LogDebug("Ollama", "Reading response content...");

                        var result =
                            await response.Content
                                .ReadFromJsonAsync<OllamaChatResponse>(
                                    _jsonOptions,
                                    cancellationToken);

                        if (result is null)
                        {
                            throw new InvalidOperationException(
                                "Ollama returned an empty response.");
                        }

                        _logger.LogDebug("Ollama", "Response parsed successfully.");

                        return ConvertResponse(result);
                    }

                    if (!IsRetryable(response.StatusCode))
                    {
                        await ThrowOllamaErrorAsync(response);
                    }

                    _logger.LogWarning("Ollama", String.Format("Ollama request failed with status {0}. Attempt {1}.",response.StatusCode,attempt));
                }
                catch (HttpRequestException ex)
                    when (attempt <= _options.MaxRetries)
                {
                    _logger.LogWarning("Ollama", String.Format("Ollama HTTP request failed. Attempt {0} : {1}",attempt,ex.Message));
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogWarning("Ollama", $"Ollama request timed out or was cancelled. Attempt {attempt}: {ex.Message}");
                    throw new OperationCanceledException($"Ollama request timed out (attempt {attempt})", ex, cancellationToken);
                }

                if (attempt <= _options.MaxRetries)
                {
                    await Task.Delay(
                        TimeSpan.FromSeconds(attempt),
                        cancellationToken);
                }
            }

            throw new InvalidOperationException(
                "Ollama request failed after all retry attempts.");
        }

        // ------------------------------------------------------------
        // STREAMING RESPONSE
        // ------------------------------------------------------------

        public async IAsyncEnumerable<LLMStreamChunk> StreamAsync(
            LLMRequest request,
            [EnumeratorCancellation]
        CancellationToken cancellationToken = default)
        {
            var ollamaRequest =
                BuildOllamaRequest(request);

            ollamaRequest.Stream = true;

            var json =
                JsonSerializer.Serialize(
                    ollamaRequest,
                    _jsonOptions);

            using var httpRequest =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    OllamaEndpoints.Chat);

            httpRequest.Content =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

            using var response =
                await _httpClient.SendAsync(
                    httpRequest,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await ThrowOllamaErrorAsync(response);
            }

            await using var stream =
                await response.Content
                    .ReadAsStreamAsync(cancellationToken);

            using var reader =
                new StreamReader(stream);

            while (!reader.EndOfStream &&
                   !cancellationToken.IsCancellationRequested)
            {
                var line =
                    await reader.ReadLineAsync(
                        cancellationToken);

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                OllamaChatResponse? chunk;

                try
                {
                    chunk =
                        JsonSerializer.Deserialize<OllamaChatResponse>(
                            line,
                            _jsonOptions);
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning("Ollama",String.Format("Unable to deserialize Ollama stream chunk. {0}",ex.Message));

                    continue;
                }

                if (chunk is null)
                {
                    continue;
                }

                yield return ConvertStreamChunk(chunk);

                if (chunk.Done)
                {
                    yield break;
                }
            }
        }

        // ------------------------------------------------------------
        // GET MODELS
        // ------------------------------------------------------------

        public async Task<IReadOnlyList<string>> GetModelsAsync(
            CancellationToken cancellationToken = default)
        {
            using var response =
                await _httpClient.GetAsync(
                    OllamaEndpoints.Tags,
                    cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                await ThrowOllamaErrorAsync(response);
            }

            var result =
                await response.Content
                    .ReadFromJsonAsync<OllamaModelResponse>(
                        _jsonOptions,
                        cancellationToken);

            if (result is null)
            {
                return [];
            }

            return result.Models
                .Select(x => x.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }

        // ------------------------------------------------------------
        // SET MODEL
        // ------------------------------------------------------------

        public async Task SetModelAsync(
            string model,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(model))
            {
                throw new ArgumentException(
                    "Model name cannot be empty.",
                    nameof(model));
            }

            var models =
                await GetModelsAsync(cancellationToken);

            if (!models.Contains(
                    model,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Ollama model '{model}' is not installed.");
            }

            _currentModel = model;

            _logger.LogDebug("Ollama",String.Format("Ollama model changed to {0}.",model));
        }

        // ------------------------------------------------------------
        // BUILD OLLAMA REQUEST
        // ------------------------------------------------------------

        private OllamaChatRequest BuildOllamaRequest(
            LLMRequest request)
        {
            var ollamaRequest =
                new OllamaChatRequest
                {
                    Model =
                        string.IsNullOrWhiteSpace(request.Model)
                            ? _currentModel
                            : request.Model,

                    Stream =
                        request.Stream ||
                        request.Options.EnableStreaming,

                    KeepAlive =
                        request.Options.KeepAlive
                        ?? (_options.KeepAlive ? _options.KeepAliveDuration : null),

                    Options =
                        new OllamaGenerateOptions
                        {
                            Temperature =
                                request.Options.Temperature
                                ?? _options.Temperature,

                            TopP =
                                request.Options.TopP
                                ?? _options.TopP,

                            TopK =
                                request.Options.TopK,

                            NumPredict =
                                request.Options.MaxTokens
                                ?? _options.MaxTokens,

                            RepeatPenalty =
                                request.Options.RepeatPenalty,

                            Seed =
                                request.Options.Seed
                        }
                };

            foreach (var message in request.Messages)
            {
                ollamaRequest.Messages.Add(
                    new OllamaMessage
                    {
                        Role =
                            message.Role.ToString()
                                .ToLowerInvariant(),

                        Content =
                            message.Content
                    });
            }

            if (!string.IsNullOrWhiteSpace(
                    request.SystemPrompt))
            {
                ollamaRequest.Messages.Insert(
                    0,
                    new OllamaMessage
                    {
                        Role = "system",

                        Content =
                            request.SystemPrompt
                    });
            }

            if (_options.EnableToolCalling)
            {
                foreach (var tool in request.Tools)
                {
                    ollamaRequest.Tools.Add(
                        ConvertTool(tool));
                }
            }

            return ollamaRequest;
        }

        // ------------------------------------------------------------
        // TOOL CONVERSION
        // ------------------------------------------------------------

        private static OllamaTool ConvertTool(
            ToolDefinition tool)
        {
            return new OllamaTool
            {
                Type = "function",

                Function =
                    new OllamaToolFunction
                    {
                        Name =
                            tool.Name,

                        Description =
                            tool.Description,

                        Parameters =
                            tool.JsonSchema
                                is null
                                    ? default
                                    : JsonSerializer.Deserialize<JsonElement>(
                                        tool.JsonSchema)
                    }
            };
        }

        // ------------------------------------------------------------
        // RESPONSE CONVERSION
        // ------------------------------------------------------------

        private static LLMResponse ConvertResponse(
            OllamaChatResponse response)
        {
            var message =
                response.Message;

            return new LLMResponse
            {
                Content =
                    message?.Content
                    ?? string.Empty,

                Model =
                    response.Model,

                Done =
                    response.Done,

                ToolCalls =
                    message?.ToolCalls?
                        .Select(tool =>
                            new LLMToolCall
                            {
                                Name =
                                    tool.Function.Name,

                                Arguments =
                                    tool.Function.Arguments
                                        .GetRawText()
                            })
                        .ToList()
                        ?? []
            };
        }

        // ------------------------------------------------------------
        // STREAM CONVERSION
        // ------------------------------------------------------------

        private static LLMStreamChunk ConvertStreamChunk(
            OllamaChatResponse response)
        {
            return new LLMStreamChunk
            {
                Content =
                    response.Message?.Content
                    ?? string.Empty,

                Model =
                    response.Model,

                IsComplete =
                    response.Done
            };
        }

        // ------------------------------------------------------------
        // RETRY LOGIC
        // ------------------------------------------------------------

        private static bool IsRetryable(
            HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.RequestTimeout
                || statusCode == HttpStatusCode.TooManyRequests
                || statusCode == HttpStatusCode.BadGateway
                || statusCode == HttpStatusCode.ServiceUnavailable
                || statusCode == HttpStatusCode.GatewayTimeout;
        }

        // ------------------------------------------------------------
        // OLLAMA ERROR
        // ------------------------------------------------------------

        private static async Task ThrowOllamaErrorAsync(
            HttpResponseMessage response)
        {
            var content =
                await response.Content.ReadAsStringAsync();

            string message;

            try
            {
                var error =
                    JsonSerializer.Deserialize<OllamaErrorResponse>(
                        content);

                message =
                    error?.Error
                    ?? content;
            }
            catch
            {
                message = content;
            }

            throw new HttpRequestException(
                $"Ollama returned HTTP {(int)response.StatusCode}: {message}");
        }
    }
}

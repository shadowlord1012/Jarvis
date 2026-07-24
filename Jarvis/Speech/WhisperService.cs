using Jarvis.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Whisper.net;

namespace Jarvis.Speech
{
    public sealed class WhisperService : IWhisperService, IDisposable
    {
        private readonly WhisperOptions _options;
        private readonly ILogger<WhisperService> _logger;
        private readonly WhisperModelDownloader _modelDownloader;

        private WhisperFactory? _factory;
        private readonly Task _initializationTask;

        public WhisperService(
            IOptions<WhisperOptions> options,
            ILogger<WhisperService> logger,
            WhisperModelDownloader modelDownloader)
        {
            _options = options.Value;
            _logger = logger;
            _modelDownloader = modelDownloader;

            // Start initialization asynchronously
            _initializationTask = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            if (string.IsNullOrWhiteSpace(_options.ModelPath))
            {
                _logger.LogWarning(
                    "Whisper model path has not been configured.");

                return;
            }

            try
            {
                // Ensure the model exists, download if necessary
                var modelExists = await _modelDownloader.EnsureModelExistsAsync(
                    _options.ModelPath,
                    _options.ModelName ?? "small",
                    progress: new Progress<double>(p => 
                    {
                        if (p % 0.1 < 0.01) // Log every 10%
                        {
                            _logger.LogInformation(
                                "Downloading Whisper model: {Progress:P0}",
                                p);
                        }
                    }));

                if (!modelExists)
                {
                    _logger.LogError(
                        "Failed to download Whisper model to {ModelPath}",
                        _options.ModelPath);
                    return;
                }

                if (!File.Exists(_options.ModelPath))
                {
                    _logger.LogError(
                        "Whisper model not found at {ModelPath} even after download attempt",
                        _options.ModelPath);
                    return;
                }

                _factory = WhisperFactory.FromPath(
                    _options.ModelPath);

                _logger.LogInformation(
                    "Whisper initialized using model {ModelPath}",
                    _options.ModelPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to initialize Whisper.");
            }
        }

        public async Task<WhisperTranscriptionResult> TranscribeAsync(
            string audioFilePath,
            CancellationToken cancellationToken = default)
        {
            // Ensure initialization is complete
            await _initializationTask;

            if (_factory is null)
            {
                throw new InvalidOperationException(
                    "Whisper has not been initialized.");
            }

            if (!File.Exists(audioFilePath))
            {
                throw new FileNotFoundException(
                    "Audio file was not found.",
                    audioFilePath);
            }

            _logger.LogDebug(
                "Starting Whisper transcription for {AudioFile}",
                audioFilePath);

            using var processor = _factory.CreateBuilder()
                .WithLanguage(_options.Language)
                .Build();

            await using var audioStream =
                File.OpenRead(audioFilePath);

            var text = new System.Text.StringBuilder();

            await foreach (
                var segment in processor.ProcessAsync(audioStream)
                    .WithCancellation(cancellationToken))
            {
                if (!string.IsNullOrWhiteSpace(segment.Text))
                {
                    text.Append(segment.Text);
                }
            }

            var transcript = text
                .ToString()
                .Trim();

            _logger.LogInformation(
                "Whisper transcription complete: {Text}",
                transcript);

            return new WhisperTranscriptionResult
            {
                Text = transcript,
                Language = _options.Language
            };
        }

        public void Dispose()
        {
            _factory?.Dispose();
        }
    }
}

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Jarvis.Speech
{
    /// <summary>
    /// Downloads and manages Whisper model files
    /// </summary>
    public class WhisperModelDownloader
    {
        private readonly ILogger<WhisperModelDownloader> _logger;
        private readonly HttpClient _httpClient;

        // Hugging Face model URLs for Whisper GGML models
        private static readonly Dictionary<string, string> ModelUrls = new()
        {
            ["tiny"] = "https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-tiny.bin",
            ["tiny.en"] = "https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-tiny.en.bin",
            ["base"] = "https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-base.bin",
            ["base.en"] = "https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-base.en.bin",
            ["small"] = "https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-small.bin",
            ["small.en"] = "https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-small.en.bin",
            ["medium"] = "https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-medium.bin",
            ["medium.en"] = "https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-medium.en.bin",
            ["large-v1"] = "https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-large-v1.bin",
            ["large-v2"] = "https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-large-v2.bin",
            ["large-v3"] = "https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-large-v3.bin"
        };

        public WhisperModelDownloader(
            ILogger<WhisperModelDownloader> logger,
            HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromMinutes(30); // Models can be large
        }

        /// <summary>
        /// Ensures the specified Whisper model exists, downloading it if necessary
        /// </summary>
        /// <param name="modelPath">Full path where the model should be stored</param>
        /// <param name="modelName">Model name (tiny, base, small, medium, large-v3, etc.)</param>
        /// <param name="progress">Optional progress callback (0.0 to 1.0)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if model is ready, false otherwise</returns>
        public async Task<bool> EnsureModelExistsAsync(
            string modelPath,
            string modelName,
            IProgress<double>? progress = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Check if model already exists
                if (File.Exists(modelPath))
                {
                    var fileInfo = new FileInfo(modelPath);
                    if (fileInfo.Length > 0)
                    {
                        _logger.LogInformation(
                            "Whisper model already exists at {ModelPath} ({Size:N0} bytes)",
                            modelPath,
                            fileInfo.Length);
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Existing model file at {ModelPath} is empty, will re-download",
                            modelPath);
                        File.Delete(modelPath);
                    }
                }

                // Get download URL
                if (!ModelUrls.TryGetValue(modelName.ToLowerInvariant(), out var url))
                {
                    _logger.LogError(
                        "Unknown Whisper model name: {ModelName}. Available models: {Models}",
                        modelName,
                        string.Join(", ", ModelUrls.Keys));
                    return false;
                }

                // Create directory if it doesn't exist
                var directory = Path.GetDirectoryName(modelPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                    _logger.LogInformation("Created directory: {Directory}", directory);
                }

                // Download the model
                _logger.LogInformation(
                    "Downloading Whisper model '{ModelName}' from {Url}",
                    modelName,
                    url);

                await DownloadFileAsync(url, modelPath, progress, cancellationToken);

                var downloadedFileInfo = new FileInfo(modelPath);
                _logger.LogInformation(
                    "Successfully downloaded Whisper model to {ModelPath} ({Size:N0} bytes)",
                    modelPath,
                    downloadedFileInfo.Length);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to download Whisper model '{ModelName}' to {ModelPath}",
                    modelName,
                    modelPath);
                return false;
            }
        }

        /// <summary>
        /// Downloads a file with progress reporting
        /// </summary>
        private async Task DownloadFileAsync(
            string url,
            string destinationPath,
            IProgress<double>? progress,
            CancellationToken cancellationToken)
        {
            using var response = await _httpClient.GetAsync(
                url,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            var canReportProgress = totalBytes != -1L && progress != null;

            var tempPath = destinationPath + ".tmp";

            try
            {
                // Download to temp file
                {
                    using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    using var fileStream = new FileStream(
                        tempPath,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None,
                        8192,
                        true);

                    var buffer = new byte[8192];
                    long totalRead = 0L;
                    int bytesRead;

                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                        totalRead += bytesRead;

                        if (canReportProgress)
                        {
                            var progressValue = (double)totalRead / totalBytes;
                            progress!.Report(progressValue);

                            if (totalRead % (1024 * 1024 * 10) == 0) // Log every 10MB
                            {
                                _logger.LogInformation(
                                    "Download progress: {Downloaded:N0} / {Total:N0} bytes ({Percent:P1})",
                                    totalRead,
                                    totalBytes,
                                    progressValue);
                            }
                        }
                    }

                    await fileStream.FlushAsync(cancellationToken);
                } // FileStream is disposed here, releasing the file lock

                // Now move the file after streams are closed
                if (File.Exists(destinationPath))
                {
                    File.Delete(destinationPath);
                }
                File.Move(tempPath, destinationPath);
            }
            catch
            {
                // Clean up temp file on error
                if (File.Exists(tempPath))
                {
                    try { File.Delete(tempPath); } catch { /* Ignore cleanup errors */ }
                }
                throw;
            }
        }

        /// <summary>
        /// Gets the recommended model size based on available system resources
        /// </summary>
        public static string GetRecommendedModel()
        {
            var totalMemoryGB = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024.0 * 1024.0 * 1024.0);

            return totalMemoryGB switch
            {
                < 4 => "tiny",      // < 4GB RAM
                < 8 => "base",      // 4-8GB RAM
                < 16 => "small",    // 8-16GB RAM
                < 32 => "medium",   // 16-32GB RAM
                _ => "large-v3"     // 32GB+ RAM
            };
        }

        /// <summary>
        /// Lists all available Whisper models
        /// </summary>
        public static IReadOnlyDictionary<string, string> GetAvailableModels() => ModelUrls;
    }
}

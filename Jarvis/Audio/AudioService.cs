using Jarvis.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using NAudio.Wave;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UI.Controls.HUD.Interfaces;

namespace Jarvis.Audio
{
    public sealed class AudioService : IAudioService, IDisposable
    {
        private readonly ILogger<AudioService> _logger;
        private readonly ILogService _logService;

        private readonly ConcurrentDictionary<Guid, CancellationTokenSource>
            _activeOperations = new();

        private WaveInEvent? _waveIn;
        private WaveFileWriter? _waveWriter;

        private readonly object _recordingLock = new();

        private bool _isRecording;
        private string? _currentRecordingPath;

        public bool IsRecording => _isRecording;

        public AudioService(
            ILogger<AudioService> logger,
            ILogService logService)
        {
            _logger = logger;
            _logService = logService;
        }

        // ============================================================
        // RECORD MICROPHONE AUDIO
        // ============================================================

        public async Task<string> RecordAsync(
            CancellationToken cancellationToken = default)
        {
            if (IsRecording)
            {
                throw new InvalidOperationException(
                    "Audio recording is already in progress.");
            }

            var operationId = Guid.NewGuid();

            using var linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken);

            _activeOperations.TryAdd(
                operationId,
                linkedCts);

            try
            {
                var recordingPath =
                    CreateRecordingFilePath();

                _logService.LogDebug(
                    "AudioService",
                    "Audio recording started.");

                _logger.LogInformation(
                    "Starting microphone recording: {Path}",
                    recordingPath);

                var completionSource =
                    new TaskCompletionSource<bool>(
                        TaskCreationOptions.RunContinuationsAsynchronously);

                lock (_recordingLock)
                {
                    if (_isRecording)
                    {
                        throw new InvalidOperationException(
                            "Audio recording is already in progress.");
                    }

                    _currentRecordingPath =
                        recordingPath;

                    _waveIn =
                        new WaveInEvent
                        {
                            WaveFormat =
                                new WaveFormat(
                                    16000,
                                    16,
                                    1),

                            BufferMilliseconds = 100
                        };

                    _waveWriter =
                        new WaveFileWriter(
                            recordingPath,
                            _waveIn.WaveFormat);

                    _waveIn.DataAvailable +=
                        OnDataAvailable;

                    _waveIn.RecordingStopped +=
                        OnRecordingStopped;

                    _isRecording = true;
                }

                linkedCts.Token.Register(
                    () =>
                    {
                        try
                        {
                            StopRecording();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(
                                ex,
                                "Error stopping recording.");
                        }
                    });

                _waveIn.StartRecording();

                await completionSource.Task
                    .WaitAsync(linkedCts.Token);

                _logService.LogDebug(
                    "AudioService",
                    $"Audio recording complete: {recordingPath}");

                return recordingPath;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation(
                    "Audio recording cancelled.");

                _logService.LogDebug(
                    "AudioService",
                    "Audio recording cancelled.");

                StopRecording();

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Audio recording failed.");

                _logService.LogError(
                    "AudioService",
                    $"Audio recording failed: {ex.Message}");

                StopRecording();

                throw;
            }
            finally
            {
                _activeOperations.TryRemove(
                    operationId,
                    out _);
            }
        }

        // ============================================================
        // STOP RECORDING
        // ============================================================

        public void StopRecording()
        {
            lock (_recordingLock)
            {
                if (!_isRecording)
                {
                    return;
                }

                try
                {
                    _logger.LogDebug(
                        "Stopping microphone recording.");

                    _waveIn?.StopRecording();
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to stop microphone recording.");

                    CleanupRecording();
                }
            }
        }

        // ============================================================
        // MICROPHONE DATA
        // ============================================================

        private void OnDataAvailable(
            object? sender,
            WaveInEventArgs e)
        {
            lock (_recordingLock)
            {
                if (_waveWriter is null)
                {
                    return;
                }

                try
                {
                    _waveWriter.Write(
                        e.Buffer,
                        0,
                        e.BytesRecorded);

                    _waveWriter.Flush();
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to write microphone audio.");
                }
            }
        }

        // ============================================================
        // RECORDING STOPPED
        // ============================================================

        private void OnRecordingStopped(
            object? sender,
            StoppedEventArgs e)
        {
            lock (_recordingLock)
            {
                if (e.Exception is not null)
                {
                    _logger.LogError(
                        e.Exception,
                        "Microphone recording stopped with an error.");
                }

                CleanupRecording();
            }
        }

        // ============================================================
        // PLAY AUDIO BYTES
        // ============================================================

        public async Task PlayAsync(
            byte[] audioData,
            CancellationToken cancellationToken = default)
        {
            if (audioData is null ||
                audioData.Length == 0)
            {
                _logger.LogWarning(
                    "PlayAsync called with empty audio data.");

                return;
            }

            _logger.LogInformation(
                "PlayAsync called with {Size} bytes of audio data",
                audioData.Length);

            var operationId = Guid.NewGuid();

            using var linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken);

            _activeOperations.TryAdd(
                operationId,
                linkedCts);

            var tempFile =
                Path.Combine(
                    Path.GetTempPath(),
                    $"jarvis_tts_{Guid.NewGuid():N}.mp3");

            try
            {
                _logger.LogInformation(
                    "Writing audio to temp file: {TempFile}",
                    tempFile);

                await File.WriteAllBytesAsync(
                    tempFile,
                    audioData,
                    linkedCts.Token);

                _logger.LogInformation(
                    "Temp file created successfully. Size: {Size}",
                    new FileInfo(tempFile).Length);

                _logService.LogDebug(
                    "AudioService",
                    "Playing generated TTS audio.");

                await PlayFileAsync(
                    tempFile,
                    linkedCts.Token);

                _logger.LogInformation(
                    "Audio playback completed successfully");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation(
                    "Audio playback cancelled.");

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Audio playback failed.");

                _logService.LogError(
                    "AudioService",
                    $"Audio playback failed: {ex.Message}");

                throw;
            }
            finally
            {
                _activeOperations.TryRemove(
                    operationId,
                    out _);

                TryDeleteFile(tempFile);
            }
        }

        // ============================================================
        // PLAY AUDIO FILE
        // ============================================================

        public async Task PlayFileAsync(
            string audioFilePath,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(audioFilePath))
            {
                throw new ArgumentException(
                    "Audio file path cannot be empty.",
                    nameof(audioFilePath));
            }

            if (!File.Exists(audioFilePath))
            {
                throw new FileNotFoundException(
                    "Audio file was not found.",
                    audioFilePath);
            }

            _logger.LogInformation(
                "PlayFileAsync starting for file: {FilePath}",
                audioFilePath);

            var operationId = Guid.NewGuid();

            using var linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken);

            _activeOperations.TryAdd(
                operationId,
                linkedCts);

            try
            {
                _logger.LogInformation(
                    "Starting playback on background thread");

                await Task.Run(
                    () =>
                    {
                        _logger.LogInformation(
                            "Opening audio file with AudioFileReader");

                        using var audioFile =
                            new AudioFileReader(
                                audioFilePath);

                        _logger.LogInformation(
                            "Audio file opened. Duration: {Duration}, Sample Rate: {SampleRate}",
                            audioFile.TotalTime,
                            audioFile.WaveFormat.SampleRate);

                        using var outputDevice =
                            new WaveOutEvent();

                        _logger.LogInformation(
                            "WaveOutEvent created");

                        var completionSource =
                            new TaskCompletionSource<bool>(
                                TaskCreationOptions
                                    .RunContinuationsAsynchronously);

                        outputDevice.PlaybackStopped +=
                            (_, args) =>
                            {
                                _logger.LogInformation(
                                    "Playback stopped event triggered");

                                if (args.Exception is not null)
                                {
                                    _logger.LogError(
                                        args.Exception,
                                        "Playback stopped with exception");

                                    completionSource
                                        .TrySetException(
                                            args.Exception);

                                    return;
                                }

                                completionSource
                                    .TrySetResult(true);
                            };

                        outputDevice.Init(
                            audioFile);

                        _logger.LogInformation(
                            "Output device initialized");

                        linkedCts.Token.Register(
                            () =>
                            {
                                try
                                {
                                    _logger.LogInformation(
                                        "Cancellation requested, stopping playback");
                                    outputDevice.Stop();
                                }
                                catch
                                {
                                    // Playback is already stopping.
                                }
                            });

                        _logger.LogInformation(
                            "Starting audio playback...");

                        outputDevice.Play();

                        _logger.LogInformation(
                            "outputDevice.Play() called, waiting for completion");

                        completionSource.Task
                            .GetAwaiter()
                            .GetResult();

                        _logger.LogInformation(
                            "Playback task completed");

                    },
                    linkedCts.Token);

                _logger.LogInformation(
                    "PlayFileAsync completed successfully");
            }
            finally
            {
                _activeOperations.TryRemove(
                    operationId,
                    out _);
            }
        }

        // ============================================================
        // STOP ALL AUDIO
        // ============================================================

        public void StopAllAudio()
        {
            foreach (
                var operation in
                _activeOperations.Values)
            {
                try
                {
                    operation.Cancel();
                }
                catch
                {
                    // Ignore cancellation errors.
                }
            }
        }

        // ============================================================
        // RECORDING CLEANUP
        // ============================================================

        private void CleanupRecording()
        {
            lock (_recordingLock)
            {
                if (_waveIn is not null)
                {
                    _waveIn.DataAvailable -=
                        OnDataAvailable;

                    _waveIn.RecordingStopped -=
                        OnRecordingStopped;

                    _waveIn.Dispose();

                    _waveIn = null;
                }

                _waveWriter?.Dispose();

                _waveWriter = null;

                _isRecording = false;
            }
        }

        // ============================================================
        // FILE PATH
        // ============================================================

        private static string CreateRecordingFilePath()
        {
            var directory =
                Path.Combine(
                    Path.GetTempPath(),
                    "Jarvis",
                    "Recordings");

            Directory.CreateDirectory(
                directory);

            return Path.Combine(
                directory,
                $"jarvis_recording_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}.wav");
        }

        // ============================================================
        // DELETE TEMP FILE
        // ============================================================

        private static void TryDeleteFile(
            string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                // Temporary cleanup failure should
                // not crash the Jarvis pipeline.
            }
        }

        // ============================================================
        // DISPOSE
        // ============================================================

        public void Dispose()
        {
            StopAllAudio();

            lock (_recordingLock)
            {
                CleanupRecording();
            }
        }
    }
}

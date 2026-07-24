using Jarvis.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Channels;

namespace Jarvis.Audio
{
    public sealed class AudioPlaybackQueue
    : IAudioPlaybackQueue
    {
        private readonly IAudioService _audioService;
        private readonly ILogger<AudioPlaybackQueue> _logger;

        private readonly Channel<AudioPlaybackItem> _queue;

        private readonly CancellationTokenSource
            _shutdownCts = new();

        private readonly Task _playbackWorker;

        private readonly object _stateLock = new();

        private TaskCompletionSource<bool>
            _completionSource;

        private int _queueLength;

        private bool _isPlaying;

        private bool _disposed;

        public bool IsPlaying
        {
            get
            {
                lock (_stateLock)
                {
                    return _isPlaying;
                }
            }
        }

        public int QueueLength =>
            Volatile.Read(
                ref _queueLength);

        public AudioPlaybackQueue(
            IAudioService audioService,
            ILogger<AudioPlaybackQueue> logger)
        {
            _audioService = audioService;
            _logger = logger;

            _queue =
                Channel.CreateUnbounded<AudioPlaybackItem>(
                    new UnboundedChannelOptions
                    {
                        SingleReader = true,
                        SingleWriter = false,
                        AllowSynchronousContinuations = false
                    });

            _completionSource =
                CreateCompletionSource();

            _playbackWorker =
                Task.Run(
                    PlaybackWorkerAsync);
        }

        // ============================================================
        // ENQUEUE AUDIO BYTES
        // ============================================================

        public async Task EnqueueAsync(
            byte[] audioData,
            CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (audioData is null ||
                audioData.Length == 0)
            {
                _logger.LogWarning(
                    "Ignoring empty audio playback request.");

                return;
            }

            var item =
                AudioPlaybackItem.FromBytes(
                    audioData);

            Interlocked.Increment(
                ref _queueLength);

            ResetCompletionSource();

            try
            {
                await _queue.Writer.WriteAsync(
                    item,
                    cancellationToken);

                _logger.LogDebug(
                    "Audio added to playback queue. Queue length: {QueueLength}",
                    QueueLength);
            }
            catch
            {
                Interlocked.Decrement(
                    ref _queueLength);

                throw;
            }
        }

        // ============================================================
        // ENQUEUE AUDIO FILE
        // ============================================================

        public async Task EnqueueFileAsync(
            string audioFilePath,
            CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(
                audioFilePath))
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

            var item =
                AudioPlaybackItem.FromFile(
                    audioFilePath);

            Interlocked.Increment(
                ref _queueLength);

            ResetCompletionSource();

            try
            {
                await _queue.Writer.WriteAsync(
                    item,
                    cancellationToken);

                _logger.LogDebug(
                    "Audio file added to playback queue: {File}",
                    audioFilePath);
            }
            catch
            {
                Interlocked.Decrement(
                    ref _queueLength);

                throw;
            }
        }

        // ============================================================
        // PLAYBACK WORKER
        // ============================================================

        private async Task PlaybackWorkerAsync()
        {
            _logger.LogInformation(
                "Audio playback queue worker started.");

            try
            {
                await foreach (
                    var item in _queue.Reader
                        .ReadAllAsync(
                            _shutdownCts.Token))
                {
                    try
                    {
                        lock (_stateLock)
                        {
                            _isPlaying = true;
                        }

                        Interlocked.Decrement(
                            ref _queueLength);

                        _logger.LogDebug(
                            "Playing queued audio. Remaining: {QueueLength}",
                            QueueLength);

                        await PlayItemAsync(
                            item,
                            _shutdownCts.Token);
                    }
                    catch (OperationCanceledException)
                        when (_shutdownCts.IsCancellationRequested)
                    {
                        _logger.LogDebug(
                            "Audio playback queue shutting down.");

                        break;
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogDebug(
                            "Queued audio playback cancelled.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Error playing queued audio.");
                    }
                    finally
                    {
                        lock (_stateLock)
                        {
                            _isPlaying = false;
                        }

                        TryCompleteIfIdle();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogDebug(
                    "Audio playback worker cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Audio playback worker failed.");
            }
            finally
            {
                lock (_stateLock)
                {
                    _isPlaying = false;
                }

                TryCompleteIfIdle();

                _logger.LogInformation(
                    "Audio playback queue worker stopped.");
            }
        }

        // ============================================================
        // PLAY ITEM
        // ============================================================

        private async Task PlayItemAsync(
            AudioPlaybackItem item,
            CancellationToken cancellationToken)
        {
            if (item.AudioData is not null)
            {
                await _audioService.PlayAsync(
                    item.AudioData,
                    cancellationToken);

                return;
            }

            if (!string.IsNullOrWhiteSpace(
                item.AudioFilePath))
            {
                await _audioService.PlayFileAsync(
                    item.AudioFilePath,
                    cancellationToken);
            }
        }

        // ============================================================
        // WAIT FOR QUEUE TO FINISH
        // ============================================================

        public async Task WaitForCompletionAsync(
            CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            while (true)
            {
                Task completionTask;

                lock (_stateLock)
                {
                    if (!IsBusy())
                    {
                        return;
                    }

                    completionTask =
                        _completionSource.Task;
                }

                await completionTask.WaitAsync(
                    cancellationToken);
            }
        }

        // ============================================================
        // CLEAR QUEUED AUDIO
        // ============================================================

        public void Clear()
        {
            ThrowIfDisposed();

            var clearedCount = 0;

            while (
                _queue.Reader.TryRead(
                    out _))
            {
                clearedCount++;
            }

            if (clearedCount > 0)
            {
                Interlocked.Add(
                    ref _queueLength,
                    -clearedCount);

                _logger.LogInformation(
                    "Cleared {Count} audio items from playback queue.",
                    clearedCount);
            }

            TryCompleteIfIdle();
        }

        // ============================================================
        // STOP CURRENT AUDIO
        // ============================================================

        public void Stop()
        {
            ThrowIfDisposed();

            _logger.LogInformation(
                "Stopping all Jarvis audio playback.");

            Clear();

            _audioService.StopAllAudio();

            TryCompleteIfIdle();
        }

        // ============================================================
        // CHECK BUSY STATE
        // ============================================================

        private bool IsBusy()
        {
            return _isPlaying ||
                   QueueLength > 0;
        }

        // ============================================================
        // RESET COMPLETION SOURCE
        // ============================================================

        private void ResetCompletionSource()
        {
            lock (_stateLock)
            {
                if (IsBusy())
                {
                    return;
                }

                if (_completionSource.Task.IsCompleted)
                {
                    _completionSource =
                        CreateCompletionSource();
                }
            }
        }

        // ============================================================
        // COMPLETE WHEN IDLE
        // ============================================================

        private void TryCompleteIfIdle()
        {
            lock (_stateLock)
            {
                if (IsBusy())
                {
                    return;
                }

                _completionSource
                    .TrySetResult(true);
            }
        }

        // ============================================================
        // CREATE COMPLETION SOURCE
        // ============================================================

        private static TaskCompletionSource<bool>
            CreateCompletionSource()
        {
            return new TaskCompletionSource<bool>(
                TaskCreationOptions
                    .RunContinuationsAsynchronously);
        }

        // ============================================================
        // DISPOSE
        // ============================================================

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            _logger.LogInformation(
                "Disposing audio playback queue.");

            _queue.Writer.TryComplete();

            _shutdownCts.Cancel();

            _audioService.StopAllAudio();

            try
            {
                await _playbackWorker;
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _shutdownCts.Dispose();

                ClearInternal();
            }
        }

        // ============================================================
        // INTERNAL CLEAR
        // ============================================================

        private void ClearInternal()
        {
            while (
                _queue.Reader.TryRead(
                    out _))
            {
                Interlocked.Decrement(
                    ref _queueLength);
            }

            Interlocked.Exchange(
                ref _queueLength,
                0);
        }

        // ============================================================
        // DISPOSE CHECK
        // ============================================================

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(
                    nameof(AudioPlaybackQueue));
            }
        }

        // ============================================================
        // AUDIO PLAYBACK ITEM
        // ============================================================

        private sealed class AudioPlaybackItem
        {
            public byte[]? AudioData { get; }

            public string? AudioFilePath { get; }

            private AudioPlaybackItem(
                byte[]? audioData,
                string? audioFilePath)
            {
                AudioData = audioData;

                AudioFilePath =
                    audioFilePath;
            }

            public static AudioPlaybackItem FromBytes(
                byte[] audioData)
            {
                return new AudioPlaybackItem(
                    audioData,
                    null);
            }

            public static AudioPlaybackItem FromFile(
                string audioFilePath)
            {
                return new AudioPlaybackItem(
                    null,
                    audioFilePath);
            }
        }
    }
}

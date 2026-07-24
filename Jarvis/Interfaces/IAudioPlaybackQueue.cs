using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Interfaces
{
    public interface IAudioPlaybackQueue : IAsyncDisposable
    {
        bool IsPlaying { get; }

        int QueueLength { get; }

        Task EnqueueAsync(
            byte[] audioData,
            CancellationToken cancellationToken = default);

        Task EnqueueFileAsync(
            string audioFilePath,
            CancellationToken cancellationToken = default);

        Task WaitForCompletionAsync(
            CancellationToken cancellationToken = default);

        void Clear();

        void Stop();
    }
}

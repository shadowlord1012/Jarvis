using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Interfaces
{
    public interface IAudioService
    {
        bool IsRecording { get; }

        Task<string> RecordAsync(
            CancellationToken cancellationToken = default);

        void StopRecording();

        Task PlayAsync(
            byte[] audioData,
            CancellationToken cancellationToken = default);

        Task PlayFileAsync(
            string audioFilePath,
            CancellationToken cancellationToken = default);

        void StopAllAudio();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Interfaces
{
    public interface IVoicePipelineService
    {
        Task ProcessAudioAsync(
            string audioFilePath,
            CancellationToken cancellationToken = default);

        Task ProcessTextAsync(
            string text,
            CancellationToken cancellationToken = default);
    }
}

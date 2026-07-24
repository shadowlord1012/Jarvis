using Jarvis.Speech;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Interfaces
{
    public interface IWhisperService
    {
        Task<WhisperTranscriptionResult> TranscribeAsync(
            string audioFilePath,
            CancellationToken cancellationToken = default);
    }
}

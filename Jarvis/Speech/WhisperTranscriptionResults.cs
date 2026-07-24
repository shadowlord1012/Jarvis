using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Speech
{
    public sealed class WhisperTranscriptionResult
    {
        public string Text { get; init; } = string.Empty;

        public string Language { get; init; } = "en";

        public double Confidence { get; init; }

        public bool IsEmpty =>
            string.IsNullOrWhiteSpace(Text);
    }
}

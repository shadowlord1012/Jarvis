using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Speech
{
    public sealed class VoicePipelineOptions
    {
        public bool Enabled { get; set; } = true;

        public bool EnableTts { get; set; } = true;

        public bool StreamResponses { get; set; } = true;

        public bool SpeakSentenceBySentence { get; set; } = true;

        public int MinimumSentenceLength { get; set; } = 5;

        public string TtsProvider { get; set; } =
            "EdgeTTS";
    }
}

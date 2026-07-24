using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Speech
{
    public sealed class WhisperOptions
    {
        public string ModelPath { get; set; } = string.Empty;

        public string ModelName { get; set; } = "base";

        public string Language { get; set; } = "en";

        public bool TranslateToEnglish { get; set; }

        public bool UseGpu { get; set; }

        public int Threads { get; set; } = 4;
    }
}

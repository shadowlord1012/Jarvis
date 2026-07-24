using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Speech
{
    public sealed class TtsOptions
    {
        public string Provider { get; set; } = "EdgeTTS";

        public EdgeTtsOptions EdgeTTS { get; set; } = new();

        public ElevenLabsOptions ElevenLabs { get; set; } = new();
    }

    public sealed class EdgeTtsOptions
    {
        public string Voice { get; set; } =
            "en-GB-RyanNeural";

        public string OutputFormat { get; set; } =
            "audio-24khz-48kbitrate-mono-mp3";
    }

    public sealed class ElevenLabsOptions
    {
        public string ApiKey { get; set; } = string.Empty;

        public string VoiceId { get; set; } =
            "21m00Tcm4TlvDq8ikWAM";

        public string ModelId { get; set; } =
            "eleven_multilingual_v2";

        public string OutputFormat { get; set; } =
            "mp3_44100_128";
    }
}

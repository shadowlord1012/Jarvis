using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Jarvis.AI.Ollama
{
    public sealed class OllamaGenerateOptions
    {
        [JsonPropertyName("temperature")]
        public double? Temperature { get; set; }

        [JsonPropertyName("top_p")]
        public double? TopP { get; set; }

        [JsonPropertyName("top_k")]
        public int? TopK { get; set; }

        [JsonPropertyName("num_predict")]
        public int? NumPredict { get; set; }

        [JsonPropertyName("repeat_penalty")]
        public double? RepeatPenalty { get; set; }

        [JsonPropertyName("seed")]
        public int? Seed { get; set; }
    }
}

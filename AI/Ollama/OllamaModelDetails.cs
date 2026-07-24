using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Jarvis.AI.Ollama
{
    public sealed class OllamaModelDetails
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("modified_at")]
        public DateTime ModifiedAt { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("digest")]
        public string Digest { get; set; } = string.Empty;

        [JsonPropertyName("details")]
        public OllamaModelInfo? Details { get; set; }
    }
}

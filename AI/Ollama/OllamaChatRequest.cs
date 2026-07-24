using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Jarvis.AI.Ollama
{
    public sealed class OllamaChatRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("messages")]
        public List<OllamaMessage> Messages { get; set; } = [];

        [JsonPropertyName("tools")]
        public List<OllamaTool> Tools { get; set; } = [];

        [JsonPropertyName("stream")]
        public bool Stream { get; set; }

        [JsonPropertyName("keep_alive")]
        public object? KeepAlive { get; set; }

        [JsonPropertyName("options")]
        public OllamaGenerateOptions? Options { get; set; }
    }
}

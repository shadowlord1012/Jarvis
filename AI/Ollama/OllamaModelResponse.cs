using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Jarvis.AI.Ollama
{
    public sealed class OllamaModelResponse
    {
        [JsonPropertyName("models")]
        public List<OllamaModelDetails> Models { get; set; } = [];
    }
}

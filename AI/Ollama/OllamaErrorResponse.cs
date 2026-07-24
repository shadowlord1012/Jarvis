using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Jarvis.AI.Ollama
{
    public sealed class OllamaErrorResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;
    }
}

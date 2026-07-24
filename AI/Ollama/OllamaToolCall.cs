using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Jarvis.AI.Ollama
{
    public sealed class OllamaToolCall
    {
        [JsonPropertyName("function")]
        public OllamaToolCallFunction Function { get; set; } = new();
    }
}

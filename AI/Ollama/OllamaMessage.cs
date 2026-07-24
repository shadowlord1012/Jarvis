using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Jarvis.AI.Ollama
{
    public sealed class OllamaMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("thinking")]
        public string? Thinking { get; set; }

        [JsonPropertyName("tool_calls")]
        public List<OllamaToolCall>? ToolCalls { get; set; }
    }
}

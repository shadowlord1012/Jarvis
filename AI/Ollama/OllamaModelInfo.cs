using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Jarvis.AI.Ollama
{
    public sealed class OllamaModelInfo
    {
        [JsonPropertyName("parent_model")]
        public string? ParentModel { get; set; }

        [JsonPropertyName("format")]
        public string? Format { get; set; }

        [JsonPropertyName("family")]
        public string? Family { get; set; }

        [JsonPropertyName("families")]
        public List<string>? Families { get; set; }

        [JsonPropertyName("parameter_size")]
        public string? ParameterSize { get; set; }

        [JsonPropertyName("quantization_level")]
        public string? QuantizationLevel { get; set; }
    }
}

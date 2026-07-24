using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Providers
{
    public sealed class LLMResponse
    {
        public string Content { get; set; } = string.Empty;

        public string? Thinking { get; set; }

        public string Model { get; set; } = string.Empty;

        public bool Done { get; set; }

        public string? FinishReason { get; set; }

        public IList<LLMToolCall> ToolCalls { get; set; }
            = new List<LLMToolCall>();

        public LLMUsage Usage { get; set; }
            = new();

        public TimeSpan? TotalDuration { get; set; }

        public IDictionary<string, object> Metadata { get; set; }
            = new Dictionary<string, object>();
    }
}

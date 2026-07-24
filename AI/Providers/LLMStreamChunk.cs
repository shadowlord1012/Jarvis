using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Providers
{
    public sealed class LLMStreamChunk
    {
        public string Content { get; set; } = string.Empty;

        public string? Thinking { get; set; }

        public string Model { get; set; } = string.Empty;

        public bool IsComplete { get; set; }

        public string? FinishReason { get; set; }

        public IList<LLMToolCall> ToolCalls { get; set; }
            = new List<LLMToolCall>();

        public LLMUsage Usage { get; set; }
            = new();
    }
}

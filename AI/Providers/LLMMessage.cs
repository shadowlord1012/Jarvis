using Jarvis.AI.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Providers
{
    public sealed class LLMMessage
    {
        public LLMRole Role { get; set; }

        public string Content { get; set; } = string.Empty;

        public string? Name { get; set; }

        public string? ToolCallId { get; set; }

        public IReadOnlyList<LLMToolCall> ToolCalls { get; set; }
            = [];
    }
}

using Jarvis.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI
{
    public sealed class AIResponse
    {
        public bool Success { get; init; }

        public string Response { get; init; } = string.Empty;

        public string Model { get; init; } = string.Empty;

        public int PromptTokens { get; init; }

        public int CompletionTokens { get; init; }

        public TimeSpan Duration { get; init; }

        public IReadOnlyList<ToolCallResult> ToolCalls { get; init; }
            = Array.Empty<ToolCallResult>();
    }
}

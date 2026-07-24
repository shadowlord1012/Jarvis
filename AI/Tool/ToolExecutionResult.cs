using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Tool
{
    public sealed class ToolExecutionResult
    {
        public bool Success { get; set; }

        public string Output { get; set; } = string.Empty;

        public string? Error { get; set; }

        public object? Data { get; set; }
    }
}

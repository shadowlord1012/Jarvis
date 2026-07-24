using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Tool
{
    public sealed class ToolExecutionRequest
    {
        public string ToolName { get; set; } = string.Empty;

        public string Arguments { get; set; } = "{}";

        public ToolContext Context { get; set; } = new();
    }
}

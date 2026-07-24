using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Providers
{
    public sealed class ToolDefinition
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string JsonSchema { get; set; } = "{}";
        public object? Parameters { get; init; }
    }
}

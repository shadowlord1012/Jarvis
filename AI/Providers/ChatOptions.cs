using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Providers
{
    public sealed class ChatOptions
    {
        public double? Temperature { get; set; }

        public double? TopP { get; set; }

        public int? MaxTokens { get; set; }

        public int? TopK { get; set; }

        public double? RepeatPenalty { get; set; }

        public int? Seed { get; set; }

        public bool EnableToolCalling { get; set; } = true;

        public bool EnableStreaming { get; set; } = true;

        public string? KeepAlive { get; set; }
    }
}

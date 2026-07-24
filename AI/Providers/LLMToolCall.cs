using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Providers
{
    public sealed class LLMToolCall
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = string.Empty;

        public string Arguments { get; set; } = "{}";
    }
}

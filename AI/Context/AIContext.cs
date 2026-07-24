using Jarvis.AI.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Context
{
    public sealed class AIContext
    {
        public string ConversationId { get; set; } = string.Empty;

        public string UserInput { get; set; } = string.Empty;

        public string SystemPrompt { get; set; } = string.Empty;

        public IList<LLMMessage> Messages { get; set; }
            = new List<LLMMessage>();

        public IList<ToolDefinition> Tools { get; set; }
            = new List<ToolDefinition>();

        public ChatOptions ChatOptions { get; set; }
            = new();

        public IDictionary<string, object> Metadata { get; set; }
            = new Dictionary<string, object>();
    }
}

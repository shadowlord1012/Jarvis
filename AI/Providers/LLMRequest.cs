using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Providers
{
    public sealed class LLMRequest
    {
        public string? Model { get; set; }

        public string? SystemPrompt { get; set; }

        public IList<LLMMessage> Messages { get; set; }
            = new List<LLMMessage>();

        public IList<ToolDefinition> Tools { get; set; }
            = new List<ToolDefinition>();

        public ChatOptions Options { get; set; }
            = new();

        public bool Stream { get; set; }

        public string? ConversationId { get; set; }

        public IDictionary<string, object> Metadata { get; set; }
            = new Dictionary<string, object>();
    }
}

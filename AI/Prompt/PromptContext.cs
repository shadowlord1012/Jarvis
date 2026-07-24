using Jarvis.AI.Conversation;
using Jarvis.AI.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Prompt
{
    public sealed class PromptContext
    {
        public IList<ConversationMessage> Conversation { get; }
            = new List<ConversationMessage>();

        public IList<string> Memories { get; }
            = new List<string>();

        public IList<ToolDefinition> Tools { get; }
            = new List<ToolDefinition>();

        public ChatOptions ChatOptions { get; set; } = new();

        public string UserInput { get; set; } = string.Empty;
    }
}

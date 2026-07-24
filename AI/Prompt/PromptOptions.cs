using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Prompt
{
    public sealed class PromptOptions
    {
        public bool IncludeSystemPrompt { get; set; } = true;

        public bool IncludeConversation { get; set; } = true;

        public bool IncludeMemory { get; set; } = true;

        public bool IncludeToolDefinitions { get; set; } = true;

        public int MaximumConversationMessages { get; set; } = 20;
    }
}

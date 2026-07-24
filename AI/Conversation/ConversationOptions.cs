using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Conversation
{
    public sealed class ConversationOptions
    {
        public int MaximumMessages { get; set; } = 100;

        public bool IncludeSystemPrompt { get; set; } = true;

        public bool TrimOldMessages { get; set; } = true;
    }
}

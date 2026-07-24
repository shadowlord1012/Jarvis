using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Tool
{
    public sealed class ToolContext
    {
        public Guid ConversationId { get; set; }

        public string UserInput { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

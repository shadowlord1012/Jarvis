using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Conversation
{
    public sealed class ConversationMessage
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public ConversationRole Role { get; init; }

        public string Content { get; set; } = string.Empty;

        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        public string? Name { get; init; }

        public IDictionary<string, object> Metadata { get; }
            = new Dictionary<string, object>();
    }
}

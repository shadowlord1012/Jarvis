using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Conversation
{
    public sealed class ConversationSession
    {
        public string ConversationId { get; init; } =
            Guid.NewGuid().ToString();

        public DateTime StartedAt { get; init; } =
            DateTime.UtcNow;

        public DateTime LastActivityAt { get; private set; } =
            DateTime.UtcNow;

        public bool IsActive { get; private set; } = true;

        public void UpdateActivity()
        {
            LastActivityAt =
                DateTime.UtcNow;
        }

        public void End()
        {
            IsActive = false;

            UpdateActivity();
        }
    }
}

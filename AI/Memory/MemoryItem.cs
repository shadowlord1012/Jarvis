using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Memory
{
    public sealed class MemoryItem
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public MemoryType Type { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime Created { get; init; } = DateTime.UtcNow;

        public DateTime LastAccessed { get; set; } = DateTime.UtcNow;

        public int AccessCount { get; set; }

        public double Importance { get; set; } = 0.5;

        public IDictionary<string, object> Metadata { get; }
            = new Dictionary<string, object>();
    }
}

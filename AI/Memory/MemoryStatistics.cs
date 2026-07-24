using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Memory
{
    public sealed class MemoryStatistics
    {
        public long TotalMemories { get; set; }

        public long ConversationMemories { get; set; }

        public long PreferenceMemories { get; set; }

        public long FactMemories { get; set; }

        public long TaskMemories { get; set; }

        public long PluginMemories { get; set; }

        public long TemporaryMemories { get; set; }

        public long SystemMemories { get; set; }

        public DateTime? OldestMemory { get; set; }

        public DateTime? NewestMemory { get; set; }

        public DateTime? LastAccessed { get; set; }

        public double AverageImportance { get; set; }

        public double AverageAccessCount { get; set; }

        public long CacheHits { get; set; }

        public long CacheMisses { get; set; }

        public double CacheHitRatio =>
            CacheHits + CacheMisses == 0
                ? 0
                : (double)CacheHits /
                  (CacheHits + CacheMisses);
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Memory
{
    public sealed class MemoryQuery
    {
        public string SearchText { get; set; }
            = string.Empty;

        public MemoryType? Type { get; set; }

        public int MaximumResults { get; set; } = 10;

        public bool OrderByImportance { get; set; } = true;

        public bool OrderByNewest { get; set; }

        public bool OrderByRelevance { get; set; } = true;
    }
}

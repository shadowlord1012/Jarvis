using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Memory
{
    public sealed class MemorySearchResult
    {
        public MemoryItem Memory { get; init; } = default!;

        public double Score { get; init; }
    }
}

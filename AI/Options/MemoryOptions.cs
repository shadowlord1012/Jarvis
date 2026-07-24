using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Options
{
    public sealed class MemoryOptions
    {
        public int MaximumItems { get; set; } = 10000;

        public bool EnableAutomaticCleanup { get; set; } = true;

        public bool CaseSensitiveSearch { get; set; } = false;
    }
}

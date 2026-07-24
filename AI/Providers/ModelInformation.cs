using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Providers
{
    public sealed class ModelInformation
    {
        public string Name { get; set; } = string.Empty;

        public string Family { get; set; } = string.Empty;

        public long Size { get; set; }

        public DateTime Modified { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace AI.Tool.Models
{
    public sealed class WebSearchParameters
    {
        public string Query { get; set; } = string.Empty;

        public string Mode { get; set; } = "search";

        public List<string> Items { get; set; } = new();

        public string Aspect { get; set; } = "general";
    }
}

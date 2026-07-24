using System;
using System.Collections.Generic;
using System.Text;

namespace AI.Tool.Models
{
    public sealed class WebSearchResult
    {
        public string Title { get; set; } = string.Empty;

        public string Snippet { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Providers
{
    public sealed class LLMUsage
    {
        public int PromptTokens { get; set; }

        public int CompletionTokens { get; set; }

        public int TotalTokens =>
            PromptTokens + CompletionTokens;
    }
}

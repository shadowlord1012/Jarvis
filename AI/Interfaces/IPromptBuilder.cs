using Jarvis.AI.Context;
using Jarvis.AI.Prompt;
using Jarvis.AI.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Interfaces
{
    public interface IPromptBuilder
    {
        LLMRequest Build(
            AIContext context);
    }
}

using Jarvis.AI.Context;
using Jarvis.AI.Prompt;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Interfaces
{
    public interface IContextManager
    {
        Task<AIContext> BuildContextAsync(
            string conversationId,
            string userInput,
            CancellationToken cancellationToken = default);

        Task ClearAsync(
            string conversationId,
            CancellationToken cancellationToken = default);
    }
}

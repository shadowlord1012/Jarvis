using AI.Enums;
using Jarvis.AI.Providers;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI.Interfaces
{
    public interface IAIService
    {
        Task<LLMResponse> ProcessAsync(
        string userInput,
        CancellationToken cancellationToken = default);

        IAsyncEnumerable<LLMStreamChunk>
            ProcessStreamingAsync(
                string userInput,
                CancellationToken cancellationToken = default);

        Task StartNewConversationAsync(
            CancellationToken cancellationToken = default);

        Task EndConversationAsync(
            CancellationToken cancellationToken = default);

        Task ClearConversationAsync(
            CancellationToken cancellationToken = default);
    }
}

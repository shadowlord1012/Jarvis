using Jarvis.AI.Conversation;
using Jarvis.AI.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Interfaces
{
    public interface IConversationService
    {
        ConversationSession CurrentSession { get; }

        Task<ConversationSession>
            StartNewConversationAsync(
                CancellationToken cancellationToken = default);

        Task EndConversationAsync(
            CancellationToken cancellationToken = default);

        Task AddUserMessageAsync(
            string content,
            CancellationToken cancellationToken = default);

        Task AddAssistantMessageAsync(
            string content,
            CancellationToken cancellationToken = default);

        Task AddToolMessageAsync(
            string toolName,
            string toolCallId,
            string content,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<LLMMessage>>
            GetMessagesAsync(
                CancellationToken cancellationToken = default);

        Task ClearCurrentConversationAsync(
            CancellationToken cancellationToken = default);
    }
}

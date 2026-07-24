using Jarvis.AI.Context;
using Jarvis.AI.Enums;
using Jarvis.AI.Interfaces;
using Jarvis.AI.Options;
using Jarvis.AI.Prompt;
using Jarvis.AI.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Interfaces;

namespace Jarvis.AI.Manager
{
    public sealed class ContextManager : IContextManager
    {
        private readonly ILogService _logger;

        private readonly IMemoryService _memoryService;

        private readonly IToolExecutor _toolExecutor;

        private readonly ContextOptions _options;

        private readonly object _lock = new();

        private readonly Dictionary<string,List<LLMMessage>> _conversations = new(StringComparer.OrdinalIgnoreCase);

        public ContextManager(
            ILogService logger,
            IMemoryService memoryService,
            IToolExecutor toolExecutor,
            IOptions<ContextOptions> options)
        {
            _logger = logger;

            _memoryService = memoryService;

            _toolExecutor = toolExecutor;

            _options = options.Value;
        }

        public async Task<AIContext> BuildContextAsync(
            string conversationId,
            string userInput,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(
                    conversationId))
            {
                throw new ArgumentException(
                    "Conversation ID cannot be empty.",
                    nameof(conversationId));
            }

            if (string.IsNullOrWhiteSpace(
                    userInput))
            {
                throw new ArgumentException(
                    "User input cannot be empty.",
                    nameof(userInput));
            }

            var messages =
                GetConversationMessages(
                    conversationId);

            var context =
                new AIContext
                {
                    ConversationId =
                        conversationId,

                    UserInput =
                        userInput,

                    Messages =
                        messages
                };

            if (_options.EnableMemory)
            {
                await AddMemoryContextAsync(
                    context,
                    cancellationToken);
            }

            if (_options.EnableTools)
            {
                var tools =
                    await _toolExecutor
                        .GetToolDefinitionsAsync(
                            cancellationToken);

                foreach (var tool in tools)
                {
                    context.Tools.Add(tool);
                }
            }

            _logger.LogDebug("Context Manager",String.Format("AI context built for conversation {0}. Messages: {1}, Tools: {2}.",
                conversationId,
                context.Messages.Count,
                context.Tools.Count));

            return context;
        }
        public Task ClearAsync(
            string conversationId,
            CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                _conversations.Remove(
                    conversationId);
            }

            _logger.LogDebug("Context Manager",String.Format("Conversation context cleared: {0}",conversationId));

            return Task.CompletedTask;
        }
        private void AddMessage(
            string conversationId,
            LLMMessage message)
        {
            lock (_lock)
            {
                if (!_conversations.TryGetValue(
                        conversationId,
                        out var messages))
                {
                    messages =
                        new List<LLMMessage>();

                    _conversations[
                        conversationId] =
                        messages;
                }

                messages.Add(message);

                TrimConversation(
                    messages);
            }
        }

        private List<LLMMessage>
            GetConversationMessages(
                string conversationId)
        {
            lock (_lock)
            {
                if (!_conversations.TryGetValue(
                        conversationId,
                        out var messages))
                {
                    return [];
                }

                return messages
                    .Select(CloneMessage)
                    .ToList();
            }
        }

        private void TrimConversation(
            List<LLMMessage> messages)
        {
            while (messages.Count >
                   _options.MaximumMessages)
            {
                messages.RemoveAt(0);
            }
        }

        private async Task AddMemoryContextAsync(
            AIContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                var memories =
                    await _memoryService.SearchAsync(
                        context.UserInput,
                        _options.MaximumMemories,
                        cancellationToken);

                if (memories is null ||
                    memories.Count == 0)
                {
                    return;
                }

                var memoryText =
                    string.Join(
                        Environment.NewLine,
                        memories.Select(
                            memory =>
                                $"- {memory}"));

                context.Metadata[
                    "MemoryContext"] =
                    memoryText;

                _logger.LogDebug("Context Manager",String.Format("Added {0} memory results to AI context.",memories.Count));
            }
            catch (Exception ex)
            {
                _logger.LogError("Context Manager",String.Format("Failed to load memory context: {0}",ex.Message));
            }
        }

        private static LLMMessage CloneMessage(
            LLMMessage message)
        {
            return new LLMMessage
            {
                Role =
                    message.Role,

                Content =
                    message.Content,

                Name =
                    message.Name,

                ToolCallId =
                    message.ToolCallId,

                ToolCalls =
                    message.ToolCalls
                        .Select(CloneToolCall)
                        .ToList()
            };
        }

        private static LLMToolCall CloneToolCall(
            LLMToolCall toolCall)
        {
            return new LLMToolCall
            {
                Id =
                    toolCall.Id,

                Name =
                    toolCall.Name,

                Arguments =
                    toolCall.Arguments
            };
        }
    }
}

using Jarvis.AI.Context;
using Jarvis.AI.Conversation;
using Jarvis.AI.Enums;
using Jarvis.AI.Interfaces;
using Jarvis.AI.Providers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Interfaces;

namespace Jarvis.AI.Prompt
{
    public sealed class PromptBuilder : IPromptBuilder
    {
        private readonly ILogService _logger;

        public PromptBuilder(
            ILogService logger)
        {
            _logger = logger;
        }

        public LLMRequest Build(
            AIContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(
                    nameof(context));
            }

            if (string.IsNullOrWhiteSpace(
                    context.UserInput))
            {
                throw new ArgumentException(
                    "User input cannot be empty.",
                    nameof(context));
            }

            var request =
                new LLMRequest
                {
                    Model = null,

                    SystemPrompt =
                        BuildSystemPrompt(
                            context),

                    Messages =
                        new List<LLMMessage>(
                            context.Messages),

                    Tools =
                        context.Tools,

                    Options =
                        context.ChatOptions,

                    Stream =
                        context.ChatOptions
                            .EnableStreaming,

                    ConversationId =
                        context.ConversationId,

                    Metadata =
                        new Dictionary<string, object>(
                            context.Metadata)
                };

            AddUserMessage(
                request,
                context.UserInput);

            _logger.LogDebug("LLM",String.Format("LLM request built for conversation {0}. Messages: {1}, Tools: {2}.",
                context.ConversationId,
                request.Messages.Count,
                request.Tools.Count));

            return request;
        }

        private static string BuildSystemPrompt(
            AIContext context)
        {
            var builder =
                new System.Text.StringBuilder();

            builder.AppendLine(
                "You are Jarvis, an intelligent personal AI assistant.");

            builder.AppendLine();

            builder.AppendLine(
                "Your behavior:");

            builder.AppendLine(
                "- Be professional, efficient, and direct.");

            builder.AppendLine(
                "- Do not unnecessarily repeat information.");

            builder.AppendLine(
                "- Use available tools when they are required to complete a task.");

            builder.AppendLine(
                "- Never claim that an action was completed unless the action actually succeeded.");

            builder.AppendLine(
                "- If a tool fails, clearly report the failure.");

            builder.AppendLine(
                "- Keep responses concise unless the user requests more detail.");

            builder.AppendLine();

            if (context.Metadata.TryGetValue(
                    "MemoryContext",
                    out var memoryContext))
            {
                builder.AppendLine(
                    "Relevant long-term memory:");

                builder.AppendLine(
                    memoryContext?.ToString());

                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static void AddUserMessage(
            LLMRequest request,
            string userInput)
        {
            request.Messages.Add(
                new LLMMessage
                {
                    Role =
                        LLMRole.User,

                    Content =
                        userInput
                });
        }
    }
}

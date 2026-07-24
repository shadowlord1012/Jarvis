using Jarvis.AI.Enums;
using Jarvis.AI.Interfaces;
using Jarvis.AI.Memory;
using Jarvis.AI.Providers;
using UI.Controls.HUD.Interfaces;

namespace Jarvis.AI.Conversation
{
    public sealed class ConversationService
    : IConversationService
    {
        private readonly ILogService _logger;

        private readonly IMemoryService _memoryService;

        private readonly ILLMProvider _llmProvider;

        private readonly SemaphoreSlim _lock =
            new(1, 1);

        private readonly List<LLMMessage> _messages =
            new();

        private ConversationSession _currentSession =
            new();

        public ConversationService(
            ILogService logger,
            IMemoryService memoryService,
            ILLMProvider llmProvider)
        {
            _logger =
                logger;

            _memoryService =
                memoryService;

            _llmProvider =
                llmProvider;
        }

        public ConversationSession CurrentSession =>
            _currentSession;

        // ============================================================
        // START NEW CONVERSATION
        // ============================================================

        public async Task<ConversationSession>
            StartNewConversationAsync(
                CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(
                cancellationToken);

            try
            {
                if (_currentSession.IsActive)
                {
                    _currentSession.End();

                    _logger.LogDebug("Conversation Service",String.Format("Previous conversation ended: {0}",_currentSession.ConversationId));
                }

                _messages.Clear();

                _currentSession =
                    new ConversationSession();

                _logger.LogDebug("Conversation Service",String.Format("New conversation started: {0}",_currentSession.ConversationId));

                return _currentSession;
            }
            finally
            {
                _lock.Release();
            }
        }

        // ============================================================
        // END CONVERSATION
        // ============================================================

        public async Task EndConversationAsync(
            CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(
                cancellationToken);

            try
            {
                if (!_currentSession.IsActive)
                {
                    return;
                }

                _currentSession.End();

                _logger.LogDebug("Conversation Service",String.Format("Conversation ended: {0}",_currentSession.ConversationId));
            }
            finally
            {
                _lock.Release();
            }
        }

        // ============================================================
        // ADD USER MESSAGE
        // ============================================================

        public async Task AddUserMessageAsync(
            string content,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            await _lock.WaitAsync(
                cancellationToken);

            try
            {
                EnsureActiveSession();

                var message =
                    new LLMMessage
                    {
                        Role =
                            LLMRole.User,

                        Content =
                            content
                    };

                _messages.Add(message);

                _currentSession.UpdateActivity();

                _logger.LogDebug("Conversation Service",String.Format("User message added to conversation {0}.",_currentSession.ConversationId));

                await PersistMessageAsync(
                    message,
                    cancellationToken);
            }
            finally
            {
                _lock.Release();
            }
        }

        // ============================================================
        // ADD ASSISTANT MESSAGE
        // ============================================================

        public async Task AddAssistantMessageAsync(
            string content,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            await _lock.WaitAsync(
                cancellationToken);

            try
            {
                EnsureActiveSession();

                var message =
                    new LLMMessage
                    {
                        Role =
                            LLMRole.Assistant,

                        Content =
                            content
                    };

                _messages.Add(message);

                _currentSession.UpdateActivity();

                _logger.LogDebug("Conversation Service",String.Format("Assistant message added to conversation {0}.",_currentSession.ConversationId));

                await PersistMessageAsync(
                    message,
                    cancellationToken);
            }
            finally
            {
                _lock.Release();
            }
        }

        // ============================================================
        // ADD TOOL MESSAGE
        // ============================================================

        public async Task AddToolMessageAsync(
            string toolName,
            string toolCallId,
            string content,
            CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(
                cancellationToken);

            try
            {
                EnsureActiveSession();

                var message =
                    new LLMMessage
                    {
                        Role =
                            LLMRole.Tool,

                        Name =
                            toolName,

                        ToolCallId =
                            toolCallId,

                        Content =
                            content
                    };

                _messages.Add(message);

                _currentSession.UpdateActivity();

                _logger.LogDebug("Conversation Service",String.Format("Tool message added. Tool: {0}.",toolName));
            }
            finally
            {
                _lock.Release();
            }
        }

        // ============================================================
        // GET ACTIVE MESSAGES
        // ============================================================

        public async Task<IReadOnlyList<LLMMessage>>
            GetMessagesAsync(
                CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(
                cancellationToken);

            try
            {
                return _messages
                    .Select(CloneMessage)
                    .ToList();
            }
            finally
            {
                _lock.Release();
            }
        }

        // ============================================================
        // CLEAR CURRENT CONVERSATION
        // ============================================================

        public async Task ClearCurrentConversationAsync(
            CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(
                cancellationToken);

            try
            {
                _messages.Clear();

                _currentSession.UpdateActivity();

                _logger.LogDebug("Conversation Service",String.Format("Active conversation context cleared: {0}",_currentSession.ConversationId));
            }
            finally
            {
                _lock.Release();
            }
        }

        // ============================================================
        // PERSIST MESSAGE
        // ============================================================

        private async Task PersistMessageAsync(
            LLMMessage message,
            CancellationToken cancellationToken)
        {
            try
            {
                var content = message.Content ?? string.Empty;

                if (string.IsNullOrWhiteSpace(content))
                {
                    return;
                }

                // Generate title and summary for the memory
                var (title, summary) = await GenerateSummaryAsync(
                    content,
                    message.Role,
                    cancellationToken);

                var memory =
                    new MemoryItem
                    {
                        Id =
                            Guid.NewGuid(),

                        Type =
                            MemoryType.Conversation,

                        Title =
                            title,

                        Content =
                            summary,

                        Created = DateTime.UtcNow
                    };

                await _memoryService.StoreAsync(
                    memory,
                    cancellationToken);

                _logger.LogDebug("Conversation Service", $"Persisted memory: {title}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Conversation Service",String.Format("Failed to persist conversation message. {0}",ex.Message));
            }
        }

        // ============================================================
        // GENERATE SUMMARY
        // ============================================================

        private async Task<(string title, string summary)> GenerateSummaryAsync(
            string content,
            LLMRole role,
            CancellationToken cancellationToken)
        {
            try
            {
                // For short messages, use content as-is with a simple title
                if (content.Length < 100)
                {
                    var shortTitle = content.Length > 50
                        ? content.Substring(0, 47) + "..."
                        : content;
                    return (shortTitle, content);
                }

                var systemPrompt =
                    """
                    You are a summarization assistant.
                    Create a concise title (max 60 characters) and a brief summary (max 200 characters) of the given text.
                    Respond in JSON format: {"title": "...", "summary": "..."}
                    """;

                var userPrompt =
                    $"""
                    Summarize this {(role == LLMRole.User ? "user question" : "assistant response")}:

                    {content}
                    """;

                var request = new LLMRequest
                {
                    Messages = new List<LLMMessage>
                    {
                        new()
                        {
                            Role = LLMRole.System,
                            Content = systemPrompt
                        },
                        new()
                        {
                            Role = LLMRole.User,
                            Content = userPrompt
                        }
                    }
                };

                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken,
                    timeoutCts.Token);

                var response = await _llmProvider.GenerateAsync(
                    request,
                    linkedCts.Token);

                if (string.IsNullOrWhiteSpace(response.Content))
                {
                    return (CreateFallbackTitle(content), content);
                }

                // Try to parse JSON response
                var jsonContent = response.Content.Trim();
                if (jsonContent.StartsWith("{") && jsonContent.EndsWith("}"))
                {
                    var json = System.Text.Json.JsonDocument.Parse(jsonContent);
                    var root = json.RootElement;

                    if (root.TryGetProperty("title", out var titleElement) &&
                        root.TryGetProperty("summary", out var summaryElement))
                    {
                        var title = titleElement.GetString() ?? CreateFallbackTitle(content);
                        var summary = summaryElement.GetString() ?? content;
                        return (title, summary);
                    }
                }

                // Fallback if JSON parsing fails
                return (CreateFallbackTitle(content), content);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Conversation Service", $"Summary generation failed: {ex.Message}");
                return (CreateFallbackTitle(content), content);
            }
        }

        private static string CreateFallbackTitle(string content)
        {
            var title = content.Length > 60
                ? content.Substring(0, 57) + "..."
                : content;

            // Remove newlines from title
            title = title.Replace("\n", " ").Replace("\r", "");

            return title;
        }

        // ============================================================
        // ENSURE ACTIVE SESSION
        // ============================================================

        private void EnsureActiveSession()
        {
            if (_currentSession.IsActive)
            {
                return;
            }

            _currentSession =
                new ConversationSession();

            _messages.Clear();

            _logger.LogDebug("Conversation Service",String.Format("Automatically created new conversation session: {0}",_currentSession.ConversationId));
        }

        // ============================================================
        // CLONE MESSAGE
        // ============================================================

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
                        .Select(
                            tool =>
                                new LLMToolCall
                                {
                                    Id =
                                        tool.Id,

                                    Name =
                                        tool.Name,

                                    Arguments =
                                        tool.Arguments
                                })
                        .ToList()
            };
        }
    }
}

using AI.Enums;
using AI.Interfaces;
using Jarvis.AI.Enums;
using Jarvis.AI.Interfaces;
using Jarvis.AI.Providers;
using Jarvis.AI.Tool;
using System.Runtime.CompilerServices;
using System.Text;
using UI.Controls.HUD.Interfaces;

namespace AI
{
    public sealed class AIService : IAIService
    {
        private readonly ILogService _logger;

        private readonly IContextManager _contextManager;

        private readonly IPromptBuilder _promptBuilder;

        private readonly ILLMProvider _llmProvider;

        private readonly IToolExecutor _toolExecutor;

        private readonly IConversationService
            _conversationService;

        private readonly IAIStateManager _stateManager;

        private const int MaximumToolIterations = 10;

        public AIService(
            ILogService logger,
            IContextManager contextManager,
            IPromptBuilder promptBuilder,
            ILLMProvider llmProvider,
            IToolExecutor toolExecutor,
            IConversationService conversationService,
            IAIStateManager stateManager)
        {
            _logger =
                logger;

            _contextManager =
                contextManager;

            _promptBuilder =
                promptBuilder;

            _llmProvider =
                llmProvider;

            _toolExecutor =
                toolExecutor;

            _conversationService =
                conversationService;

            _stateManager =
                stateManager;
        }

        // ============================================================
        // PROCESS REQUEST
        // ============================================================

        public async Task<LLMResponse> ProcessAsync(
            string userInput,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                throw new ArgumentException(
                    "User input cannot be empty.",
                    nameof(userInput));
            }

            var conversationId =
                _conversationService
                    .CurrentSession
                    .ConversationId;

            _logger.LogDebug("AI Service",String.Format("Processing AI request. Conversation: {0}",conversationId));

            _stateManager.SetState(AIState.Thinking);

            try
            {
                await _conversationService
                    .AddUserMessageAsync(
                        userInput,
                        cancellationToken);

                var context =
                    await _contextManager
                        .BuildContextAsync(
                            conversationId,
                            userInput,
                            cancellationToken);

                var messages =
                    await _conversationService
                        .GetMessagesAsync(
                            cancellationToken);

                context.Messages =
                    messages.ToList();

                var request =
                    _promptBuilder.Build(
                        context);

                var response =
                    await ExecuteLLMLoopAsync(
                        request,
                        cancellationToken);

                if (!string.IsNullOrWhiteSpace(
                        response.Content))
                {
                    await _conversationService
                        .AddAssistantMessageAsync(
                            response.Content,
                            cancellationToken);
                }

                _stateManager.SetState(AIState.Idle);

                return response;
            }
            catch (OperationCanceledException)
            {
                _stateManager.SetState(AIState.Idle);

                _logger.LogDebug("AI Service","AI request cancelled.");

                throw;
            }
            catch (Exception ex)
            {
                _stateManager.SetState(AIState.Error);

                _logger.LogError("AI Service",String.Format("AI request failed. {0}",ex.Message));

                throw;
            }
        }

        // ============================================================
        // STREAMING REQUEST
        // ============================================================

        public async IAsyncEnumerable<LLMStreamChunk>
            ProcessStreamingAsync(
                string userInput,
                [EnumeratorCancellation]
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userInput))
            {
                throw new ArgumentException(
                    "User input cannot be empty.",
                    nameof(userInput));
            }

            var responseBuilder =
                new StringBuilder();

            _stateManager.SetState(AIState.Thinking);


            var conversationId =
                _conversationService
                    .CurrentSession
                    .ConversationId;

            await _conversationService
                .AddUserMessageAsync(
                    userInput,
                    cancellationToken);

            var context =
                await _contextManager
                    .BuildContextAsync(
                        conversationId,
                        userInput,
                        cancellationToken);

            var messages =
                await _conversationService
                    .GetMessagesAsync(
                        cancellationToken);

            context.Messages =
                messages.ToList();

            var request =
                _promptBuilder.Build(
                    context);

            // Execute tool loop for streaming
            for (var iteration = 1;
                 iteration <= MaximumToolIterations;
                 iteration++)
            {
                _logger.LogDebug("AI Service", $"Streaming iteration {iteration}.");

                _stateManager.SetState(AIState.Thinking);

                request.Stream = false; // Use non-streaming for tool detection
                var initialResponse =
                    await _llmProvider
                        .GenerateAsync(
                            request,
                            cancellationToken);

                _logger.LogDebug("AI Service", $"LLM response received. Tool calls: {initialResponse.ToolCalls.Count}");

                // If no tools, stream the final response
                if (initialResponse.ToolCalls.Count == 0)
                {
                    request.Stream = true;

                    // Stream the response content
                    if (!string.IsNullOrEmpty(initialResponse.Content))
                    {
                        // Emit content as chunks for consistency
                        foreach (var word in initialResponse.Content.Split(' '))
                        {
                            yield return new LLMStreamChunk
                            {
                                Content = word + " ",
                                IsComplete = false
                            };
                        }

                        responseBuilder.Append(initialResponse.Content);
                    }

                    yield return new LLMStreamChunk
                    {
                        Content = string.Empty,
                        IsComplete = true
                    };

                    break;
                }

                // Execute tools
                _logger.LogDebug("AI Service", $"LLM requested {initialResponse.ToolCalls.Count} tool calls.");
                _stateManager.SetState(AIState.ExecutingTool);

                request.Messages.Add(
                    new LLMMessage
                    {
                        Role =
                            LLMRole.Assistant,

                        Content =
                            initialResponse.Content,

                        ToolCalls =
                            (IReadOnlyList<LLMToolCall>)initialResponse.ToolCalls
                    });

                foreach (var toolCall
                    in initialResponse.ToolCalls)
                {
                    _logger.LogDebug("AI Service", $"Executing tool: {toolCall.Name}");

                    var toolResult =
                        await ExecuteToolAsync(
                            toolCall,
                            cancellationToken);

                    _logger.LogDebug("AI Service", $"Tool {toolCall.Name} execution completed. Success: {toolResult.Success}");

                    // Add tool result to messages
                    request.Messages.Add(
                        new LLMMessage
                        {
                            Role =
                                LLMRole.Tool,

                            Name =
                                toolCall.Name,

                            ToolCallId =
                                toolCall.Id,

                            Content =
                                toolResult.Output
                        });

                    // Stream a notification about tool execution
                    yield return new LLMStreamChunk
                    {
                        Content = $"[{toolCall.Name} executed]",
                        IsComplete = false
                    };
                }

                // Continue loop to get LLM response with tool results
            }

            var finalResponse =
                responseBuilder.ToString();

            if (!string.IsNullOrWhiteSpace(
                    finalResponse))
            {
                await _conversationService
                    .AddAssistantMessageAsync(
                        finalResponse,
                        cancellationToken);
            }

            _stateManager.SetState(AIState.Idle);

        }

        // ============================================================
        // LLM / TOOL LOOP
        // ============================================================

        private async Task<LLMResponse>
            ExecuteLLMLoopAsync(
                LLMRequest request,
                CancellationToken cancellationToken)
        {
            for (var iteration = 1;
                 iteration <= MaximumToolIterations;
                 iteration++)
            {
                _logger.LogDebug("AI Service",String.Format("LLM execution iteration {0}.",iteration));

                _stateManager.SetState(AIState.Thinking);

                _logger.LogDebug("AI Service", "Calling LLM provider...");

                var response =
                    await _llmProvider
                        .GenerateAsync(
                            request,
                            cancellationToken);

                _logger.LogDebug("AI Service", $"LLM response received. Tool calls: {response.ToolCalls.Count}");

                if (response.ToolCalls.Count == 0)
                {
                    return response;
                }

                _logger.LogDebug("Ai Service",String.Format("LLM requested {0} tool calls.",response.ToolCalls.Count));

                _stateManager.SetState(AIState.ExecutingTool);

                request.Messages.Add(
                    new LLMMessage
                    {
                        Role =
                            LLMRole.Assistant,

                        Content =
                            response.Content,

                        ToolCalls =
                            (IReadOnlyList<LLMToolCall>)response.ToolCalls
                    });

                foreach (var toolCall
                    in response.ToolCalls)
                {
                    _logger.LogDebug("AI Service", $"Executing tool: {toolCall.Name}");

                    var toolResult =
                        await ExecuteToolAsync(
                            toolCall,
                            cancellationToken);

                    _logger.LogDebug("AI Service", $"Tool {toolCall.Name} execution completed. Success: {toolResult.Success}");

                    request.Messages.Add(
                        new LLMMessage
                        {
                            Role =
                                LLMRole.Tool,

                            Name =
                                toolCall.Name,

                            ToolCallId =
                                toolCall.Id,

                            Content =
                                toolResult.Output
                        });
                }
            }

            throw new InvalidOperationException(
                $"Maximum tool execution iterations ({MaximumToolIterations}) exceeded.");
        }

        // ============================================================
        // TOOL EXECUTION
        // ============================================================

        private async Task<ToolExecutionResult>
            ExecuteToolAsync(
                LLMToolCall toolCall,
                CancellationToken cancellationToken)
        {
            _logger.LogDebug("AI Service",String.Format("Executing AI requested tool: {0}",toolCall.Name));

            var session =
                _conversationService
                    .CurrentSession;

            var request =
                new ToolExecutionRequest
                {
                    ToolName =
                        toolCall.Name,

                    Arguments =
                        toolCall.Arguments,

                    Context =
                        new ToolContext
                        {
                            ConversationId =
                                Guid.Parse(
                                    session.ConversationId),

                            UserInput =
                                string.Empty,

                            Timestamp =
                                DateTime.UtcNow
                        }
                };

            var result =
                await _toolExecutor
                    .ExecuteAsync(
                        request,
                        cancellationToken);

            await _conversationService
                .AddToolMessageAsync(
                    toolCall.Name,
                    toolCall.Id,
                    result.Output,
                    cancellationToken);

            if (!result.Success)
            {
                _logger.LogWarning("AI Service",String.Format("Tool {0} failed: {1}",toolCall.Name,result.Error));
            }

            return result;
        }

        // ============================================================
        // START NEW CONVERSATION
        // ============================================================

        public async Task StartNewConversationAsync(
            CancellationToken cancellationToken = default)
        {
            await _conversationService
                .StartNewConversationAsync(
                    cancellationToken);

            _stateManager.SetState(AIState.Idle);

            _logger.LogDebug("AI Service","AI started a new conversation.");
        }

        // ============================================================
        // END CONVERSATION
        // ============================================================

        public async Task EndConversationAsync(
            CancellationToken cancellationToken = default)
        {
            await _conversationService
                .EndConversationAsync(
                    cancellationToken);

            _stateManager.SetState(AIState.Idle);

            _logger.LogDebug("AI Service","AI conversation ended.");
        }

        // ============================================================
        // CLEAR ACTIVE CONTEXT
        // ============================================================

        public async Task ClearConversationAsync(
            CancellationToken cancellationToken = default)
        {
            await _conversationService
                .ClearCurrentConversationAsync(
                    cancellationToken);

            await _contextManager
                .ClearAsync(
                    _conversationService
                        .CurrentSession
                        .ConversationId,
                    cancellationToken);

            _logger.LogDebug("AI Service", "AI active conversation context cleared.");
        }
    }
}

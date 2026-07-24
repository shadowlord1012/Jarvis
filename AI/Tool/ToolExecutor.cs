using Jarvis.AI.Interfaces;
using Jarvis.AI.Providers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Interfaces;

namespace Jarvis.AI.Tool
{
    public sealed class ToolExecutor : IToolExecutor
    {
        private readonly ILogService _logger;

        private readonly IToolRegistry _registry;

        public ToolExecutor(
            ILogService logger,
            IToolRegistry registry)
        {
            _logger = logger;
            _registry = registry;
        }

        public async Task<ToolExecutionResult> ExecuteAsync(
            ToolExecutionRequest request,
            CancellationToken cancellationToken = default)
        {
            var tool = _registry.Get(request.ToolName);

            if (tool is null)
            {
                _logger.LogWarning("Tool",String.Format("Tool '{0}' not found.", request.ToolName));

                return new ToolExecutionResult
                {
                    Success = false,
                    Error = $"Tool '{request.ToolName}' was not found."
                };
            }

            try
            {
                _logger.LogDebug("Tool", String.Format("Executing tool {0}",tool.Name));

                var result = await tool.ExecuteAsync(
                    request,
                    cancellationToken);

                _logger.LogDebug("Tool", String.Format("Tool {0} completed successfully.", tool.Name));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Tool", String.Format("Tool {0} failed. {1}", tool.Name, ex.Message));

                return new ToolExecutionResult
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        public Task<IReadOnlyList<ToolDefinition>> GetToolDefinitionsAsync(
            CancellationToken cancellationToken = default)
        {
            return _registry.GetDefinitionsAsync(cancellationToken);
        }
    }
}

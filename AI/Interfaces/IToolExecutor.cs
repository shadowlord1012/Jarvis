using Jarvis.AI.Providers;
using Jarvis.AI.Tool;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Interfaces
{
    public interface IToolExecutor
    {
        Task<ToolExecutionResult> ExecuteAsync(
            ToolExecutionRequest request,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ToolDefinition>> GetToolDefinitionsAsync(
            CancellationToken cancellationToken = default);
    }
}

using Jarvis.AI.Providers;
using Jarvis.AI.Tool;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Interfaces
{
    public interface ITool
    {
        string Name { get; }

        string Description { get; }

        ToolDefinition Definition { get; }

        Task<ToolExecutionResult> ExecuteAsync(
            ToolExecutionRequest request,
            CancellationToken cancellationToken = default);
    }
}

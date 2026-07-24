using Jarvis.AI.Interfaces;
using Jarvis.AI.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Tool.Test
{
    public sealed class TimeTool : ITool
    {
        public string Name => "get_time";

        public string Description => "Returns the current system time.";

        public ToolDefinition Definition => new()
        {
            Name = Name,
            Description = Description,
            JsonSchema =
            """
        {
            "type":"object",
            "properties":{}
        }
        """
        };

        public Task<ToolExecutionResult> ExecuteAsync(
            ToolExecutionRequest request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new ToolExecutionResult
            {
                Success = true,
                Output = DateTime.Now.ToString("F")
            });
        }
    }
}

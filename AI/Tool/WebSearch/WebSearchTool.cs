using AI.Interfaces;
using AI.Tool.Models;
using Jarvis.AI.Interfaces;
using Jarvis.AI.Providers;
using Jarvis.AI.Tool;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AI.Tool.WebSearch
{
    public sealed class WebSearchTool : ITool
    {
        private readonly IWebSearchService _webSearchService;

        public WebSearchTool(
            IWebSearchService webSearchService)
        {
            _webSearchService = webSearchService;
        }

        public string Name => "web_search";

        public string Description =>
            "Searches the internet for current information " +
            "or compares multiple items using current web results. " +
            "Use mode 'search' for a normal web search. " +
            "Use mode 'compare' when comparing multiple items.";

        public ToolDefinition Definition => new()
        {
            Name = Name,
            Description = Description,
            JsonSchema =
            """
            {
                "type": "object",
                "properties": {
                    "query": {
                        "type": "string",
                        "description": "The user's search query."
                    },
                    "mode": {
                        "type": "string",
                        "description": "The search mode: 'search' or 'compare'.",
                        "enum": ["search", "compare"]
                    },
                    "items": {
                        "type": "array",
                        "items": { "type": "string" },
                        "description": "Items to compare (only for compare mode)."
                    },
                    "aspect": {
                        "type": "string",
                        "description": "The specific aspect to compare (only for compare mode)."
                    }
                }
            }
            """
        };

        public async Task<ToolExecutionResult> ExecuteAsync(
            ToolExecutionRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var parameters = ParseArguments(request.Arguments);

                if (parameters is null)
                {
                    return new ToolExecutionResult
                    {
                        Success = false,
                        Error = "Invalid web search parameters, sir."
                    };
                }

                var result = await _webSearchService.SearchAsync(
                    parameters,
                    cancellationToken);

                return new ToolExecutionResult
                {
                    Success = true,
                    Output = result
                };
            }
            catch (JsonException)
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = "Invalid web search request format, sir."
                };
            }
            catch (Exception ex)
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = $"Web search failed, sir: {ex.Message}"
                };
            }
        }

        private WebSearchParameters? ParseArguments(string arguments)
        {
            if (string.IsNullOrWhiteSpace(arguments))
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<WebSearchParameters>(
                    arguments,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch
            {
                return null;
            }
        }
    }
}

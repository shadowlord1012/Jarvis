using AI.Interfaces;
using AI.Tool.Models;
using Jarvis.AI.Enums;
using Jarvis.AI.Interfaces;
using Jarvis.AI.Providers;
using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Interfaces;

namespace AI.Tool.WebSearch
{
    public sealed class WebSearchService : IWebSearchService
    {
        private readonly IDuckDuckGoSearchClient _searchClient;
        private readonly ILLMProvider _llmProvider;
        private readonly ILogService _logService;

        public WebSearchService(
            IDuckDuckGoSearchClient searchClient,
            ILLMProvider llmProvider,
            ILogService logService)
        {
            _searchClient = searchClient;
            _llmProvider = llmProvider;
            _logService = logService;
        }

        public async Task<string> SearchAsync(
            WebSearchParameters parameters,
            CancellationToken cancellationToken = default)
        {
            parameters ??= new WebSearchParameters();

            var query = parameters.Query?.Trim() ?? string.Empty;

            var mode = parameters.Mode?
                .Trim()
                .ToLowerInvariant() ?? "search";

            var items = parameters.Items ?? new List<string>();

            var aspect = string.IsNullOrWhiteSpace(parameters.Aspect)
                ? "general"
                : parameters.Aspect.Trim();

            if (string.IsNullOrWhiteSpace(query) &&
                items.Count == 0)
            {
                return "Please provide a search query, sir.";
            }

            if (items.Count > 0 &&
                mode != "compare")
            {
                mode = "compare";
            }

            var logQuery = string.IsNullOrWhiteSpace(query)
                ? string.Join(", ", items)
                : query;

            _logService.LogInfo(
                "WebSearch",
                $"[Search] {logQuery}");

            try
            {
                if (mode == "compare" &&
                    items.Count > 0)
                {
                    _logService.LogInfo(
                        "WebSearch",
                        $"[WebSearch] Comparing: {string.Join(", ", items)}");

                    return await CompareAsync(
                        items,
                        aspect,
                        cancellationToken);
                }

                var results =
                    await _searchClient.SearchAsync(
                        query,
                        6,
                        cancellationToken);

                var rawResults =
                    FormatResults(
                        query,
                        results);

                _logService.LogInfo(
                    "WebSearch",
                    $"[WebSearch] Search returned {results.Count} result(s).");

                return await SummarizeAsync(
                    query,
                    rawResults,
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logService.LogInfo(
                    "WebSearch",
                    "[WebSearch] Search cancelled.");

                throw;
            }
            catch (Exception ex)
            {
                _logService.LogError(
                    "WebSearch",
                    $"[WebSearch] Search failed: {ex.Message}");

                return $"Search failed, sir: {ex.Message}";
            }
        }

        public async Task<string> CompareAsync(
            IReadOnlyCollection<string> items,
            string aspect,
            CancellationToken cancellationToken = default)
        {
            if (items.Count == 0)
            {
                return "Please provide items to compare, sir.";
            }

            aspect = string.IsNullOrWhiteSpace(aspect)
                ? "general"
                : aspect.Trim();

            var allResults =
                new Dictionary<string, IReadOnlyList<WebSearchResult>>();

            foreach (var item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var results =
                        await _searchClient.SearchAsync(
                            $"{item} {aspect}",
                            3,
                            cancellationToken);

                    allResults[item] = results;
                }
                catch (Exception ex)
                {
                    _logService.LogError(
                        "WebSearch",
                        $"Comparison search failed for {item}: {ex.Message}");

                    allResults[item] =
                        Array.Empty<WebSearchResult>();
                }
            }

            var builder = new StringBuilder();

            builder.AppendLine(
                $"Comparison — {aspect.ToUpperInvariant()}");

            builder.AppendLine(
                new string('─', 40));

            foreach (var item in items)
            {
                builder.AppendLine();
                builder.AppendLine($"▸ {item}");

                if (!allResults.TryGetValue(
                        item,
                        out var results))
                {
                    continue;
                }

                foreach (var result in results.Take(2))
                {
                    if (!string.IsNullOrWhiteSpace(
                            result.Snippet))
                    {
                        builder.AppendLine(
                            $"  • {result.Snippet}");
                    }
                }
            }

            var raw = builder.ToString();

            return await SummarizeAsync(
                $"Compare {string.Join(", ", items)} regarding {aspect}",
                raw,
                cancellationToken);
        }

        private async Task<string> SummarizeAsync(
            string query,
            string rawResults,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(rawResults))
            {
                return "No search results were found, sir.";
            }

            try
            {
                var limitedResults =
                    rawResults.Length > 4000
                        ? rawResults[..4000]
                        : rawResults;

                var systemPrompt =
                    """
                You are JARVIS.
                Summarize web search results clearly and concisely.
                Answer the user's query directly.
                Be factual.
                Address the user as "sir".
                Do not invent information that is not supported by the search results.
                """;

                var prompt =
                    $"""
                User question:
                {query}

                Web search results:
                {limitedResults}

                Answer the question based on these results.
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
                        Content = prompt
                    }
                }
                };

                _logService.LogDebug(
                    "WebSearch",
                    "[WebSearch] Requesting LLM summarization of search results...");

                // Create a timeout for the LLM call to prevent indefinite hanging
                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                var response =
                    await _llmProvider.GenerateAsync(
                        request,
                        linkedCts.Token);

                _logService.LogDebug(
                    "WebSearch",
                    "[WebSearch] LLM summarization completed successfully.");

                return response?.Content
                    ?? rawResults;
            }
            catch (OperationCanceledException)
            {
                _logService.LogWarning(
                    "WebSearch",
                    "[WebSearch] LLM summarization timed out or was cancelled. Returning raw results.");

                return rawResults;
            }
            catch (Exception ex)
            {
                _logService.LogError(
                    "WebSearch",
                    $"[WebSearch] LLM summarization failed: {ex.Message}");

                // Same fallback behavior as Python.
                return rawResults;
            }
        }

        private static string FormatResults(
            string query,
            IReadOnlyList<WebSearchResult> results)
        {
            if (results.Count == 0)
            {
                return $"No results found for: {query}";
            }

            var builder = new StringBuilder();

            builder.AppendLine(
                $"Search results for: {query}");

            builder.AppendLine();

            for (var i = 0;
                 i < results.Count;
                 i++)
            {
                var result = results[i];

                if (!string.IsNullOrWhiteSpace(
                        result.Title))
                {
                    builder.AppendLine(
                        $"{i + 1}. {result.Title}");
                }

                if (!string.IsNullOrWhiteSpace(
                        result.Snippet))
                {
                    builder.AppendLine(
                        $"   {result.Snippet}");
                }

                if (!string.IsNullOrWhiteSpace(
                        result.Url))
                {
                    builder.AppendLine(
                        $"   {result.Url}");
                }

                builder.AppendLine();
            }

            return builder.ToString().Trim();
        }
    }
}

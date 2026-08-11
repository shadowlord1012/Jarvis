using AI.Interfaces;
using AI.Tool.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UI.Controls.HUD.Interfaces;

namespace AI.Tool.WebSearch
{
    public sealed class DuckDuckGoSearchClient : IDuckDuckGoSearchClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogService _logService;

        public DuckDuckGoSearchClient(
            HttpClient httpClient,
            ILogService logService)
        {
            _httpClient = httpClient;
            _logService = logService;

            _httpClient.Timeout = TimeSpan.FromSeconds(15);

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Jarvis/1.0");
        }

        public async Task<IReadOnlyList<WebSearchResult>> SearchAsync(
            string query,
            int maxResults = 6,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Array.Empty<WebSearchResult>();
            }

            try
            {
                var encodedQuery = Uri.EscapeDataString(query);

                var url =
                    $"https://html.duckduckgo.com/html/?q={encodedQuery}";

                _logService.LogDebug(
                    "WebSearch",
                    $"Requesting URL: {url}");

                using var response = await _httpClient.GetAsync(
                    url,
                    cancellationToken);

                _logService.LogDebug(
                    "WebSearch",
                    $"Response status: {response.StatusCode}");

                response.EnsureSuccessStatusCode();

                var html = await response.Content.ReadAsStringAsync(
                    cancellationToken);

                _logService.LogDebug(
                    "WebSearch",
                    $"Received HTML length: {html.Length} characters");

                var results = ParseResults(html, maxResults);

                _logService.LogDebug(
                    "WebSearch",
                    $"ParseResults returned {results.Count} results from {html.Length} char HTML");

                return results;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logService.LogError(
                    "WebSearch",
                    $"DuckDuckGo search failed: {ex.Message}");

                return Array.Empty<WebSearchResult>();
            }
        }

        private static List<WebSearchResult> ParseResults(
            string html,
            int maxResults)
        {
            var results = new List<WebSearchResult>();

            if (string.IsNullOrWhiteSpace(html))
            {
                return results;
            }

            // DEBUG: Save HTML response to file for inspection
            try
            {
                var debugPath = Path.Combine(Path.GetTempPath(), $"duckduckgo_response_{DateTime.Now:yyyyMMdd_HHmmss}.html");
                File.WriteAllText(debugPath, html);
                Console.WriteLine($"[DuckDuckGo DEBUG] HTML response saved to: {debugPath}");
            }
            catch
            {
                // Ignore file writing errors - this is just for debugging
            }

            // Basic HTML result extraction.
            // This intentionally avoids depending on a third-party
            // HTML parsing package.

            // Try multiple parsing strategies as DuckDuckGo's HTML structure may vary

            // Strategy 1: Look for result__body class (original approach)
            var resultBlocks = html.Split(
                "result__body",
                StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine($"[DuckDuckGo DEBUG] Found {resultBlocks.Length} blocks containing 'result__body'");

            // Strategy 2: If no results, try links-results wrapper
            if (resultBlocks.Length <= 1)
            {
                resultBlocks = html.Split(
                    new[] { "<div class=\"result", "<article class=\"result" },
                    StringSplitOptions.RemoveEmptyEntries);
                Console.WriteLine($"[DuckDuckGo DEBUG] Fallback strategy: Found {resultBlocks.Length} result divs");
            }

            foreach (var block in resultBlocks)
            {
                if (results.Count >= maxResults)
                {
                    break;
                }

                // Try multiple title extraction patterns
                var title = ExtractBetween(block, "result__a\">", "</a>");
                if (string.IsNullOrWhiteSpace(title))
                {
                    title = ExtractBetween(block, "class=\"result__title\">", "</a>");
                }
                if (string.IsNullOrWhiteSpace(title))
                {
                    title = ExtractBetween(block, "<h2", "</h2>");
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        // Extract text after the last '>'
                        var lastBracket = title.LastIndexOf('>');
                        if (lastBracket >= 0 && lastBracket < title.Length - 1)
                        {
                            title = title.Substring(lastBracket + 1);
                        }
                    }
                }

                var url = ExtractUrl(block);

                // Try multiple snippet extraction patterns
                var snippet = ExtractBetween(block, "result__snippet\">", "</a>");
                if (string.IsNullOrWhiteSpace(snippet))
                {
                    snippet = ExtractBetween(block, "class=\"result__snippet\">", "</div>");
                }

                Console.WriteLine($"[DuckDuckGo DEBUG] Block {results.Count}: Title='{title?.Substring(0, Math.Min(50, title?.Length ?? 0))}', URL='{url?.Substring(0, Math.Min(80, url?.Length ?? 0))}'");

                if (string.IsNullOrWhiteSpace(title) &&
                    string.IsNullOrWhiteSpace(snippet))
                {
                    continue;
                }

                results.Add(new WebSearchResult
                {
                    Title = WebUtility.HtmlDecode(
                        StripHtml(title).Trim()),

                    Snippet = WebUtility.HtmlDecode(
                        StripHtml(snippet).Trim()),

                    Url = WebUtility.HtmlDecode(
                        url.Trim())
                });
            }

            return results;
        }

        private static string ExtractBetween(
            string source,
            string start,
            string end)
        {
            var startIndex = source.IndexOf(
                start,
                StringComparison.OrdinalIgnoreCase);

            if (startIndex < 0)
            {
                return string.Empty;
            }

            startIndex += start.Length;

            var endIndex = source.IndexOf(
                end,
                startIndex,
                StringComparison.OrdinalIgnoreCase);

            if (endIndex < 0)
            {
                return string.Empty;
            }

            return source[
                startIndex..endIndex];
        }

        private static string ExtractUrl(string block)
        {
            var hrefIndex = block.IndexOf(
                "href=\"",
                StringComparison.OrdinalIgnoreCase);

            if (hrefIndex < 0)
            {
                return string.Empty;
            }

            hrefIndex += 6;

            var endIndex = block.IndexOf(
                "\"",
                hrefIndex,
                StringComparison.OrdinalIgnoreCase);

            if (endIndex < 0)
            {
                return string.Empty;
            }

            return block[
                hrefIndex..endIndex];
        }

        private static string StripHtml(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var result = value;

            while (true)
            {
                var start = result.IndexOf('<');

                if (start < 0)
                {
                    break;
                }

                var end = result.IndexOf(
                    '>',
                    start);

                if (end < 0)
                {
                    break;
                }

                result = result.Remove(
                    start,
                    end - start + 1);
            }

            return result;
        }
    }
}

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

                using var response = await _httpClient.GetAsync(
                    url,
                    cancellationToken);

                response.EnsureSuccessStatusCode();

                var html = await response.Content.ReadAsStringAsync(
                    cancellationToken);

                return ParseResults(html, maxResults);
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

            // Basic HTML result extraction.
            // This intentionally avoids depending on a third-party
            // HTML parsing package.

            var resultBlocks = html.Split(
                "result__body",
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var block in resultBlocks)
            {
                if (results.Count >= maxResults)
                {
                    break;
                }

                var title = ExtractBetween(
                    block,
                    "result__a\">",
                    "</a>");

                var url = ExtractUrl(block);

                var snippet = ExtractBetween(
                    block,
                    "result__snippet\">",
                    "</a>");

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

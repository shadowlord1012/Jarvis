using AI.Tool.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI.Interfaces
{
    public interface IDuckDuckGoSearchClient
    {
        Task<IReadOnlyList<WebSearchResult>> SearchAsync(
            string query,
            int maxResults = 6,
            CancellationToken cancellationToken = default);
    }
}

using AI.Tool.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI.Interfaces
{
    public interface IWebSearchService
    {
        Task<string> SearchAsync(
            WebSearchParameters parameters,
            CancellationToken cancellationToken = default);

        Task<string> CompareAsync(
            IReadOnlyCollection<string> items,
            string aspect,
            CancellationToken cancellationToken = default);
    }
}

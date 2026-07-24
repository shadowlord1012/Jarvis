using Jarvis.AI.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Interfaces
{
    public interface IMemoryService
    {
        Task InitializeAsync(
            CancellationToken cancellationToken = default);

        Task StoreAsync(
            MemoryItem memory,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<string>> SearchAsync(
            string query,
            int maximumResults = 5,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<MemorySearchResult>> SearchDetailedAsync(
            string query,
            int maximumResults = 5,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task ClearAsync(
            CancellationToken cancellationToken = default);

        IReadOnlyCollection<MemoryItem> GetAll();
    }
}

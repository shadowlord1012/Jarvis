using Jarvis.AI.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Interfaces
{
    public interface IMemoryRepository
    {
        Task InitializeAsync(
            CancellationToken cancellationToken = default);

        Task SaveAsync(
            MemoryItem item,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            MemoryItem item,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<MemoryItem?> GetAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<MemoryItem>> SearchAsync(
            MemoryQuery query,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<MemoryItem>> GetRecentAsync(
            int count,
            CancellationToken cancellationToken = default);

        Task<long> CountAsync(
            CancellationToken cancellationToken = default);

        Task<MemoryStatistics> GetStatisticsAsync(
            CancellationToken cancellationToken = default);
    }
}

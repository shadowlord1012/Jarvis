using Jarvis.AI.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Interfaces;

namespace Jarvis.AI.Memory
{
    public abstract class MemoryRepositoryBase : IMemoryRepository
    {
        protected readonly ILogService Logger;

        protected MemoryRepositoryBase(
            ILogService logger)
        {
            Logger = logger;
        }

        public virtual Task InitializeAsync(
            CancellationToken cancellationToken = default)
        {
            Logger.LogDebug(
                "{Repository} initialized.",
                GetType().Name);

            return Task.CompletedTask;
        }

        public abstract Task SaveAsync(
            MemoryItem item,
            CancellationToken cancellationToken = default);

        public abstract Task UpdateAsync(
            MemoryItem item,
            CancellationToken cancellationToken = default);

        public abstract Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        public abstract Task<MemoryItem?> GetAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        public abstract Task<IReadOnlyList<MemoryItem>> SearchAsync(
            MemoryQuery query,
            CancellationToken cancellationToken = default);

        public abstract Task<IReadOnlyList<MemoryItem>> GetRecentAsync(
            int count,
            CancellationToken cancellationToken = default);

        public abstract Task<long> CountAsync(
            CancellationToken cancellationToken = default);
        
        public abstract Task<MemoryStatistics> GetStatisticsAsync(
            CancellationToken cancellationToken = default);
    }
}

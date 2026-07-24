using Dapper;
using Jarvis.AI.Interfaces;
using Jarvis.AI.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Interfaces;

namespace Jarvis.AI.Storage.MariaDB
{
    public sealed class MariaDbMemoryRepository
    : MemoryRepositoryBase
    {
        private readonly IDatabaseConnectionFactory _factory;

        private readonly IMemorySqlProvider _sql;

        public MariaDbMemoryRepository(
            ILogService logger,
            IDatabaseConnectionFactory factory,
            IMemorySqlProvider sql)
            : base(logger)
        {
            _factory = factory;
            _sql = sql;
        }

        public override async Task SaveAsync(
            MemoryItem item,
            CancellationToken cancellationToken = default)
        {
            using var connection =
                await _factory.CreateConnectionAsync(cancellationToken);

            await connection.ExecuteAsync(_sql.Insert, item);
        }

        public override async Task UpdateAsync(
            MemoryItem item,
            CancellationToken cancellationToken = default)
        {
            using var connection =
                await _factory.CreateConnectionAsync(cancellationToken);

            await connection.ExecuteAsync(_sql.Update, item);
        }

        public override async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            using var connection =
                await _factory.CreateConnectionAsync(cancellationToken);

            await connection.ExecuteAsync(
                _sql.Delete,
                new { Id = id });
        }

        public override async Task<MemoryItem?> GetAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            using var connection =
                await _factory.CreateConnectionAsync(cancellationToken);

            return await connection.QueryFirstOrDefaultAsync<MemoryItem>(
                _sql.GetById,
                new { Id = id });
        }

        public override async Task<IReadOnlyList<MemoryItem>> SearchAsync(
            MemoryQuery query,
            CancellationToken cancellationToken = default)
        {
            using var connection =
                await _factory.CreateConnectionAsync(cancellationToken);

            var memories =
             await connection.QueryAsync<MemoryItem>(
                 _sql.Search,
                 new
                 {
                     Search = $"%{query.SearchText}%",
                     Limit = query.MaximumResults
                 });

            return memories.ToList();
        }

        public override async Task<IReadOnlyList<MemoryItem>> GetRecentAsync(
            int count,
            CancellationToken cancellationToken = default)
        {
            using var connection =
                await _factory.CreateConnectionAsync(cancellationToken);

            return (await connection.QueryAsync<MemoryItem>(
                _sql.GetRecent,
                new { Count = count })).ToList();
        }

        public override async Task<long> CountAsync(
            CancellationToken cancellationToken = default)
        {
            using var connection =
                await _factory.CreateConnectionAsync(cancellationToken);

            return await connection.ExecuteScalarAsync<long>(
                _sql.Count);    
        }

        public override async Task<MemoryStatistics> GetStatisticsAsync(
            CancellationToken cancellationToken = default)
        {
            using var connection =
                await _factory.CreateConnectionAsync(cancellationToken);

            var total = await CountAsync(cancellationToken);

            return new MemoryStatistics
            {
                TotalMemories = total
            };
        }
    }
}

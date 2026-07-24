using Jarvis.AI.Interfaces;
using Jarvis.AI.Options;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Interfaces;

namespace Jarvis.AI.Memory
{
    public sealed class MemoryService : IMemoryService
    {
        private readonly ILogService _logger;

        private readonly MemoryOptions _options;

        private readonly MemoryCache _cache;

        private readonly IMemoryRepository _repository;


        public MemoryService(
            ILogService logger,
            MemoryOptions options,
            MemoryCache cache,
            IMemoryRepository repository)
        {
            _logger = logger;
            _options = options;
            _cache = cache;
            _repository = repository;
        }

        public async Task InitializeAsync(
            CancellationToken cancellationToken = default)
        {

            await _repository.InitializeAsync(
                cancellationToken);
            _logger.LogDebug("Memory",
            "Memory Service initialized.");

        }

        public async Task StoreAsync(
            MemoryItem memory,
            CancellationToken cancellationToken = default)
        {
            _cache.Add(memory);

            await _repository.SaveAsync(
                memory,
                cancellationToken);

            _logger.LogDebug(
                "Stored memory",String.Format("Memory Title: {0}", memory.Title));
        }

        public async Task<IReadOnlyList<string>> SearchAsync(
            string query,
            int maximumResults = 5,
            CancellationToken cancellationToken = default)
        {
            var results =
                await _repository.SearchAsync(
                    new MemoryQuery
                    {
                        SearchText = query,
                        MaximumResults = maximumResults
                    },
                    cancellationToken);

            return results
                .Select(x => x.Content)
                .ToList();
        }

        public async Task<IReadOnlyList<MemorySearchResult>> SearchDetailedAsync(
            string query,
            int maximumResults = 5,
            CancellationToken cancellationToken = default)
        {
            var results =
                await _repository.SearchAsync(
                    new MemoryQuery
                    {
                        SearchText = query,
                        MaximumResults = maximumResults
                    },
                    cancellationToken);

                        return results
                            .Select(x =>
                            {
                                x.LastAccessed = DateTime.UtcNow;
                                x.AccessCount++;

                                return new MemorySearchResult
                                {
                                    Memory = x,
                                    Score = x.Importance
                                };
                            })
                            .ToList();
        }

        public async Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {

            _cache.Remove(id);

            await _repository.DeleteAsync(
                id,
                cancellationToken);

            _logger.LogDebug("Memory",String.Format("Deleted memory with ID: {0}", id));
           
        }

        public Task ClearAsync(
            CancellationToken cancellationToken = default)
        {
            lock (_cache)
            {
                _cache.Clear();
            }

            return Task.CompletedTask;
        }

        public IReadOnlyCollection<MemoryItem> GetAll()
        {
            return _cache.GetAll();
        }       
    }
}

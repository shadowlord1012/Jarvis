using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Memory
{
    public sealed class MemoryCache
    {
        private readonly ConcurrentDictionary<Guid, MemoryItem> _cache
            = new();

        public void Add(MemoryItem item)
        {
            _cache[item.Id] = item;
        }

        public bool TryGet(
            Guid id,
            out MemoryItem? item)
        {
            return _cache.TryGetValue(id, out item);
        }

        public IReadOnlyCollection<MemoryItem> GetAll()
        {
            return _cache.Values.ToList();
        }

        public void Remove(Guid id)
        {
            _cache.TryRemove(id, out _);
        }

        public void Clear()
        {
            _cache.Clear();
        }
    }
}
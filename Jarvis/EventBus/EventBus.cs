using Common.Events.Interfaces;
using System.Collections.Concurrent;

namespace EventBus
{
    public sealed class EventBus : IEventBus, IDisposable
    {
        private readonly ConcurrentDictionary<Type, List<object>> _handlers =
        new();

        private readonly object _lock = new();

        public void Subscribe<TEvent>(
            IEventHandler<TEvent> handler)
            where TEvent : IEvent
        {
            ArgumentNullException.ThrowIfNull(handler);

            lock (_lock)
            {
                var list = _handlers.GetOrAdd(
                    typeof(TEvent),
                    _ => new List<object>());

                if (!list.Contains(handler))
                    list.Add(handler);
            }
        }

        public void Unsubscribe<TEvent>(
            IEventHandler<TEvent> handler)
            where TEvent : IEvent
        {
            lock (_lock)
            {
                if (_handlers.TryGetValue(typeof(TEvent), out var list))
                {
                    list.Remove(handler);

                    if (list.Count == 0)
                        _handlers.TryRemove(typeof(TEvent), out _);
                }
            }
        }

        public async Task PublishAsync<TEvent>(
            TEvent @event,
            CancellationToken cancellationToken = default)
            where TEvent : IEvent
        {
            List<object>? handlers;

            lock (_lock)
            {
                if (!_handlers.TryGetValue(typeof(TEvent), out var registered))
                    return;

                handlers = registered.ToList();
            }

            var tasks = handlers
                .Cast<IEventHandler<TEvent>>()
                .Select(async handler =>
                {
                    try
                    {
                        await handler.HandlerAsycn(
                            @event,
                            cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });

            await Task.WhenAll(tasks);
        }

        public void Dispose()
        {
            _handlers.Clear();
        }
    }
}

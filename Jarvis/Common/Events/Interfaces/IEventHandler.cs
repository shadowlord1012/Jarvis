namespace Common.Events.Interfaces
{
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        Task HandlerAsycn(TEvent @event, CancellationToken cancellationToken = default);
    }
}

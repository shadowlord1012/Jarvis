namespace Common.Events.Interfaces
{
    public interface ICommandPlugin
    {
        bool CanHandle(string command);

        Task ExecuteAsync(string command);
    }
}

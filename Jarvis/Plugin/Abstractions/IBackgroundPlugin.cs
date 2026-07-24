namespace Common.Events.Interfaces
{
    public interface IBackgroundPlugin : IPlugin
    {
        Task StartAsync();

        Task StopAsync();
    }
}

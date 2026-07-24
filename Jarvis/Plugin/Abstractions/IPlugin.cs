using Jarvis.AI.Interfaces;

namespace Common.Events.Interfaces
{
    public interface IPlugin
    {
        string Name { get; }
        string Version { get; }
        string Author { get; }
        string Description { get; }
        Task InitializeAsync();
        Task ShutdownAsync();
        IEnumerable<ITool> GetTools();
    }
}

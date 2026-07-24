namespace Common.Events.Interfaces
{
    public interface IPluginManager
    {
        IEnumerable<IPlugin> LoadedPlugins { get; }
        Task loadPluginsAsync();

        Task<PluginResult> ExecuteAsync(string path, PluginContext context);
    }
}

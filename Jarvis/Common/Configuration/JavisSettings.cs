namespace Common.Configuration
{
    public sealed class JavisSettings
    {
        public string Name { get; init; } = "";
        public string Version { get; init; } = "";
        public string LogDirectory { get; init; } = "";
        public string PluginDirectory { get; init; } = "";
        public string MemoryDirectory { get; init; } = "";
    }
}

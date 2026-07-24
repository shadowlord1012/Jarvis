namespace Common.Configuration
{
    public sealed class TTSSettings
    {
        public string Engine { get; init; } = "";
        public string Voice { get; init; } = "";
        public string Speed { get; init; } = "";
        public string ElevenlabsAPIkey { get; init; } = "";
    }
}

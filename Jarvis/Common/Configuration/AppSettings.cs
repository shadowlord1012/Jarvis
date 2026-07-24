namespace Common.Configuration
{
    public sealed class AppSettings
    {
        public JavisSettings Jarvis { get; init; } = new();
        public LLMSettings LlmOllama { get; init; } = new();

        public STTSettings Speach { get; init; } = new();

        public TTSSettings TTS { get; init; } = new();
    }
}

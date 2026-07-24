namespace Common.Configuration
{
    public sealed class LLMSettings
    {
        public string Engine { get; init; } = "";
        public string Model { get; init; } = "";
        public string Language { get; init; } = "";
    }
}

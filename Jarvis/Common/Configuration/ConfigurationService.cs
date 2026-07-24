using Microsoft.Extensions.Options;

namespace Common.Configuration
{
    public sealed class ConfigurationService : IConfigurationService
    {
        public AppSettings Settings { get; }
        public ConfigurationService(IOptions<AppSettings> options)
        {
            Settings = options.Value;
        }
    }
}

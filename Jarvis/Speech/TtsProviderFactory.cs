using Jarvis.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Speech
{
    public sealed class TtsProviderFactory
    {
        private readonly IEnumerable<ITtsProvider> _providers;

        public TtsProviderFactory(
            IEnumerable<ITtsProvider> providers)
        {
            _providers = providers;
        }

        public ITtsProvider GetProvider(
            string providerName)
        {
            var provider =
                _providers.FirstOrDefault(
                    x => x.ProviderName.Equals(
                        providerName,
                        StringComparison.OrdinalIgnoreCase));

            if (provider is null)
            {
                throw new InvalidOperationException(
                    $"TTS provider '{providerName}' " +
                    $"is not registered.");
            }

            return provider;
        }
    }
}

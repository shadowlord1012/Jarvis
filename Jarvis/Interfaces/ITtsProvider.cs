using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Interfaces
{
    public interface ITtsProvider
    {
        string ProviderName { get; }

        Task<byte[]> SynthesizeAsync(
            string text,
            CancellationToken cancellationToken = default);
    }
}

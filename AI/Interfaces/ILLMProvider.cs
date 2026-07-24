using Jarvis.AI.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Interfaces
{
    public interface ILLMProvider
    {
        string ProviderName { get; }

        string CurrentModel { get; }

        Task InitializeAsync(
            CancellationToken cancellationToken = default);

        Task<bool> CheckHealthAsync(
            CancellationToken cancellationToken = default);

        Task<LLMResponse> GenerateAsync(
            LLMRequest request,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<LLMStreamChunk> StreamAsync(
            LLMRequest request,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<string>> GetModelsAsync(
            CancellationToken cancellationToken = default);

        Task SetModelAsync(
            string model,
            CancellationToken cancellationToken = default);
    }
}

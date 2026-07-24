using Jarvis.AI.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Interfaces
{
    public interface IToolRegistry
    {
        void Register(ITool tool);

        bool Contains(string name);

        ITool? Get(string name);

        IReadOnlyCollection<ITool> GetAll();

        Task<IReadOnlyList<ToolDefinition>> GetDefinitionsAsync(
            CancellationToken cancellationToken = default);
    }
}

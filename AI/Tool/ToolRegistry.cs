using Jarvis.AI.Interfaces;
using Jarvis.AI.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Tool
{
    public sealed class ToolRegistry : IToolRegistry
    {
        private readonly Dictionary<string, ITool> _tools =
            new(StringComparer.OrdinalIgnoreCase);

        public void Register(ITool tool)
        {
            _tools[tool.Name] = tool;
        }

        public bool Contains(string name)
        {
            return _tools.ContainsKey(name);
        }

        public ITool? Get(string name)
        {
            _tools.TryGetValue(name, out var tool);
            return tool;
        }

        public IReadOnlyCollection<ITool> GetAll()
        {
            return _tools.Values.ToList().AsReadOnly();
        }

        public Task<IReadOnlyList<ToolDefinition>> GetDefinitionsAsync(
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<ToolDefinition> definitions = _tools.Values
                .Select(t => t.Definition)
                .ToList();

            return Task.FromResult(definitions);
        }
    }
}

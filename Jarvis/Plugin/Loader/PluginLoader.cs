using Common.Events.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Loader
{
    public class PluginLoader
    {
        public List<IPlugin> Plugins { get; } = new();
        private readonly StartupStatusService _startup;

        public PluginLoader(StartupStatusService startup)
        {
            _startup = startup;
        }


        public async Task LoadPluginsAsync(string folder)
        {
            if (!Directory.Exists(folder))
            {
                _startup.Error("Unable to locate folder");
                return;
            }

            await Task.Run(() =>
            {
                foreach (var dll in Directory.GetFiles(folder, "*.dll"))
                {
                    LoadPlugin(dll);
                }
            });
        }

        public void LoadPlugins(string folder)
        {
            if (!Directory.Exists(folder))
            {
                return;
            }

            foreach (var dll in Directory.GetFiles(folder, "*.dll"))
            {
                LoadPlugin(dll);
            }
        }

        private void LoadPlugin(string path)
        {
            var loadContext = new PluginLoadContext(path);

            var assembly = loadContext.LoadFromAssemblyName(
                AssemblyName.GetAssemblyName(path));

            var pluginTypes = assembly.GetTypes()
                .Where(t =>
                    typeof(IPlugin).IsAssignableFrom(t) &&
                    !t.IsAbstract &&
                    !t.IsInterface);

            foreach (var type in pluginTypes)
            {
                if (Activator.CreateInstance(type) is IPlugin plugin)
                {
                    Plugins.Add(plugin);
                }
            }
        }
        public async Task InitializePluginsAsync()
        {
            foreach (var plugin in Plugins)
            {
                await plugin.InitializeAsync();
            }
        }
        public async Task ShutdownPluginsAsync()
        {
            foreach (var plugin in Plugins)
            {
                await plugin.ShutdownAsync();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Abstract;
using UI.Controls.HUD.Configurations;
using UI.Controls.HUD.Factories;

namespace UI.Controls.HUD.Managers
{
    public class HudPanelManager
    {
        private readonly HudPanelFactory _factory;

        private readonly Dictionary<string, HudPanel> _loadedPanels =
            new();
        private readonly HudConfigurationLoader _loader;
        public HudPanelManager(
            HudPanelFactory factory,
            HudConfigurationLoader loader)
        {
            _factory = factory;
            _loader = loader;
        }

        /// <summary>
        /// Builds all registered HUD panels.
        /// </summary>
        public async Task InitializeAsync()
        {

            var configuration =
                await _loader.LoadAsync();



            _factory.LoadDefinitions(
                configuration);



            foreach (var panel in configuration.Panels)
            {
                await CreatePanelAsync(
                    panel.Id);
            }
        }



        private async Task CreatePanelAsync(
            string id)
        {
            await Task.Delay(10);


            var panel =
                _factory.CreateRegisteredPanelByID(id);


            _loadedPanels[id] =
                panel;
        }



        public HudPanel GetPanel(
            string id)
        {
            if (_loadedPanels.TryGetValue(
                id,
                out var panel))
            {
                return panel;
            }


            return null;
        }



        private IEnumerable<string> GetRegisteredPanels()
        {
            return new[]
            {
                "ReactorCore",
                "SystemStatus",
                "Diagnostics"
            };
        }
    }
}

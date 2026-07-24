using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace UI.Controls.HUD.Configurations
{
    public class HudConfigurationLoader
    {

        private readonly string _configPath;
        public HudConfigurationLoader()
        {
            _configPath =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Hud",
                    "Configuration",
                    "hud_layout.json");
        }

        public async Task<HudLayoutConfiguration>
            LoadAsync()
        {

            if (!File.Exists(_configPath))
            {
                return new HudLayoutConfiguration();
            }


            var json =
                await File.ReadAllTextAsync(
                    _configPath);



            return JsonSerializer.Deserialize
                <HudLayoutConfiguration>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
        }
        public async Task SaveAsync(
            HudLayoutConfiguration layout)
        {

            var options =
                new JsonSerializerOptions
                {
                    WriteIndented = true
                };


            var json =
                JsonSerializer.Serialize(
                    layout,
                    options);



            await File.WriteAllTextAsync(
                _configPath,
                json);
        }

    }
}

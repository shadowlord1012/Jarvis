using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using UI.Controls.HUD.Interfaces;

namespace Jarvis.Diagnostics
{
    /// <summary>
    /// Diagnostic utility to verify configuration files and settings
    /// </summary>
    public class ConfigurationDiagnostics
    {
        private readonly ILogService _logger;

        public ConfigurationDiagnostics(ILogService logger)
        {
            _logger = logger;
        }

        public void PrintConfigurationDiagnostics(IConfiguration configuration)
        {
            _logger.LogInfo("ConfigurationDiagnostics", "==================== Configuration Diagnostics ====================");
            _logger.LogInfo("ConfigurationDiagnostics", $"Current Directory: {Directory.GetCurrentDirectory()}");
            _logger.LogInfo("ConfigurationDiagnostics", $"Base Directory: {AppContext.BaseDirectory}");

            // Check if appsettings.json exists
            var appSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            _logger.LogInfo("ConfigurationDiagnostics", $"appsettings.json exists: {File.Exists(appSettingsPath)}");
            if (File.Exists(appSettingsPath))
            {
                _logger.LogInfo("ConfigurationDiagnostics", $"  Location: {appSettingsPath}");
                var fileInfo = new FileInfo(appSettingsPath);
                _logger.LogInfo("ConfigurationDiagnostics", $"  Size: {fileInfo.Length} bytes");
                _logger.LogInfo("ConfigurationDiagnostics", $"  Last Modified: {fileInfo.LastWriteTime}");
            }

            _logger.LogInfo("ConfigurationDiagnostics", "Configuration Keys:");
            var allKeys = configuration.AsEnumerable()
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .OrderBy(x => x.Key);

            foreach (var kvp in allKeys)
            {
                // Mask sensitive values
                var value = kvp.Key.Contains("Password", StringComparison.OrdinalIgnoreCase)
                    ? "***REDACTED***"
                    : kvp.Value;
                _logger.LogInfo("ConfigurationDiagnostics", $"  {kvp.Key} = {value}");
            }

            // Check MariaDB section specifically
            _logger.LogInfo("ConfigurationDiagnostics", "MariaDB Configuration:");
            var mariaDbSection = configuration.GetSection("MariaDB");
            if (mariaDbSection.Exists())
            {
                _logger.LogSuccess("ConfigurationDiagnostics", "  ✓ MariaDB section found");
                var connectionString = mariaDbSection["ConnectionString"];
                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    _logger.LogSuccess("ConfigurationDiagnostics", "  ✓ ConnectionString is configured");
                    // Parse and show details without password
                    try
                    {
                        var builder = new MySqlConnector.MySqlConnectionStringBuilder(connectionString);
                        _logger.LogInfo("ConfigurationDiagnostics", $"    Server: {builder.Server}");
                        _logger.LogInfo("ConfigurationDiagnostics", $"    Port: {builder.Port}");
                        _logger.LogInfo("ConfigurationDiagnostics", $"    Database: {builder.Database}");
                        _logger.LogInfo("ConfigurationDiagnostics", $"    UserId: {builder.UserID}");
                        _logger.LogInfo("ConfigurationDiagnostics", $"    SslMode: {builder.SslMode}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("ConfigurationDiagnostics", $"  ✗ Invalid connection string format: {ex.Message}");
                    }
                }
                else
                {
                    _logger.LogWarning("ConfigurationDiagnostics", "  ✗ ConnectionString is empty or not found");
                }

                _logger.LogInfo("ConfigurationDiagnostics", $"  TableName: {mariaDbSection["TableName"]}");
            }
            else
            {
                _logger.LogWarning("ConfigurationDiagnostics", "  ✗ MariaDB section NOT found in configuration");
            }

            _logger.LogInfo("ConfigurationDiagnostics", "===================================================================");
        }
    }
}

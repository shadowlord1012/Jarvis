using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace Jarvis.Diagnostics
{
    /// <summary>
    /// Diagnostic utility to verify configuration files and settings
    /// </summary>
    public static class ConfigurationDiagnostics
    {
        public static void PrintConfigurationDiagnostics(IConfiguration configuration)
        {
            Console.WriteLine("==================== Configuration Diagnostics ====================");
            Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");
            Console.WriteLine($"Base Directory: {AppContext.BaseDirectory}");

            // Check if appsettings.json exists
            var appSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            Console.WriteLine($"appsettings.json exists: {File.Exists(appSettingsPath)}");
            if (File.Exists(appSettingsPath))
            {
                Console.WriteLine($"  Location: {appSettingsPath}");
                var fileInfo = new FileInfo(appSettingsPath);
                Console.WriteLine($"  Size: {fileInfo.Length} bytes");
                Console.WriteLine($"  Last Modified: {fileInfo.LastWriteTime}");
            }

            Console.WriteLine("\nConfiguration Keys:");
            var allKeys = configuration.AsEnumerable()
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .OrderBy(x => x.Key);

            foreach (var kvp in allKeys)
            {
                // Mask sensitive values
                var value = kvp.Key.Contains("Password", StringComparison.OrdinalIgnoreCase)
                    ? "***REDACTED***"
                    : kvp.Value;
                Console.WriteLine($"  {kvp.Key} = {value}");
            }

            // Check MariaDB section specifically
            Console.WriteLine("\nMariaDB Configuration:");
            var mariaDbSection = configuration.GetSection("MariaDB");
            if (mariaDbSection.Exists())
            {
                Console.WriteLine("  ✓ MariaDB section found");
                var connectionString = mariaDbSection["ConnectionString"];
                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    Console.WriteLine("  ✓ ConnectionString is configured");
                    // Parse and show details without password
                    try
                    {
                        var builder = new MySqlConnector.MySqlConnectionStringBuilder(connectionString);
                        Console.WriteLine($"    Server: {builder.Server}");
                        Console.WriteLine($"    Port: {builder.Port}");
                        Console.WriteLine($"    Database: {builder.Database}");
                        Console.WriteLine($"    UserId: {builder.UserID}");
                        Console.WriteLine($"    SslMode: {builder.SslMode}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  ✗ Invalid connection string format: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("  ✗ ConnectionString is empty or not found");
                }

                Console.WriteLine($"  TableName: {mariaDbSection["TableName"]}");
            }
            else
            {
                Console.WriteLine("  ✗ MariaDB section NOT found in configuration");
            }

            Console.WriteLine("===================================================================\n");
        }
    }
}

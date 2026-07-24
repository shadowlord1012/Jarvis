using Jarvis.AI.Interfaces;
using Jarvis.AI.Storage.MariaDB;
using Microsoft.Extensions.Options;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UI.Controls.HUD.Interfaces;

namespace Jarvis.AI.Factories
{
    public sealed class DatabaseConnectionFactory
    : IDatabaseConnectionFactory
    {
        private readonly MariaDbOptions _options;
        private readonly ILogService _logger;

        public DatabaseConnectionFactory(
            IOptions<MariaDbOptions> options,
            ILogService logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task<IDbConnection> CreateConnectionAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var connectionString = _options.GetConnectionStringWithPooling();
                _logger.LogDebug("DatabaseConnection", "Attempting to connect to database...");
                _logger.LogDebug("DatabaseConnection", $"Connection string (sanitized): Server={GetServerFromConnectionString(connectionString)}, Database={GetDatabaseFromConnectionString(connectionString)}");

                var connection = new MySqlConnection(connectionString);

                await connection.OpenAsync(cancellationToken);

                _logger.LogSuccess("DatabaseConnection", "Database connection successfully established.");

                return connection;
            }
            catch (MySqlException ex)
            {
                _logger.LogError("DatabaseConnection", $"MySQL Error connecting to database: [{ex.Number}] {ex.Message}");
                _logger.LogError("DatabaseConnection", $"Error Details - SqlState: {ex.SqlState}, Server: {GetServerFromConnectionString(_options.GetConnectionStringWithPooling())}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("DatabaseConnection", $"Unexpected error connecting to database: {ex.Message}");
                _logger.LogError("DatabaseConnection", $"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private string GetServerFromConnectionString(string connectionString)
        {
            try
            {
                var builder = new MySqlConnectionStringBuilder(connectionString);
                return $"{builder.Server}:{builder.Port}";
            }
            catch
            {
                return "unknown";
            }
        }

        private string GetDatabaseFromConnectionString(string connectionString)
        {
            try
            {
                var builder = new MySqlConnectionStringBuilder(connectionString);
                return builder.Database;
            }
            catch
            {
                return "unknown";
            }
        }
    }
}

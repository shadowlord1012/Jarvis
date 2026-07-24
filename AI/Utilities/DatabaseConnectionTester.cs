using MySqlConnector;
using System;
using System.Threading.Tasks;

namespace Jarvis.AI.Utilities
{
    /// <summary>
    /// Utility class to test MariaDB/MySQL database connections
    /// </summary>
    public static class DatabaseConnectionTester
    {
        /// <summary>
        /// Tests a database connection and returns detailed diagnostic information
        /// </summary>
        public static async Task<ConnectionTestResult> TestConnectionAsync(string connectionString)
        {
            var result = new ConnectionTestResult
            {
                ConnectionString = SanitizeConnectionString(connectionString),
                TestTimestamp = DateTime.UtcNow
            };

            try
            {
                var builder = new MySqlConnectionStringBuilder(connectionString);
                result.Server = builder.Server;
                result.Port = builder.Port;
                result.Database = builder.Database;
                result.UserId = builder.UserID;

                using var connection = new MySqlConnection(connectionString);

                var startTime = DateTime.UtcNow;
                await connection.OpenAsync();
                var endTime = DateTime.UtcNow;

                result.ConnectionTimeMs = (endTime - startTime).TotalMilliseconds;
                result.IsSuccess = true;
                result.ServerVersion = connection.ServerVersion;
                result.Message = "Connection successful";

                // Test a simple query
                using var command = connection.CreateCommand();
                command.CommandText = "SELECT VERSION()";
                var version = await command.ExecuteScalarAsync();
                result.DatabaseVersion = version?.ToString() ?? "Unknown";

                await connection.CloseAsync();
            }
            catch (MySqlException ex)
            {
                result.IsSuccess = false;
                result.ErrorCode = ex.Number;
                result.SqlState = ex.SqlState;
                result.Message = ex.Message;
                result.ErrorDetails = GetMySqlErrorDetails(ex.Number);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.ErrorDetails = ex.GetType().Name;
            }

            return result;
        }

        private static string SanitizeConnectionString(string connectionString)
        {
            try
            {
                var builder = new MySqlConnectionStringBuilder(connectionString);
                builder.Password = "***REDACTED***";
                return builder.ConnectionString;
            }
            catch
            {
                return "Invalid connection string";
            }
        }

        private static string GetMySqlErrorDetails(int errorCode)
        {
            return errorCode switch
            {
                1042 => "Unable to connect to any of the specified MySQL hosts (Network/firewall issue)",
                1043 => "Bad handshake (Authentication protocol issue)",
                1044 => "Access denied for user to database",
                1045 => "Access denied for user (Invalid username/password)",
                1049 => "Unknown database",
                1130 => "Host is not allowed to connect to this MySQL server",
                2002 => "Can't connect to MySQL server (Connection refused)",
                2003 => "Can't connect to MySQL server (Connection timeout/network issue)",
                2006 => "MySQL server has gone away",
                2013 => "Lost connection to MySQL server during query",
                _ => $"MySQL Error {errorCode}"
            };
        }
    }

    public class ConnectionTestResult
    {
        public bool IsSuccess { get; set; }
        public string Server { get; set; } = string.Empty;
        public uint Port { get; set; }
        public string Database { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ErrorDetails { get; set; }
        public int? ErrorCode { get; set; }
        public string? SqlState { get; set; }
        public string ConnectionString { get; set; } = string.Empty;
        public DateTime TestTimestamp { get; set; }
        public double ConnectionTimeMs { get; set; }
        public string? ServerVersion { get; set; }
        public string? DatabaseVersion { get; set; }

        public override string ToString()
        {
            if (IsSuccess)
            {
                return $"✓ SUCCESS: Connected to {Server}:{Port}/{Database} in {ConnectionTimeMs:F2}ms\n" +
                       $"  Server Version: {ServerVersion}\n" +
                       $"  Database Version: {DatabaseVersion}";
            }
            else
            {
                return $"✗ FAILED: {Message}\n" +
                       $"  Server: {Server}:{Port}\n" +
                       $"  Database: {Database}\n" +
                       $"  User: {UserId}\n" +
                       (ErrorCode.HasValue ? $"  Error Code: {ErrorCode} ({ErrorDetails})\n" : "") +
                       (SqlState != null ? $"  SQL State: {SqlState}\n" : "") +
                       $"  Connection String: {ConnectionString}";
            }
        }
    }
}

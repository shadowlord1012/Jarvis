using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Storage.MariaDB
{
    public sealed class MariaDbOptions
    {
        public string ConnectionString { get; set; } = string.Empty;

        public string TableName { get; set; } = "memory";

        /// <summary>
        /// Minimum number of connections in the pool.
        /// </summary>
        public int MinPoolSize { get; set; } = 0;

        /// <summary>
        /// Maximum number of connections in the pool.
        /// </summary>
        public int MaxPoolSize { get; set; } = 100;

        /// <summary>
        /// Whether to enable connection pooling.
        /// </summary>
        public bool Pooling { get; set; } = true;

        /// <summary>
        /// Validates that the configuration has been loaded correctly.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                throw new InvalidOperationException(
                    "MariaDB ConnectionString is not configured. Please check appsettings.json");
            }

            if (string.IsNullOrWhiteSpace(TableName))
            {
                throw new InvalidOperationException(
                    "MariaDB TableName is not configured. Please check appsettings.json");
            }
        }

        /// <summary>
        /// Gets the connection string with pooling parameters applied.
        /// </summary>
        public string GetConnectionStringWithPooling()
        {
            Validate();

            var builder = new MySqlConnector.MySqlConnectionStringBuilder(ConnectionString)
            {
                Pooling = Pooling,
                MinimumPoolSize = (uint)MinPoolSize,
                MaximumPoolSize = (uint)MaxPoolSize,
                ConnectionLifeTime = 300 // 5 minutes
            };
            return builder.ConnectionString;
        }
    }
}

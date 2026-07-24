using Jarvis.AI.Interfaces;
using Jarvis.AI.Storage.MariaDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Providers
{
    public sealed class MemorySqlProvider : IMemorySqlProvider
    {
        private readonly string _table;

        public MemorySqlProvider(MariaDbOptions options)
        {
            _table = options.TableName;
        }

        public string Insert =>
                $"""
            INSERT INTO {_table}
            (
                Id,
                Type,
                Title,
                Content,
                Created,
                LastAccessed,
                AccessCount,
                Importance
            )
            VALUES
            (
                @Id,
                @Type,
                @Title,
                @Content,
                @Created,
                @LastAccessed,
                @AccessCount,
                @Importance
            );
            """;

        public string Update =>
                $"""
            UPDATE {_table}
            SET
                Type = @Type,
                Title = @Title,
                Content = @Content,
                LastAccessed = @LastAccessed,
                AccessCount = @AccessCount,
                Importance = @Importance
            WHERE
                Id = @Id;
            """;

        public string Delete =>
                $"""
            DELETE
            FROM {_table}
            WHERE Id = @Id;
            """;

        public string GetById =>
                $"""
            SELECT *
            FROM {_table}
            WHERE Id = @Id;
            """;

        public string Search =>
                $"""
            SELECT *
            FROM {_table}
            WHERE
            (
                Title LIKE @Search
                OR
                Content LIKE @Search
            )
            ORDER BY
                Importance DESC,
                LastAccessed DESC
            LIMIT @Limit;
            """;

        public string GetRecent =>
                $"""
            SELECT *
            FROM {_table}
            ORDER BY Created DESC
            LIMIT @Count;
            """;

        public string Count =>
            $"""
        SELECT COUNT(*)
        FROM {_table};
        """;

        public string Statistics =>
                $"""
            SELECT
            COUNT(*) AS TotalMemories,
            SUM(Type = 0) AS ConversationMemories,
            SUM(Type = 1) AS FactMemories,
            SUM(Type = 2) AS PreferenceMemories,
            SUM(Type = 3) AS TaskMemories,
            SUM(Type = 4) AS SystemMemories,
            SUM(Type = 5) AS PluginMemories,
            SUM(Type = 6) AS TemporaryMemories,
            AVG(Importance) AS AverageImportance,
            AVG(AccessCount) AS AverageAccessCount,
            MIN(Created) AS OldestMemory,
            MAX(Created) AS NewestMemory,
            MAX(LastAccessed) AS LastAccessed
            FROM {_table};
            """;

        public string Clear =>
            $"""
        TRUNCATE TABLE {_table};
        """;

        public string CreateTable =>
                $"""
            CREATE TABLE IF NOT EXISTS {_table}
            (
                Id CHAR(36) PRIMARY KEY,

                Type INT NOT NULL,

                Title VARCHAR(255),

                Content LONGTEXT,

                Created DATETIME,

                LastAccessed DATETIME,

                AccessCount INT,

                Importance DOUBLE
            );
            """;

        public string CreateIndexes =>
            $"""
        CREATE INDEX IF NOT EXISTS IDX_Memory_Type
        ON {_table}(Type);

        CREATE INDEX IF NOT EXISTS IDX_Memory_Created
        ON {_table}(Created);

        CREATE INDEX IF NOT EXISTS IDX_Memory_LastAccessed
        ON {_table}(LastAccessed);

        CREATE INDEX IF NOT EXISTS IDX_Memory_Importance
        ON {_table}(Importance);
        """;
    }
}


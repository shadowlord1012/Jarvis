using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Jarvis.AI.Interfaces
{
    public interface IDatabaseConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync(
            CancellationToken cancellationToken = default);
    }
}

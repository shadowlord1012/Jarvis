using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Interfaces
{
    public interface IMemorySqlProvider
    {
        string Insert { get; }

        string Update { get; }

        string Delete { get; }

        string GetById { get; }

        string Search { get; }

        string GetRecent { get; }

        string Count { get; }

        string Statistics { get; }

        string Clear { get; }

        string CreateTable { get; }

        string CreateIndexes { get; }
    }
}

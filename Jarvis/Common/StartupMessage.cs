using System;
using System.Collections.Generic;
using System.Text;

namespace Loader
{
    public enum StartupStatus
    {
        Information,
        Success,
        Warning,
        Error
    }

    public class StartupMessage
    {
        public DateTime Time { get; init; }

        public StartupStatus Status { get; init; }

        public string Message { get; init; } = string.Empty;
    }
}

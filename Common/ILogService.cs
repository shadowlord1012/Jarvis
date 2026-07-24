using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Interfaces
{
    public interface ILogService
    {
        event EventHandler<LogEntry>? EntryAdded;

        IReadOnlyList<LogEntry> Entries { get; }

        void LogInfo(string source, string message);

        void LogSuccess(string source, string message);

        void LogWarning(string source, string message);

        void LogError(string source, string message);

        void LogDebug(string source, string message);

        void LogSummary(string source, string summary);
    }
}

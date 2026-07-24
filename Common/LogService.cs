using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Interfaces;

namespace UI.Controls.HUD.Models
{
    /// <summary>
    /// Central logging service for Jarvis.
    /// </summary>
    public sealed class LogService : ILogService
    {
        private readonly ConcurrentQueue<LogEntry> _entries = new();

        private readonly object _lock = new();

        private readonly string _logDirectory;

        private readonly string _logFile;

        private const int MaxEntries = 500;

        public event EventHandler<LogEntry>? EntryAdded;

        public IReadOnlyList<LogEntry> Entries
        {
            get
            {
                return _entries.ToList().AsReadOnly();
            }
        }

        public LogService()
        {
            _logDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Logs");

            Directory.CreateDirectory(_logDirectory);

            _logFile = Path.Combine(
                _logDirectory,
                $"Jarvis_{DateTime.Now:yyyy-MM-dd}.log");
        }

        #region Public Logging

        public void LogInfo(
            string source,
            string message)
        {
            Add(LogSeverity.Info, source, message);
        }

        public void LogSuccess(
            string source,
            string message)
        {
            Add(LogSeverity.Success, source, message);
        }

        public void LogWarning(
            string source,
            string message)
        {
            Add(LogSeverity.Warning, source, message);
        }

        public void LogError(
            string source,
            string message)
        {
            Add(LogSeverity.Error, source, message);
        }

        public void LogDebug(
            string source,
            string message)
        {
            Add(LogSeverity.Debug, source, message);
        }

        public void LogSummary(
            string source,
            string message)
        {
            Add(LogSeverity.Summary, source, message);
        }

        public void LogAI(
            string source,
            string message)
        {
            Add(LogSeverity.AI, source, message);
        }

        #endregion

        #region Internal

        private void Add(
            LogSeverity severity,
            string source,
            string message)
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(
            message))
            {
                return;
            }

            // Create a new log entry
            LogEntry entry = new()
            {
                Timestamp = DateTime.Now,

                Severity = severity,

                Source = source,

                Message = message
            };

                _entries.Enqueue(entry);


            // Ensure the queue does not exceed the maximum number of entries
            while (_entries.Count > MaxEntries)
            {
                _entries.TryDequeue(out _);
            }

                WriteToDisk(entry);

            // Raise the EntryAdded event
            EntryAdded?.Invoke(
                this,
                entry);
        }

        private void WriteToDisk(
            LogEntry entry)
        {
            lock (_lock)
            {
                File.AppendAllText(
                    _logFile,
                    Format(entry) + Environment.NewLine,
                    Encoding.UTF8);
            }
        }

        private static string Format(
            LogEntry entry)
        {
            return
                $"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] " +
                $"[{entry.Severity,-7}] " +
                $"[{entry.Source}] " +
                $"{entry.Message}";
        }

        #endregion
    }
}

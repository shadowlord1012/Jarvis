using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Enums;

namespace UI.Controls.HUD.Models
{
    public sealed class LogEntry
    {
        public DateTime Timestamp { get; init; } = DateTime.Now;

        public LogSeverity Severity { get; init; }

        public string Source { get; init; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Optional AI-generated summary.
        /// </summary>
        public string? Summary { get; init; }
    }
}

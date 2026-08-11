using System;

namespace Jarvis.AI.Tool.Models
{
    /// <summary>
    /// Parameters for file processing operations.
    /// </summary>
    public sealed class FileProcessingParameters
    {
        /// <summary>
        /// The file operation to perform.
        /// Valid operations: read, write, list, exists, delete, search, copy, move
        /// </summary>
        public string Operation { get; set; } = string.Empty;

        /// <summary>
        /// The file or directory path for the operation.
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Content to write to the file (used for 'write' operation).
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// Destination path for copy or move operations.
        /// </summary>
        public string? DestinationPath { get; set; }

        /// <summary>
        /// Search pattern for search or list operations (e.g., "*.txt").
        /// </summary>
        public string? SearchPattern { get; set; }

        /// <summary>
        /// Whether to search recursively in subdirectories.
        /// </summary>
        public bool Recursive { get; set; } = false;
    }
}

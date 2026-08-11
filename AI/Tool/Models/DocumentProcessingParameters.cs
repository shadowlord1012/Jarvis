using System;
using System.Text.Json.Serialization;

namespace Jarvis.AI.Tool.Models
{
    /// <summary>
    /// Parameters for document processing operations.
    /// </summary>
    public sealed class DocumentProcessingParameters
    {
        /// <summary>
        /// The document processing operation to perform.
        /// Valid operations: summarize, translate
        /// </summary>
        [JsonPropertyName("operation")]
        public string Operation { get; set; } = string.Empty;

        /// <summary>
        /// The file path or content to process.
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Target language for translation (e.g., "Spanish", "French", "German").
        /// Only used for translate operation.
        /// </summary>
        [JsonPropertyName("targetLanguage")]
        public string? TargetLanguage { get; set; }

        /// <summary>
        /// Summary length preference (e.g., "brief", "detailed", "bullet_points").
        /// Only used for summarize operation.
        /// </summary>
        [JsonPropertyName("summaryType")]
        public string? SummaryType { get; set; } = "detailed";

        /// <summary>
        /// Optional file path to read content from.
        /// If provided, will read from file instead of using Content property.
        /// </summary>
        [JsonPropertyName("filePath")]
        public string? FilePath { get; set; }

        /// <summary>
        /// Optional output file path to save the result.
        /// If not provided, will auto-generate a filename in Documents/Jarvis/Summaries.
        /// </summary>
        [JsonPropertyName("outputFilePath")]
        public string? OutputFilePath { get; set; }
    }
}

using System;

namespace Jarvis.AI.Services
{
    /// <summary>
    /// Service to hold uploaded document context for AI conversations.
    /// </summary>
    public sealed class DocumentContextService
    {
        private string? _uploadedDocumentPath;
        private string? _uploadedDocumentFileName;
        private DateTime? _uploadedAt;

        /// <summary>
        /// Sets the uploaded document context.
        /// </summary>
        public void SetUploadedDocument(string filePath, string fileName)
        {
            _uploadedDocumentPath = filePath;
            _uploadedDocumentFileName = fileName;
            _uploadedAt = DateTime.Now;
        }

        /// <summary>
        /// Clears the uploaded document context.
        /// </summary>
        public void ClearUploadedDocument()
        {
            _uploadedDocumentPath = null;
            _uploadedDocumentFileName = null;
            _uploadedAt = null;
        }

        /// <summary>
        /// Gets the uploaded document context as a formatted string for the AI system prompt.
        /// </summary>
        public string? GetUploadedDocumentContext()
        {
            if (string.IsNullOrEmpty(_uploadedDocumentPath))
            {
                return null;
            }

            return $"File: '{_uploadedDocumentFileName}' at path: {_uploadedDocumentPath} (uploaded {_uploadedAt:HH:mm:ss})";
        }

        /// <summary>
        /// Checks if a document is currently uploaded.
        /// </summary>
        public bool HasUploadedDocument => !string.IsNullOrEmpty(_uploadedDocumentPath);
    }
}

using Jarvis.AI.Interfaces;
using Jarvis.AI.Providers;
using Jarvis.AI.Tool.Models;
using Jarvis.AI.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;


namespace Jarvis.AI.Tool.DocumentProcessing
{
    /// <summary>
    /// Tool for processing documents using LLM capabilities.
    /// Supports: summarize, translate
    /// </summary>
    public sealed class DocumentProcessingTool : ITool
    {
        private readonly ILLMProvider _llmProvider;
        private const int MaxContentLength = 50000; // ~50k characters limit

        public DocumentProcessingTool(ILLMProvider llmProvider)
        {
            _llmProvider = llmProvider ?? throw new ArgumentNullException(nameof(llmProvider));
        }

        public string Name => "document_processing";

        public string Description =>
            "Processes documents to summarize content or translate to different languages. " +
            "Requires either 'content' (text to process) or 'filePath' (file to read). " +
            "IMPORTANT: You must provide the actual text content in the 'content' parameter.";

        public ToolDefinition Definition => new()
        {
            Name = Name,
            Description = Description,
            JsonSchema =
            """
            {
                "type": "object",
                "properties": {
                    "operation": {
                        "type": "string",
                        "description": "The document processing operation to perform.",
                        "enum": ["summarize", "translate"]
                    },
                    "content": {
                        "type": "string",
                        "description": "The document content to process. REQUIRED: Must contain the actual text you want to summarize or translate."
                    },
                    "filePath": {
                        "type": "string",
                        "description": "Optional file path to read content from."
                    },
                    "outputFilePath": {
                        "type": "string",
                        "description": "Optional output file path to save the result. If not provided, will auto-generate a filename."
                    },
                    "targetLanguage": {
                        "type": "string",
                        "description": "Target language for translation (e.g., 'Spanish', 'French', 'German')."
                    },
                    "summaryType": {
                        "type": "string",
                        "description": "Summary style: 'brief', 'detailed', or 'bullet_points'.",
                        "enum": ["brief", "detailed", "bullet_points"]
                    }
                },
                "required": ["operation"]
            }
            """
        };

        public async Task<ToolExecutionResult> ExecuteAsync(
            ToolExecutionRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                Console.WriteLine($"[DocumentProcessingTool] Parsing arguments: {request.Arguments}");

                var parameters = ParseArguments(request.Arguments);

                if (parameters == null)
                {
                    return new ToolExecutionResult
                    {
                        Success = false,
                        Error = "Invalid document processing parameters."
                    };
                }

                Console.WriteLine($"[DocumentProcessingTool] Parsed - Operation: {parameters.Operation}, Content length: {parameters.Content?.Length ?? 0}, FilePath: {parameters.FilePath ?? "null"}");

                // Load content from file if filePath is provided
                string content = parameters.Content;
                if (!string.IsNullOrEmpty(parameters.FilePath))
                {
                    if (!File.Exists(parameters.FilePath))
                    {
                        return new ToolExecutionResult
                        {
                            Success = false,
                            Error = $"File not found: {parameters.FilePath}"
                        };
                    }

                    content = await File.ReadAllTextAsync(parameters.FilePath, cancellationToken);
                }

                Console.WriteLine($"[DocumentProcessingTool] Final content length: {content?.Length ?? 0}");

                if (string.IsNullOrWhiteSpace(content))
                {
                    return new ToolExecutionResult
                    {
                        Success = false,
                        Error = "Content is required for document processing. Please provide the text to process in the 'content' parameter, or specify a 'filePath' to read from a file."
                    };
                }

                if (content.Length > MaxContentLength)
                {
                    return new ToolExecutionResult
                    {
                        Success = false,
                        Error = $"Content too large. Maximum length is {MaxContentLength} characters."
                    };
                }

                return parameters.Operation.ToLowerInvariant() switch
                {
                    "summarize" => await SummarizeAsync(content, parameters.SummaryType ?? "brief", parameters.OutputFilePath, cancellationToken),
                    "translate" => await TranslateAsync(content, parameters.TargetLanguage, cancellationToken),
                    _ => new ToolExecutionResult
                    {
                        Success = false,
                        Error = $"Unknown operation: {parameters.Operation}"
                    }
                };
            }
            catch (Exception ex)
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = $"Document processing error: {ex.Message}"
                };
            }
        }

        private DocumentProcessingParameters? ParseArguments(string arguments)
        {
            try
            {
                return JsonSerializer.Deserialize<DocumentProcessingParameters>(arguments);
            }
            catch
            {
                return null;
            }
        }

        private async Task<ToolExecutionResult> SummarizeAsync(
            string content,
            string summaryType,
            string? outputFilePath,
            CancellationToken cancellationToken)
        {
            var promptBuilder = new StringBuilder();
            promptBuilder.Append("Please provide a ");

            switch (summaryType.ToLowerInvariant())
            {
                case "brief":
                    promptBuilder.Append("brief summary (2-3 sentences) ");
                    break;
                case "detailed":
                    promptBuilder.Append("detailed summary ");
                    break;
                case "bullet_points":
                    promptBuilder.Append("summary in bullet point format ");
                    break;
                default:
                    promptBuilder.Append("summary ");
                    break;
            }

            promptBuilder.AppendLine("of the following document:");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("--- DOCUMENT START ---");
            promptBuilder.AppendLine(content);
            promptBuilder.AppendLine("--- DOCUMENT END ---");

            var request = new LLMRequest
            {
                Messages = new List<LLMMessage>
                {
                    new LLMMessage
                    {
                        Role = LLMRole.User,
                        Content = promptBuilder.ToString()
                    }
                },
                Tools = new List<ToolDefinition>() // CRITICAL: Empty tools list to prevent recursive tool calling
            };

            var response = await _llmProvider.GenerateAsync(request, cancellationToken);

            if (string.IsNullOrWhiteSpace(response.Content))
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = "Failed to generate summary."
                };
            }

            // Save to file if requested
            string savedFilePath = null;
            if (outputFilePath != null || summaryType != null)
            {
                try
                {
                    // Generate filename if not provided
                    if (string.IsNullOrWhiteSpace(outputFilePath))
                    {
                        var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        var jarvisFolder = Path.Combine(documentsFolder, "Jarvis", "Summaries");
                        Directory.CreateDirectory(jarvisFolder);

                        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        outputFilePath = Path.Combine(jarvisFolder, $"Summary_{timestamp}.docx");
                    }

                    // Ensure .docx extension
                    if (!outputFilePath.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
                    {
                        outputFilePath += ".docx";
                    }

                    // Create the Word document
                    SaveToWordDocument(response.Content, outputFilePath);
                    savedFilePath = outputFilePath;

                    Console.WriteLine($"[DocumentProcessingTool] Summary saved to: {savedFilePath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DocumentProcessingTool] Failed to save file: {ex.Message}");
                    // Don't fail the tool execution, just log the error
                }
            }

            var resultMessage = savedFilePath != null
                ? $"{response.Content}\n\n[Summary saved to: {savedFilePath}]"
                : response.Content;

            return new ToolExecutionResult
            {
                Success = true,
                Output = resultMessage
            };
        }

        private async Task<ToolExecutionResult> TranslateAsync(
            string content,
            string? targetLanguage,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(targetLanguage))
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = "Target language is required for translation."
                };
            }

            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine($"Please translate the following document to {targetLanguage}:");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("--- DOCUMENT START ---");
            promptBuilder.AppendLine(content);
            promptBuilder.AppendLine("--- DOCUMENT END ---");
            promptBuilder.AppendLine();
            promptBuilder.AppendLine("Provide only the translation, without any explanations or additional text.");

            var request = new LLMRequest
            {
                Messages = new List<LLMMessage>
                {
                    new LLMMessage
                    {
                        Role = LLMRole.User,
                        Content = promptBuilder.ToString()
                    }
                },
                Tools = new List<ToolDefinition>() // CRITICAL: Empty tools list to prevent recursive tool calling
            };

            var response = await _llmProvider.GenerateAsync(request, cancellationToken);

            if (string.IsNullOrWhiteSpace(response.Content))
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = "Failed to generate translation."
                };
            }

            return new ToolExecutionResult
            {
                Success = true,
                Output = response.Content
            };
        }

        private static void SaveToWordDocument(string content, string filePath)
        {
            // Create the Word document
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                // Add a main document part
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                // Create the document structure
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                // Split content into paragraphs and add them
                var paragraphs = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                foreach (var paragraphText in paragraphs)
                {
                    if (string.IsNullOrWhiteSpace(paragraphText))
                    {
                        // Add empty paragraph for spacing
                        body.AppendChild(new Paragraph(new Run(new Text(""))));
                    }
                    else
                    {
                        Paragraph para = body.AppendChild(new Paragraph());
                        Run run = para.AppendChild(new Run());
                        run.AppendChild(new Text(paragraphText));
                    }
                }

                // Save the document
                mainPart.Document.Save();
            }
        }
    }
}

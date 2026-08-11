using Jarvis.AI.Interfaces;
using Jarvis.AI.Providers;
using Jarvis.AI.Tool.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Jarvis.AI.Tool.FileProcessing
{
    /// <summary>
    /// Tool for performing file system operations.
    /// Supports: read, write, list, exists, delete, search, copy, move
    /// </summary>
    public sealed class FileProcessingTool : ITool
    {
        private readonly string _basePath;
        private const int MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB limit

        public FileProcessingTool(string? basePath = null)
        {
            // Default to user's documents folder if no base path specified
            _basePath = basePath ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public string Name => "file_processing";

        public string Description =>
            "Performs file system operations including read, write, list, exists, delete, search, copy, and move. " +
            "All paths are relative to the user's documents folder for security.";

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
                        "description": "The file operation to perform.",
                        "enum": ["read", "write", "list", "exists", "delete", "search", "copy", "move"]
                    },
                    "path": {
                        "type": "string",
                        "description": "The file or directory path (relative to documents folder)."
                    },
                    "content": {
                        "type": "string",
                        "description": "Content to write (only for write operation)."
                    },
                    "destinationPath": {
                        "type": "string",
                        "description": "Destination path for copy or move operations."
                    },
                    "searchPattern": {
                        "type": "string",
                        "description": "Search pattern (e.g., '*.txt', '*.pdf')."
                    },
                    "recursive": {
                        "type": "boolean",
                        "description": "Search recursively in subdirectories."
                    }
                },
                "required": ["operation", "path"]
            }
            """
        };

        public async Task<ToolExecutionResult> ExecuteAsync(
            ToolExecutionRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var parameters = ParseArguments(request.Arguments);

                if (parameters == null)
                {
                    return new ToolExecutionResult
                    {
                        Success = false,
                        Error = "Invalid file processing parameters."
                    };
                }

                // Validate and resolve paths
                var resolvedPath = ResolvePath(parameters.Path);
                if (resolvedPath == null)
                {
                    return new ToolExecutionResult
                    {
                        Success = false,
                        Error = $"Invalid or unsafe path: {parameters.Path}"
                    };
                }

                return parameters.Operation.ToLowerInvariant() switch
                {
                    "read" => await ReadFileAsync(resolvedPath, cancellationToken),
                    "write" => await WriteFileAsync(resolvedPath, parameters.Content, cancellationToken),
                    "list" => await ListDirectoryAsync(resolvedPath, parameters.SearchPattern),
                    "exists" => CheckPathExists(resolvedPath),
                    "delete" => DeleteFile(resolvedPath),
                    "search" => await SearchFilesAsync(resolvedPath, parameters.SearchPattern ?? "*.*", parameters.Recursive),
                    "copy" => await CopyFileAsync(resolvedPath, ResolvePath(parameters.DestinationPath), cancellationToken),
                    "move" => MoveFile(resolvedPath, ResolvePath(parameters.DestinationPath)),
                    _ => new ToolExecutionResult
                    {
                        Success = false,
                        Error = $"Unknown operation: {parameters.Operation}"
                    }
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = $"Access denied: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = $"File processing error: {ex.Message}"
                };
            }
        }

        private FileProcessingParameters? ParseArguments(string arguments)
        {
            try
            {
                return JsonSerializer.Deserialize<FileProcessingParameters>(arguments);
            }
            catch
            {
                return null;
            }
        }

        private string? ResolvePath(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            try
            {
                // Combine with base path
                var fullPath = Path.GetFullPath(Path.Combine(_basePath, relativePath));

                // Ensure the path is within the base path (security check)
                if (!fullPath.StartsWith(_basePath, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                return fullPath;
            }
            catch
            {
                return null;
            }
        }

        private async Task<ToolExecutionResult> ReadFileAsync(string path, CancellationToken cancellationToken)
        {
            if (!File.Exists(path))
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = $"File not found: {path}"
                };
            }

            var fileInfo = new FileInfo(path);
            if (fileInfo.Length > MaxFileSizeBytes)
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = $"File too large. Maximum size is {MaxFileSizeBytes / (1024 * 1024)}MB."
                };
            }

            var content = await File.ReadAllTextAsync(path, cancellationToken);
            return new ToolExecutionResult
            {
                Success = true,
                Output = content
            };
        }

        private async Task<ToolExecutionResult> WriteFileAsync(string path, string? content, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(content))
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = "Content is required for write operation."
                };
            }

            // Create directory if it doesn't exist
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(path, content, cancellationToken);
            return new ToolExecutionResult
            {
                Success = true,
                Output = $"Successfully wrote {content.Length} characters to {Path.GetFileName(path)}"
            };
        }

        private Task<ToolExecutionResult> ListDirectoryAsync(string path, string? pattern)
        {
            if (!Directory.Exists(path))
            {
                return Task.FromResult(new ToolExecutionResult
                {
                    Success = false,
                    Error = $"Directory not found: {path}"
                });
            }

            var searchPattern = pattern ?? "*";
            var files = Directory.GetFiles(path, searchPattern);
            var directories = Directory.GetDirectories(path);

            var sb = new StringBuilder();
            sb.AppendLine($"Directory: {path}");
            sb.AppendLine();
            sb.AppendLine($"Directories ({directories.Length}):");
            foreach (var dir in directories)
            {
                sb.AppendLine($"  [DIR] {Path.GetFileName(dir)}");
            }

            sb.AppendLine();
            sb.AppendLine($"Files ({files.Length}):");
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                sb.AppendLine($"  {Path.GetFileName(file)} ({FormatFileSize(fileInfo.Length)})");
            }

            return Task.FromResult(new ToolExecutionResult
            {
                Success = true,
                Output = sb.ToString()
            });
        }

        private ToolExecutionResult CheckPathExists(string path)
        {
            var exists = File.Exists(path) || Directory.Exists(path);
            var type = File.Exists(path) ? "file" : Directory.Exists(path) ? "directory" : "none";

            return new ToolExecutionResult
            {
                Success = true,
                Output = exists ? $"Yes, {type} exists" : "No, path does not exist"
            };
        }

        private ToolExecutionResult DeleteFile(string path)
        {
            if (!File.Exists(path))
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = $"File not found: {path}"
                };
            }

            File.Delete(path);
            return new ToolExecutionResult
            {
                Success = true,
                Output = $"Successfully deleted {Path.GetFileName(path)}"
            };
        }

        private async Task<ToolExecutionResult> SearchFilesAsync(string path, string pattern, bool recursive)
        {
            if (!Directory.Exists(path))
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = $"Directory not found: {path}"
                };
            }

            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(path, pattern, searchOption);

            if (files.Length == 0)
            {
                return new ToolExecutionResult
                {
                    Success = true,
                    Output = $"No files found matching pattern '{pattern}'"
                };
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Found {files.Length} file(s) matching '{pattern}':");
            sb.AppendLine();

            foreach (var file in files.Take(100)) // Limit to 100 results
            {
                var relativePath = Path.GetRelativePath(_basePath, file);
                var fileInfo = new FileInfo(file);
                sb.AppendLine($"  {relativePath} ({FormatFileSize(fileInfo.Length)})");
            }

            if (files.Length > 100)
            {
                sb.AppendLine();
                sb.AppendLine($"... and {files.Length - 100} more files");
            }

            return new ToolExecutionResult
            {
                Success = true,
                Output = sb.ToString()
            };
        }

        private async Task<ToolExecutionResult> CopyFileAsync(string sourcePath, string? destinationPath, CancellationToken cancellationToken)
        {
            if (destinationPath == null)
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = "Destination path is required for copy operation."
                };
            }

            if (!File.Exists(sourcePath))
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = $"Source file not found: {sourcePath}"
                };
            }

            // Create destination directory if needed
            var destDir = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            File.Copy(sourcePath, destinationPath, overwrite: true);

            return new ToolExecutionResult
            {
                Success = true,
                Output = $"Successfully copied {Path.GetFileName(sourcePath)} to {Path.GetFileName(destinationPath)}"
            };
        }

        private ToolExecutionResult MoveFile(string sourcePath, string? destinationPath)
        {
            if (destinationPath == null)
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = "Destination path is required for move operation."
                };
            }

            if (!File.Exists(sourcePath))
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = $"Source file not found: {sourcePath}"
                };
            }

            // Create destination directory if needed
            var destDir = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            File.Move(sourcePath, destinationPath, overwrite: true);

            return new ToolExecutionResult
            {
                Success = true,
                Output = $"Successfully moved {Path.GetFileName(sourcePath)} to {Path.GetFileName(destinationPath)}"
            };
        }

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}

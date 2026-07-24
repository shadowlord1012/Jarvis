using Jarvis.AI.Interfaces;
using Jarvis.AI.Providers;
using Jarvis.AI.Tool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Jarvis.Plugin.Weather
{
    /// <summary>
    /// Opens a weather search for a specified city and time.
    /// </summary>
    public sealed class WeatherTool : ITool
    {
        public string Name => "weather";

        public string Description =>
            "Shows the weather for a specified city and optional time period. " +
            "Use this tool when the user asks about current weather or a weather forecast.";

        public ToolDefinition Definition => new()
        {
            Name = Name,
            Description = Description,

            JsonSchema =
            """
            {
                "type": "object",
                "properties": {
                    "city": {
                        "type": "string",
                        "description": "The city to get the weather for."
                    },
                    "time": {
                        "type": "string",
                        "description": "The requested time period, such as today, tomorrow, this weekend, or Friday."
                    }
                },
                "required": ["city"]
            }
            """
        };

        public async Task<ToolExecutionResult> ExecuteAsync(
            ToolExecutionRequest request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                string? city = GetStringArgument(request, "city");
                string? time = GetStringArgument(request, "time");

                if (string.IsNullOrWhiteSpace(city))
                {
                    return new ToolExecutionResult
                    {
                        Success = false,
                        Error = "The city is missing for the weather report."
                    };
                }

                city = city.Trim();

                if (string.IsNullOrWhiteSpace(time))
                {
                    time = "today";
                }
                else
                {
                    time = time.Trim();
                }

                string searchQuery =
                    $"weather in {city} {time}";

                string url =
                    "https://www.google.com/search?q=" +
                    Uri.EscapeDataString(searchQuery);

                OpenBrowser(url);

                string message =
                    $"Showing the weather for {city}, {time}, sir.";

                return new ToolExecutionResult
                {
                    Success = true,
                    Output = message,
                    Data = new
                    {
                        city,
                        time,
                        searchQuery,
                        url
                    }
                };
            }
            catch (OperationCanceledException)
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = "The weather request was cancelled."
                };
            }
            catch (Exception ex)
            {
                return new ToolExecutionResult
                {
                    Success = false,
                    Error = $"I couldn't open the weather report: {ex.Message}"
                };
            }
        }

        private static string? GetStringArgument(
            ToolExecutionRequest request,
            string argumentName)
        {
            try
            {
                var arguments = JsonSerializer.Deserialize<Dictionary<string, object>>(
                    request.Arguments);

                if (arguments == null)
                {
                    return null;
                }

                if (!arguments.TryGetValue(argumentName, out object? value))
                {
                    return null;
                }

                if (value is JsonElement element)
                {
                    return element.GetString();
                }

                return value?.ToString();
            }
            catch
            {
                return null;
            }
        }

        private static void OpenBrowser(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}

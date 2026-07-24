using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Ollama
{
    public sealed class OllamaOptions
    {
        public string BaseUrl { get; set; }
            = "http://localhost:11434";

        public string DefaultModel { get; set; }
            = "qwen2.5:7b";

        public TimeSpan Timeout { get; set; }
            = TimeSpan.FromMinutes(10);
        /// <summary>
        /// HTTP request timeout in seconds.
        /// </summary>
        public int TimeoutSeconds { get; set; }
            = 600;

        /// <summary>
        /// Number of times a failed request should be retried.
        /// </summary>
        public int MaxRetries { get; set; }
            = 3;

        /// <summary>
        /// Whether Ollama should keep the model loaded.
        /// </summary>
        public bool KeepAlive { get; set; }
            = true;

        /// <summary>
        /// How long Ollama should keep the model in memory (e.g., "30m", "1h").
        /// Only applies if KeepAlive is true.
        /// </summary>
        public string KeepAliveDuration { get; set; }
            = "30m";

        /// <summary>
        /// Whether to pre-load the model at startup for faster first response.
        /// Runs in background and doesn't block app initialization.
        /// </summary>
        public bool EnableWarmup { get; set; }
            = true;

        /// <summary>
        /// Default temperature for LLM responses.
        /// </summary>
        public double Temperature { get; set; }
            = 0.7;

        /// <summary>
        /// Default maximum number of tokens.
        /// </summary>
        public int MaxTokens { get; set; }
            = 4096;

        /// <summary>
        /// Default top-p sampling value.
        /// </summary>
        public double TopP { get; set; }
            = 0.95;

        /// <summary>
        /// Whether streaming responses are enabled.
        /// </summary>
        public bool EnableStreaming { get; set; }
            = true;

        /// <summary>
        /// Whether tools should be sent to Ollama.
        /// </summary>
        public bool EnableToolCalling { get; set; }
            = true;
    }
}

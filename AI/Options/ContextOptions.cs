using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Options
{
    public sealed class ContextOptions
    {
        /// <summary>
        /// Maximum number of conversation messages included.
        /// </summary>
        public int MaximumConversationMessages { get; set; } = 20;

        /// <summary>
        /// Maximum number of conversation messages
        /// retained in the active context.
        /// </summary>
        public int MaximumMessages { get; set; } = 30;

        /// <summary>
        /// Maximum number of memories injected.
        /// </summary>
        public int MaximumMemories { get; set; } = 10;

        /// <summary>
        /// Whether long-term memory should be searched
        /// before constructing the prompt.
        /// </summary>
        public bool EnableMemory { get; set; } = true;

        /// <summary>
        /// Whether tool definitions should be included.
        /// </summary>
        public bool EnableTools { get; set; } = true;

        public bool IncludeConversation { get; set; } = true;

        public bool IncludeMemory { get; set; } = true;

        public bool IncludeTools { get; set; } = true;

        public bool IncludeSystemInformation { get; set; } = true;
    }
}

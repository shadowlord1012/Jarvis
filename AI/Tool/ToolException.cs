using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Tool
{
    public sealed class ToolException : Exception
    {
        public ToolException(string message)
            : base(message)
        {
        }

        public ToolException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

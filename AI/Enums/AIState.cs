using System;
using System.Collections.Generic;
using System.Text;

namespace AI.Enums
{
    public enum AIState
    {
        Offline,

        Initializing,

        Idle,

        Listening,

        Transcribing,

        Thinking,

        ExecutingTool,

        Speaking,

        Error
    }
}

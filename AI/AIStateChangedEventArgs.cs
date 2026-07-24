using AI.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI
{
    public sealed class AIStateChangedEventArgs : EventArgs
    {
        public AIStateChangedEventArgs(
            AIState previousState,
            AIState currentState)
        {
            PreviousState = previousState;
            CurrentState = currentState;
            Timestamp = DateTime.UtcNow;
        }

        public AIState PreviousState { get; }

        public AIState CurrentState { get; }

        public DateTime Timestamp { get; }
    }
}

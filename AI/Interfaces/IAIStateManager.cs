using AI.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.AI.Interfaces
{
    public interface IAIStateManager
    {
        AIState CurrentState { get; }

        AIState PreviousState { get; }

        bool IsBusy { get; }

        DateTime LastStateChange { get; }

        event EventHandler<AIStateChangedEventArgs>? StateChanged;

        void SetState(AIState state);

        bool IsState(AIState state);

        void Reset();
    }
}

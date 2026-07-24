using AI.Enums;
using Jarvis.AI.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Interfaces;

namespace Jarvis.AI.Manager
{
    public sealed class AIStateManager : IAIStateManager
    {
        private readonly ILogService _logger;

        private readonly object _syncRoot = new();

        public AIState CurrentState { get; private set; }
            = AIState.Offline;

        public AIState PreviousState { get; private set; }
            = AIState.Offline;

        public DateTime LastStateChange { get; private set; }
            = DateTime.UtcNow;

        public bool IsBusy =>
            CurrentState is AIState.Listening
            or AIState.Transcribing
            or AIState.Thinking
            or AIState.ExecutingTool
            or AIState.Speaking;

        public event EventHandler<AIStateChangedEventArgs>? StateChanged;

        public AIStateManager(
            ILogService logger)
        {
            _logger = logger;
        }

        public void SetState(AIState state)
        {
            lock (_syncRoot)
            {
                if (CurrentState == state)
                    return;

                PreviousState = CurrentState;
                CurrentState = state;
                LastStateChange = DateTime.UtcNow;
            }

            _logger.LogDebug(
                "AI State changed:",
                PreviousState + " - >" +
                CurrentState);

            StateChanged?.Invoke(
                this,
                new AIStateChangedEventArgs(
                    PreviousState,
                    CurrentState));
        }

        public bool IsState(AIState state)
        {
            return CurrentState == state;
        }

        public void Reset()
        {
            SetState(AIState.Idle);
        }
    }
}

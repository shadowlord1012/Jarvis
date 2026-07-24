using AI.Interfaces;
using Jarvis.UI.Controls.HUD.Widgets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.UI.Controls.HUD.Models
{
    public sealed class AIInputController
    {
        private readonly IAIService _aiService;

        private readonly HudInputWidget _inputWidget;

        public event EventHandler<string>?
            ResponseReceived;

        public event EventHandler<Exception>?
            ErrorOccurred;

        public event EventHandler?
            ResponseStarted;


        public event EventHandler<string>?
            ResponseChunkReceived;


        public event EventHandler?
            ResponseCompleted;

        public event EventHandler<string>?
            InputSubmittedEvent;

        public AIInputController(
            IAIService aiService,
            HudInputWidget inputWidget)
        {
            _aiService =
        aiService;

            _inputWidget =
                inputWidget;


            _inputWidget.InputSubmitted +=
                InputWidget_InputSubmitted;

            _inputWidget.InputCancelled +=
                InputWidget_InputCancelled;
        }

        // ============================================================
        // SUBMIT INPUT
        // ============================================================

        private async void InputWidget_InputSubmitted(
            object? sender,
            string input)
        {
            if (_inputWidget is null)
                return;

            // Raise event for external handlers (e.g., Voice Pipeline)
            InputSubmittedEvent?.Invoke(this, input);

            // Check for shutdown command
            if (IsShutdownCommand(input))
            {
                _inputWidget.SetStatus(
                    "SHUTTING DOWN");

                // Trigger shutdown via Application
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    System.Windows.Application.Current.Shutdown();
                });

                return;
            }

            try
            {
                _inputWidget.SetStatus(
                    "THINKING");

                // ========================================================
                // START RESPONSE
                // ========================================================

                ResponseStarted?
                    .Invoke(
                        this,
                        EventArgs.Empty);


                // ========================================================
                // STREAM RESPONSE
                // ========================================================

                await foreach (
                    var chunk
                    in _aiService
                        .ProcessStreamingAsync(
                            input)
                        .WithCancellation(
                            CancellationToken.None))
                {
                    if (!string.IsNullOrEmpty(
                            chunk.Content))
                    {
                        ResponseChunkReceived?
                            .Invoke(
                                this,
                                chunk.Content);
                    }


                    if (chunk.IsComplete)
                    {
                        break;
                    }
                }


                // ========================================================
                // RESPONSE COMPLETE
                // ========================================================

                ResponseCompleted?
                    .Invoke(
                        this,
                        EventArgs.Empty);


                _inputWidget.SetStatus(
                    "READY");

            }
            catch (Exception ex)
            {
                _inputWidget.SetStatus(
                    "ERROR");

                ErrorOccurred?
                    .Invoke(
                        this,
                        ex);
            }
        }

        // ============================================================
        // CHECK SHUTDOWN COMMAND
        // ============================================================

        private static bool IsShutdownCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            var normalized = input.Trim().ToLowerInvariant();

            return normalized == "shutdown" ||
                   normalized == "exit" ||
                   normalized == "quit" ||
                   normalized == "close";
        }

        // ============================================================
        // CANCEL
        // ============================================================

        private void InputWidget_InputCancelled(
            object? sender,
            EventArgs e)
        {
            _inputWidget.SetStatus(
                "READY");
        }
    }
}

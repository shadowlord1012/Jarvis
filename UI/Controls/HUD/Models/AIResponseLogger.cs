using Jarvis.UI.Controls.HUD.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Widgets;

namespace UI.Controls.HUD.Models
{
    /// <summary>
    /// Connects AI responses and user input to the HUD LogWidget.
    /// </summary>
    public sealed class AIResponseLogger
    {
        private readonly LogWidget _logWidget;

        private AIInputController? _aiInputController;
        private readonly StringBuilder  _currentResponse = new();

        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public AIResponseLogger(
            LogWidget logWidget)
        {
            _logWidget =
                logWidget
                ?? throw new ArgumentNullException(
                    nameof(logWidget));
        }


        // ============================================================
        // CONNECT AI CONTROLLER
        // ============================================================

        public void Connect(
            AIInputController aiInputController)
        {
            if (aiInputController is null)
            {
                throw new ArgumentNullException(
                    nameof(aiInputController));
            }


            // --------------------------------------------------------
            // DISCONNECT EXISTING CONTROLLER
            // --------------------------------------------------------

            if (_aiInputController is not null)
            {
                _aiInputController.ResponseReceived -=
                    OnResponseReceived;

                _aiInputController.ErrorOccurred -=
                    OnErrorOccurred;
            }


            // --------------------------------------------------------
            // CONNECT NEW CONTROLLER
            // --------------------------------------------------------

            _aiInputController =
                aiInputController;


            _aiInputController.ResponseReceived +=
                OnResponseReceived;

            _aiInputController.ErrorOccurred +=
                OnErrorOccurred;

            _aiInputController.ResponseStarted +=
                OnResponseStarted;

            _aiInputController.ResponseChunkReceived +=
                OnResponseChunkReceived;

            _aiInputController.ResponseCompleted +=
                OnResponseCompleted;

        }


        // ============================================================
        // DISCONNECT
        // ============================================================

        public void Disconnect()
        {
            if (_aiInputController is null)
            {
                return;
            }


            _aiInputController.ResponseReceived -=
                OnResponseReceived;

            _aiInputController.ErrorOccurred -=
                OnErrorOccurred;


            _aiInputController =
                null;
        }


        // ============================================================
        // AI RESPONSE
        // ============================================================

        private void OnResponseReceived(
            object? sender,
            string response)
        {
            if (string.IsNullOrWhiteSpace(
                    response))
            {
                return;
            }


            _logWidget.Log(
                $"AI RESPONSE > {response}");
        }


        // ============================================================
        // AI ERROR
        // ============================================================

        private void OnErrorOccurred(
            object? sender,
            Exception exception)
        {
            if (exception is null)
            {
                return;
            }


            _logWidget.Log(
                $"AI ERROR > {exception.Message}");
        }

        private void OnResponseChunkReceived(
            object? sender,
            string chunk)
        {
            if (string.IsNullOrEmpty(
                    chunk))
            {
                return;
            }


            _currentResponse.Append(
                chunk);


            _logWidget.AppendAIResponse(
                chunk);
        }
        private void OnResponseCompleted(
            object? sender,
            EventArgs e)
        {
            var response =
                _currentResponse
                    .ToString();


            if (string.IsNullOrWhiteSpace(
                    response))
            {
                return;
            }


            _logWidget.EndAIResponse();
        }

        private void OnResponseStarted(
            object? sender,
            EventArgs e)
        {
            _currentResponse.Clear();
            _logWidget.BeginAIResponse();
        }
    }
}

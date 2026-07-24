using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.UI.Controls.HUD.Models
{
    public sealed class AIResponseEventArgs
    : EventArgs
    {
        public string Response { get; }

        public AIResponseEventArgs(
            string response)
        {
            Response =
                response;
        }
    }
}

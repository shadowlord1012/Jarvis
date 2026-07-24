using System;
using System.Collections.Generic;
using System.Text;

namespace UI.Controls.HUD.Models
{
    public sealed class HudEffects
    {
        //---------------------------------------
        // Global Time
        //---------------------------------------

        public double Time { get; internal set; }

        //---------------------------------------
        // Shared Animation Values
        //---------------------------------------

        public double Pulse { get; internal set; }

        public double SlowPulse { get; internal set; }

        public double FastPulse { get; internal set; }

        public double Glow { get; internal set; }

        public double Shimmer { get; internal set; }

        public double Scan { get; internal set; }

        public double Warning { get; internal set; }

        public double Listening { get; internal set; }

        public double Speaking { get; internal set; }

        public double Thinking { get; internal set; }
    }
}

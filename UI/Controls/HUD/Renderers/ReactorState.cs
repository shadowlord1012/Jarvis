using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using UI.Controls.HUD.Definitions;

namespace UI.Controls.HUD.Renderers
{
    public sealed class ReactorState
    {
        //---------------------------------------
        // Position
        //---------------------------------------

        public Point Center { get; set; }

        //---------------------------------------
        // Time
        //---------------------------------------

        public double Time { get; set; }

        //---------------------------------------
        // Animation
        //---------------------------------------

        public double OuterRotation { get; set; }

        public double SegmentRotation { get; set; }

        public double TickRotation { get; set; }

        public double CoreRotation { get; set; }

        public double Pulse { get; set; }

        //---------------------------------------
        // Energy
        //---------------------------------------

        public double Energy { get; set; } = 1.0;

        //---------------------------------------
        // Voice Activity
        //---------------------------------------

        public double VoiceLevel { get; set; }

        //---------------------------------------
        // Status
        //---------------------------------------

        public bool IsListening { get; set; }

        public bool IsThinking { get; set; }

        public bool IsSpeaking { get; set; }

        //---------------------------------------
        // Alerts
        //---------------------------------------

        public bool AlertMode { get; set; }

        //---------------------------------------
        // Orbit Nodes
        //---------------------------------------
        public List<OrbitNodeDefinition> OrbitNodes { get; } = new();

    }
}

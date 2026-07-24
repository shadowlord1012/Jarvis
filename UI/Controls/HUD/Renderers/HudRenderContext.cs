using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Managers;

namespace UI.Controls.HUD.Renderers
{
    public sealed class HudRenderContext
    {
        public double Width { get; internal set; }

        public double Height { get; internal set; }

        public HudLayerManager Layers { get; }

        public TimeSpan ElapsedTime { get; internal set; }

        public double DeltaTime { get; internal set; }

        public HudRenderContext(HudLayerManager layers)
        {
            Layers = layers;
        }
    }
}

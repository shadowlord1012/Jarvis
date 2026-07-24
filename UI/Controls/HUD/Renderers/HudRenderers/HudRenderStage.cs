using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Interfaces;

namespace UI.Controls.HUD.Renderers.HudRenderers
{
    public sealed class HudRenderStage
    {
        public int Priority { get; init; }

        public IHudRenderer Renderer { get; init; } = default!;
    }
}

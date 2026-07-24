using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using UI.Controls.HUD.Enums;

namespace UI.Controls.HUD.Managers
{
    public sealed class HudLayerManager
    {
        private readonly Dictionary<HudLayer, Canvas> _layers = new();

        public void Register(HudLayer layer, Canvas canvas)
        {
            _layers[layer] = canvas;
        }

        public Canvas GetLayer(HudLayer layer)
        {
            return _layers[layer];
        }

        public void Clear(HudLayer layer)
        {
            _layers[layer].Children.Clear();
        }

        public void ClearAll()
        {
            foreach (var canvas in _layers.Values)
            {
                canvas.Children.Clear();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Managers;

namespace UI.Controls.HUD.Dashboard
{
    public sealed class DashboardPanelRenderer : IHudRenderer
    {
        private readonly Canvas _canvas;

        private readonly DashboardManager _manager;

        private readonly DashboardLayoutEngine _layout =
            new();

        public DashboardPanelRenderer(
            HudLayerManager layers,
            DashboardManager manager)
        {
            _canvas =
                layers.GetLayer(HudLayer.Panels);

            _manager = manager;
        }

        public void Initialize(
            double width,
            double height)
        {
            _layout.Layout(
                _manager,
                width,
                height);
        }

        public void Update(double deltaTime)
        {
            _canvas.Children.Clear();

            foreach (var panel in _manager.Panels)
            {
                if (!panel.Visible)
                    continue;

                panel.Widget.Update(deltaTime);

            /*    panel.Widget.Render(
                    _canvas,
                    panel.Bounds);*/
            }
        }

        public void Resize(
            double width,
            double height)
        {
            _layout.Layout(
                _manager,
                width,
                height);
        }

        public void Dispose()
        {
            _canvas.Children.Clear();
        }
    }
}

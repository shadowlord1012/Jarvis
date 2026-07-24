using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Factories;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Managers;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Renderers.HudRenderers
{
    public class GlowRenderer : IHudRenderer
    {
        private readonly Canvas _canvas;

        private readonly HudTheme _theme;

        private Ellipse? _glow;

        private double _time;

        public GlowRenderer(HudLayerManager layers, HudTheme theme)
        {
            _canvas = layers.GetLayer(HudLayer.Effects);
            _theme = theme;
        }

        public void Initialize(double width, double height)
        {
            _canvas.Children.Clear();

            _glow = HudShapeFactory.Ellipse(
                    _theme.GlowSize,
                    _theme.GlowSize,
                    HudBrushFactory.Glow(
                        Color.FromArgb(255, 0, 255, 255)),
                    _theme.GlowOpacity);
           

            Canvas.SetLeft(_glow, width / 2 - 350);
            Canvas.SetTop(_glow, height / 2 - 350);

            _canvas.Children.Add(_glow);
        }

        public void Update(double deltaTime)
        {
            if (_glow == null)
                return;

            _time += deltaTime;

            _glow.Opacity =
                .08 + (.03 * System.Math.Sin(_time * 2));
        }

        public void Resize(double width, double height)
        {
            if (_glow == null)
                return;

            Canvas.SetLeft(_glow, width / 2 - 350);
            Canvas.SetTop(_glow, height / 2 - 350);
        }

        public void Dispose()
        {
            _canvas.Children.Clear();
        }
    }
}

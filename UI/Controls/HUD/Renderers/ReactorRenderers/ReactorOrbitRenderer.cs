using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Shapes;
using UI.Controls.HUD.Definitions;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Factories;
using UI.Controls.HUD.Managers;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Renderers.ReactorRenderers
{
    /// <summary>
    /// Draws and animates the reactor orbit nodes.
    /// </summary>
    public sealed class ReactorOrbitRenderer : BaseReactorRenderer
    {
        private sealed class OrbitVisual
        {
            public OrbitNodeDefinition Node = default!;

            public Ellipse Circle = default!;

            public Ellipse Glow = default!;

            public TextBlock? Label;
        }

        private readonly List<OrbitVisual> _visuals = new();

        public ReactorOrbitRenderer(
            HudLayerManager layers,
            ReactorManager reactor)
            : base(layers, reactor)
        {
        }

        //--------------------------------------------------

        protected override void Build()
        {
            _visuals.Clear();

            foreach (OrbitNodeDefinition node in State.OrbitNodes)
            {
                Ellipse glow = CreateOrb(
                    node.Size * 0.65,
                    Theme.OrbitGlowOpacity);

                glow.Fill = Theme.ReactorHighlightBrush;

                ApplyGlow(glow);

                Add(
                    glow,
                    0,
                    0);

                Ellipse circle = CreateOrb(
                    node.Size * 0.5,
                    Theme.OrbitOpacity);

                circle.Fill = Theme.ReactorCoreBrush;

                Add(
                    circle,
                    0,
                    0);

                TextBlock? label = null;

                if (Theme.ShowOrbitLabels)
                {
                    label = new TextBlock
                    {
                        Text = node.Label,
                        FontSize = Theme.OrbitLabelFontSize,
                        Foreground = Theme.TextBrush,
                        IsHitTestVisible = false
                    };

                    Add(label);
                }

                _visuals.Add(
                    new OrbitVisual
                    {
                        Node = node,
                        Circle = circle,
                        Glow = glow,
                        Label = label
                    });
            }
        }

        //--------------------------------------------------

        public override void Update(
            double deltaTime)
        {
            foreach (OrbitVisual visual in _visuals)
            {
                OrbitNodeDefinition node = visual.Node;

                double direction =
                    node.Clockwise ? 1 : -1;

                node.Angle +=
                    direction *
                    node.OrbitSpeed *
                    deltaTime;

                if (node.Angle >= 360)
                    node.Angle -= 360;

                if (node.Angle < 0)
                    node.Angle += 360;

                double radians =
                    node.Angle *
                    Math.PI /
                    180.0;

                double x =
                    Center.X +
                    node.Radius *
                    Math.Cos(radians);

                double y =
                    Center.Y +
                    node.Radius *
                    Math.Sin(radians);

                Canvas.SetLeft(
                    visual.Circle,
                    x - visual.Circle.Width / 2);

                Canvas.SetTop(
                    visual.Circle,
                    y - visual.Circle.Height / 2);

                Canvas.SetLeft(
                    visual.Glow,
                    x - visual.Glow.Width / 2);

                Canvas.SetTop(
                    visual.Glow,
                    y - visual.Glow.Height / 2);

                visual.Glow.Opacity =
                    Theme.OrbitGlowOpacity *
                    (0.80 +
                     0.20 *
                     Math.Sin(node.Angle * Math.PI / 180));

                if (visual.Label != null)
                {
                    Canvas.SetLeft(
                        visual.Label,
                        x + Theme.OrbitLabelOffsetX);

                    Canvas.SetTop(
                        visual.Label,
                        y + Theme.OrbitLabelOffsetY);
                }
            }
        }

        //--------------------------------------------------

        protected override void OnThemeChanged(
            HudTheme theme)
        {
            base.OnThemeChanged(theme);
        }

        //--------------------------------------------------

        public override void Dispose()
        {
            _visuals.Clear();

            base.Dispose();
        }
    }
}

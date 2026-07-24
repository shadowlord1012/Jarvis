using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using UI.Controls.HUD.Definitions;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Renderers
{
    public sealed class ReactorManager
    {
        public ReactorState State { get; }

        public ReactorAnimation Animation { get; }

        public ReactorManager(HudTheme theme)
        {
            State = new ReactorState();

            Animation = new ReactorAnimation(
                State,
                theme);

            State.Center = theme.DefaultScreenSize;
            setup();
        }

        private void setup()
        {
            State.OrbitNodes.AddRange(new[]
            {
                new OrbitNodeDefinition
                {
                    Id = "CPU",
                    Label = "CPU",
                    Radius = 135,
                    Angle = 0
                },

                new OrbitNodeDefinition
                {
                    Id = "MEM",
                    Label = "MEM",
                    Radius = 135,
                    Angle = 60
                },

                new OrbitNodeDefinition
                {
                    Id = "NET",
                    Label = "NET",
                    Radius = 135,
                    Angle = 120
                },

                new OrbitNodeDefinition
                {
                    Id = "AI",
                    Label = "AI",
                    Radius = 135,
                    Angle = 180
                },

                new OrbitNodeDefinition
                {
                    Id = "AUDIO",
                    Label = "AUDIO",
                    Radius = 135,
                    Angle = 240
                },

                new OrbitNodeDefinition
                {
                    Id = "PLUGIN",
                    Label = "PLUGIN",
                    Radius = 135,
                    Angle = 300
                }
            });
        }

        public void Update(double deltaTime)
        {
            Animation.Update(deltaTime);
        }
        public void Resize(double width, double height)
        {
            State.Center = new Point(width / 2, height / 2);
        }
    }
}

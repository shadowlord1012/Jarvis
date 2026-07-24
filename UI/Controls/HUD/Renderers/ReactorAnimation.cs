using System;
using System.Collections.Generic;
using System.Text;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Renderers
{
    public sealed class ReactorAnimation
    {
        private readonly ReactorState _state;
        private readonly HudTheme _theme;

        public ReactorAnimation(
            ReactorState state,
            HudTheme theme)
        {
            _state = state;
            _theme = theme;
        }

        public void Update(double deltaTime)
        {
            _state.Time += deltaTime;

            _state.OuterRotation +=
                _theme.ReactorRotationSpeed *
                deltaTime;

            _state.SegmentRotation +=
                _theme.ReactorSegmentRotationSpeed *
                deltaTime;

            _state.TickRotation +=
                _theme.ReactorTickRotationSpeed *
                deltaTime;

            _state.CoreRotation +=
                _theme.CoreRotationSpeed *
                deltaTime;

            _state.Pulse =
                (Math.Sin(
                    _state.Time *
                    _theme.CorePulseSpeed) + 1) * 0.5;
        }
    }
}

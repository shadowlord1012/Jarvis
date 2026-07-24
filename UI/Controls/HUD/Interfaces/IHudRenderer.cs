using System;
using System.Collections.Generic;
using System.Text;

namespace UI.Controls.HUD.Interfaces
{
    public interface IHudRenderer : IDisposable
    {
        void Initialize(
            double width,
            double height);

        void Update(
            double deltaTime);

        void Resize(
            double width,
            double height);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UI.Controls.HUD.Interfaces
{
    /// <summary>
    /// Base interface implemented by every HUD widget.
    /// </summary>
    public interface IHudWidget : IDisposable
    {
        /// <summary>
        /// Unique widget identifier.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Display name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Whether the widget is currently visible.
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Current widget position.
        /// </summary>
        Point Position { get; }

        /// <summary>
        /// Current widget size.
        /// </summary>
        Size Size { get; }

        /// <summary>
        /// Initializes the widget.
        /// </summary>
        void Initialize(double screenWidth, double screenHeight);

        /// <summary>
        /// Updates the widget every frame.
        /// </summary>
        void Update(double deltaTime);

        /// <summary>
        /// Called whenever the HUD is resized.
        /// </summary>
        void Resize(double screenWidth, double screenHeight);

        /// <summary>
        /// Changes the widget position.
        /// </summary>
        void SetPosition(Point position);

        /// <summary>
        /// Changes the widget size.
        /// </summary>
        void SetSize(Size size);

        /// <summary>
        /// Makes the widget visible.
        /// </summary>
        void Show();

        /// <summary>
        /// Hides the widget.
        /// </summary>
        void Hide();
    }
}

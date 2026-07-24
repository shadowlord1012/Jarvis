using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Widgets
{
    /// <summary>
    /// Base class for every HUD widget.
    /// Handles lifecycle, sizing, positioning, visibility and disposal.
    /// </summary>
    public abstract class HudWidgetBase : IHudWidget
    {
        protected readonly Canvas ParentCanvas;
        protected readonly HudTheme Theme;

        protected FrameworkElement? RootElement;

        protected double ScreenWidth;
        protected double ScreenHeight;

        protected bool IsInitialized;

        protected HudWidgetBase(
            Canvas parentCanvas,
            HudTheme theme,
            WidgetConfiguration configuration)
        {
            ParentCanvas = parentCanvas ??
                           throw new ArgumentNullException(nameof(parentCanvas));

            Theme = theme ??
                    throw new ArgumentNullException(nameof(theme));

            Configuration = configuration ??
                            throw new ArgumentNullException(nameof(configuration));
        }

        #region Configuration

        public WidgetConfiguration Configuration { get; }

        public string Id => Configuration.Id;

        public string Name => Configuration.Name;

        public bool IsVisible => Configuration.Visible;

        public Point Position => Configuration.Position;

        public Size Size => Configuration.Size;

        #endregion

        #region Lifecycle

        public virtual void Initialize(
            double screenWidth,
            double screenHeight)
        {
            if (IsInitialized)
                return;

            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;

            RootElement = CreateVisual();

            if (RootElement == null)
                throw new InvalidOperationException(
                    $"{GetType().Name} did not create a RootElement.");

            RootElement.Width = Configuration.Size.Width;
            RootElement.Height = Configuration.Size.Height;
            RootElement.Opacity = Configuration.Opacity;

            Canvas.SetZIndex(
                RootElement,
                Configuration.ZIndex);

            ApplyLayout();

            ParentCanvas.Children.Add(RootElement);

            if (!Configuration.Visible)
                RootElement.Visibility = Visibility.Collapsed;

            IsInitialized = true;
        }

        public virtual void Update(double deltaTime)
        {
        }

        public virtual void Resize(
            double screenWidth,
            double screenHeight)
        {
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;

            ApplyLayout();
        }

        #endregion

        #region Visibility

        public virtual void Show()
        {
            Configuration.Visible = true;

            if (RootElement != null)
                RootElement.Visibility = Visibility.Visible;
        }

        public virtual void Hide()
        {
            Configuration.Visible = false;

            if (RootElement != null)
                RootElement.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Position

        public virtual void SetPosition(Point position)
        {
            Configuration.Position = position;

            ApplyLayout();
        }

        public virtual void SetSize(Size size)
        {
            Configuration.Size = size;

            if (RootElement != null)
            {
                RootElement.Width = size.Width;
                RootElement.Height = size.Height;
            }

            ApplyLayout();
        }

        protected virtual void ApplyLayout()
        {
            if (RootElement == null)
                return;

            Point position = WidgetLayoutEngine.CalculatePosition(
                ScreenWidth,
                ScreenHeight,
                Configuration);

            Canvas.SetLeft(
                RootElement,
                position.X);

            Canvas.SetTop(
                RootElement,
                position.Y);
        }

        #endregion

        #region Rendering

        /// <summary>
        /// Creates the widget's root visual.
        /// Called once during initialization.
        /// </summary>
        protected abstract FrameworkElement CreateVisual();

        #endregion

        #region Disposal

        public virtual void Dispose()
        {
            if (RootElement != null)
            {
                ParentCanvas.Children.Remove(RootElement);
                RootElement = null;
            }

            IsInitialized = false;
        }

        #endregion
    }
}

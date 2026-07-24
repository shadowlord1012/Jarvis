using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Managers
{
    /// <summary>
    /// Manages all HUD widgets.
    /// </summary>
    public class HudWidgetManager : IDisposable
    {
        private readonly Canvas _widgetLayer;

        private readonly HudTheme _theme;

        private readonly List<IHudWidget> _widgets = new();

        private double _screenWidth;

        private double _screenHeight;

        public HudWidgetManager(
            HudLayerManager layers,
            HudTheme theme)
        {
            _widgetLayer = layers.GetLayer(HudLayer.Widgets);

            _theme = theme;
        }

        public IReadOnlyCollection<IHudWidget> Widgets =>
            _widgets.AsReadOnly();

        public void Register(IHudWidget widget)
        {
            ArgumentNullException.ThrowIfNull(widget);

            if (_widgets.Contains(widget))
                return;

            _widgets.Add(widget);
        }

        public void Unregister(IHudWidget widget)
        {
            if (!_widgets.Remove(widget))
                return;

            widget.Dispose();
        }

        public void Initialize(
            double width,
            double height)
        {
            _screenWidth = width;
            _screenHeight = height;

            foreach (var widget in _widgets)
            {
                widget.Initialize(width, height);
            }
        }

        public void Update(double deltaTime)
        {
            foreach (var widget in _widgets)
            {
                widget.Update(deltaTime);
            }
        }

        public void Resize(
            double width,
            double height)
        {
            _screenWidth = width;
            _screenHeight = height;

            foreach (var widget in _widgets)
            {
                widget.Resize(width, height);
            }
        }

        public T? GetWidget<T>()
            where T : class, IHudWidget
        {
            return _widgets.OfType<T>().FirstOrDefault();
        }

        public IEnumerable<T> GetWidgets<T>()
            where T : class, IHudWidget
        {
            return _widgets.OfType<T>();
        }

        public void ShowAll()
        {
            foreach (var widget in _widgets)
            {
                widget.Show();
            }
        }

        public void HideAll()
        {
            foreach (var widget in _widgets)
            {
                widget.Hide();
            }
        }

        public void Clear()
        {
            foreach (var widget in _widgets)
            {
                widget.Dispose();
            }

            _widgets.Clear();

            _widgetLayer.Children.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}

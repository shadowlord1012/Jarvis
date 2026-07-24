using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using UI.Controls.HUD.Enums;

namespace UI.Controls.HUD.Widgets
{
    /// <summary>
    /// Configuration settings for a HUD widget.
    /// </summary>
    public sealed class WidgetConfiguration
    {
        /// <summary>
        /// Unique widget identifier.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Display name.
        /// </summary>
        public string Name { get; set; } = "Widget";

        /// <summary>
        /// Widget type.
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Determines whether the widget is enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Determines whether the widget is visible.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Docking position.
        /// </summary>
        public WidgetDock Dock { get; set; } = WidgetDock.None;

        /// <summary>
        /// Manual widget position.
        /// Ignored when Dock != None.
        /// </summary>
        public Point Position { get; set; } = new Point(0, 0);

        /// <summary>
        /// Widget size.
        /// </summary>
        public Size Size { get; set; } = new Size(300, 150);

        /// <summary>
        /// Distance from the dock edge.
        /// </summary>
        public Thickness Margin { get; set; } = new Thickness(20);

        /// <summary>
        /// Additional offset from the calculated dock position.
        /// Applied after dock position is calculated.
        /// </summary>
        public Point Offset { get; set; } = new Point(0, 0);

        /// <summary>
        /// Widget opacity.
        /// </summary>
        public double Opacity { get; set; } = 1.0;

        /// <summary>
        /// Display order.
        /// </summary>
        public int ZIndex { get; set; } = 0;

        /// <summary>
        /// Allow dragging.
        /// </summary>
        public bool Draggable { get; set; } = false;

        /// <summary>
        /// Allow resizing.
        /// </summary>
        public bool Resizable { get; set; } = false;

        /// <summary>
        /// Automatically hide when inactive.
        /// </summary>
        public bool AutoHide { get; set; } = false;

        /// <summary>
        /// Refresh rate in frames per second.
        /// 0 = Every frame.
        /// </summary>
        public double RefreshRate { get; set; } = 0;

        /// <summary>
        /// Optional tag for grouping.
        /// </summary>
        public string Category { get; set; } = "General";

        /// <summary>
        /// Reserved for future plugin-specific settings.
        /// </summary>
        public Dictionary<string, object> Properties { get; } = new();
    }
}

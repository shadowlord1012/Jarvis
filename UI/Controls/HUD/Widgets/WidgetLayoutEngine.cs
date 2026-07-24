using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using UI.Controls.HUD.Enums;

namespace UI.Controls.HUD.Widgets
{
    /// <summary>
    /// Calculates screen positions for HUD widgets.
    /// </summary>
    public static class WidgetLayoutEngine
    {
        public static Point CalculatePosition(
            double screenWidth,
            double screenHeight,
            WidgetConfiguration config)
        {
            if (config.Dock == WidgetDock.None)
                return config.Position;

            double x = 0;
            double y = 0;

            double width = config.Size.Width;
            double height = config.Size.Height;

            Thickness margin = config.Margin;

            switch (config.Dock)
            {
                case WidgetDock.TopLeft:
                    x = margin.Left;
                    y = margin.Top + 50;
                    break;

                case WidgetDock.TopCenter:
                    x = (screenWidth - width) / 2;
                    y = margin.Top;
                    break;

                case WidgetDock.TopRight:
                    x = screenWidth - width - margin.Right;
                    y = margin.Top + 50;
                    break;

                case WidgetDock.Left:
                    x = margin.Left;
                    y = (screenHeight - height) / 2;
                    break;

                case WidgetDock.Center:
                    x = (screenWidth - width) / 2;
                    y = (screenHeight - height) / 2;
                    break;

                case WidgetDock.Right:
                    x = screenWidth - width - margin.Right;
                    y = (screenHeight - height) / 2;
                    break;

                case WidgetDock.BottomLeft:
                    x = margin.Left;
                    y = screenHeight - height - margin.Bottom;
                    break;

                case WidgetDock.BottomCenter:
                    x = (screenWidth - width) / 2;
                    y = screenHeight - height - margin.Bottom;
                    break;

                case WidgetDock.BottomRight:
                    x = screenWidth - width - margin.Right;
                    y = screenHeight - height - margin.Bottom;
                    break;

                default:
                    x = config.Position.X;
                    y = config.Position.Y;
                    break;
            }

            // Apply offset to the calculated position
            x += config.Offset.X;
            y += config.Offset.Y;

            return new Point(x, y);
        }
    }
}

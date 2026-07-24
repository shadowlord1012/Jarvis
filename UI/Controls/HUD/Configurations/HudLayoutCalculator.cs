using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using UI.Controls.HUD.Enums;

namespace UI.Controls.HUD.Configurations
{
    /// <summary>
    /// Calculates HUD layout positions.
    /// Contains no rendering logic.
    /// </summary>
    public sealed class HudLayoutCalculator
    {
        private readonly HudLayoutSettings _settings;

        public HudLayoutCalculator(HudLayoutSettings? settings = null)
        {
            _settings = settings ?? new HudLayoutSettings();
        }

        public Rect GetDockedRect(
            Size viewport,
            Size element,
            HudDock dock)
        {
            double margin = _settings.Margin;

            return dock switch
            {
                HudDock.TopLeft =>
                    new Rect(
                        margin,
                        margin,
                        element.Width,
                        element.Height),

                HudDock.TopCenter =>
                    new Rect(
                        (viewport.Width - element.Width) / 2,
                        margin,
                        element.Width,
                        element.Height),

                HudDock.TopRight =>
                    new Rect(
                        viewport.Width - element.Width - margin,
                        margin,
                        element.Width,
                        element.Height),

                HudDock.Left =>
                    new Rect(
                        margin,
                        (viewport.Height - element.Height) / 2,
                        element.Width,
                        element.Height),

                HudDock.Center =>
                    new Rect(
                        (viewport.Width - element.Width) / 2,
                        (viewport.Height - element.Height) / 2,
                        element.Width,
                        element.Height),

                HudDock.Right =>
                    new Rect(
                        viewport.Width - element.Width - margin,
                        (viewport.Height - element.Height) / 2,
                        element.Width,
                        element.Height),

                HudDock.BottomLeft =>
                    new Rect(
                        margin,
                        viewport.Height - element.Height - margin,
                        element.Width,
                        element.Height),

                HudDock.BottomCenter =>
                    new Rect(
                        (viewport.Width - element.Width) / 2,
                        viewport.Height - element.Height - margin,
                        element.Width,
                        element.Height),

                HudDock.BottomRight =>
                    new Rect(
                        viewport.Width - element.Width - margin,
                        viewport.Height - element.Height - margin,
                        element.Width,
                        element.Height),

                _ => throw new ArgumentOutOfRangeException(nameof(dock))
            };
        }

        public Point GetCenter(Size viewport)
        {
            return new Point(
                viewport.Width / 2,
                viewport.Height / 2);
        }

        public Rect Inflate(Rect rect, double amount)
        {
            rect.Inflate(amount, amount);
            return rect;
        }

        public Rect Deflate(Rect rect, double amount)
        {
            rect.Inflate(-amount, -amount);
            return rect;
        }

        public Rect GetSafeArea(Size viewport)
        {
            double s = _settings.SafeArea;

            return new Rect(
                s,
                s,
                viewport.Width - (s * 2),
                viewport.Height - (s * 2));
        }

        public Rect[] SplitHorizontal(Rect rect, int count)
        {
            Rect[] result = new Rect[count];

            double spacing = _settings.PanelSpacing;

            double width =
                (rect.Width - (spacing * (count - 1))) / count;

            for (int i = 0; i < count; i++)
            {
                result[i] = new Rect(
                    rect.Left + i * (width + spacing),
                    rect.Top,
                    width,
                    rect.Height);
            }

            return result;
        }

        public Rect[] SplitVertical(Rect rect, int count)
        {
            Rect[] result = new Rect[count];

            double spacing = _settings.PanelSpacing;

            double height =
                (rect.Height - (spacing * (count - 1))) / count;

            for (int i = 0; i < count; i++)
            {
                result[i] = new Rect(
                    rect.Left,
                    rect.Top + i * (height + spacing),
                    rect.Width,
                    height);
            }

            return result;
        }

        public Point Snap(Point point, double grid)
        {
            return new Point(
                Math.Round(point.X / grid) * grid,
                Math.Round(point.Y / grid) * grid);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UI.Controls.HUD.Abstract;
using UI.Controls.HUD.Configurations;
using UI.Controls.HUD.Dashboard;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Factories
{
    /// <summary>
    /// Creates reusable HUD panel visuals.
    /// </summary>
    public class HudPanelFactory
    {

        private static readonly Dictionary<string, HudPanelDefinition> _definitions =
            new();

        private static readonly Dictionary<string, HudPanel> _activePanels =
            new();

        public Canvas CreatePanel(
            Rect bounds,
            HudTheme theme,
            PanelStyleEnum style = PanelStyleEnum.Standard)
        {
            Canvas panel = new()
            {
                Width = bounds.Width,
                Height = bounds.Height,
                IsHitTestVisible = false
            };

            panel.Children.Add(CreateBackground(bounds, theme, style));
            panel.Children.Add(CreateBorder(bounds, theme, style));
            //panel.Children.Add(CreateInnerFrame(bounds, theme, style));

            return panel;
        }

        public Rectangle CreateBackground(
            Rect bounds,
            HudTheme theme,
            PanelStyleEnum style)
        {
            return new Rectangle
            {
                Width = bounds.Width,
                Height = bounds.Height,

                RadiusX = style == PanelStyleEnum.Rounded ? 8 : 0,
                RadiusY = style == PanelStyleEnum.Rounded ? 8 : 0,

                Fill = CreateBackgroundBrush(theme),

                StrokeThickness = 0,

                SnapsToDevicePixels = true,

                IsHitTestVisible = false
            };
        }

        public Rectangle CreateBorder(
            Rect bounds,
            HudTheme theme,
            PanelStyleEnum style)
        {
            return new Rectangle
            {
                Width = bounds.Width,
                Height = bounds.Height,

                RadiusX = style == PanelStyleEnum.Rounded ? 8 : 0,
                RadiusY = style == PanelStyleEnum.Rounded ? 8 : 0,

                Stroke = theme.PrimaryBrush,

                StrokeThickness = 2,

                Fill = Brushes.Transparent,

                SnapsToDevicePixels = true,

                IsHitTestVisible = false
            };
        }

        public Rectangle CreateInnerFrame(
            Rect bounds,
            HudTheme theme,
            PanelStyleEnum style)
        {
            Rectangle frame = new()
            {
                Width = bounds.Width - 16,
                Height = bounds.Height - 16,

                RadiusX = style == PanelStyleEnum.Rounded ? 6 : 0,
                RadiusY = style == PanelStyleEnum.Rounded ? 6 : 0,

                Stroke = theme.SecondaryBrush,

                StrokeThickness = 1,

                Fill = Brushes.Transparent,

                Margin = new Thickness(8),

                Opacity = .55,

                IsHitTestVisible = false
            };

            Canvas.SetLeft(frame, 8);
            Canvas.SetTop(frame, 8);

            return frame;
        }

        public Line CreateHeaderDivider(
            double width,
            HudTheme theme)
        {
            return new Line
            {
                X1 = 12,
                Y1 = 30,

                X2 = width - 12,
                Y2 = 30,

                Stroke = theme.PrimaryBrush,

                StrokeThickness = 1.5,

                Opacity = .75
            };
        }

        public TextBlock CreateHeader(
            string title,
            HudTheme theme)
        {
            return new TextBlock
            {
                Text = title,

                Margin = new Thickness(14, 8, 0, 0),

                FontSize = 12,

                FontWeight = FontWeights.Bold,

                Foreground = theme.PrimaryBrush
            };
        }

        public Canvas CreateContentHost(
            double width,
            double height)
        {
            Canvas canvas = new()
            {
                Width = width - 20,
                Height = height - 42,

                IsHitTestVisible = false
            };

            Canvas.SetLeft(canvas, 10);
            Canvas.SetTop(canvas, 36);

            return canvas;
        }

        public Brush CreateBackgroundBrush(
            HudTheme theme)
        {
            LinearGradientBrush brush = new()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1)
            };

            Color c = theme.BackgroundDark;

            brush.GradientStops.Add(
                new GradientStop(
                    Color.FromArgb(40, c.R, c.G, c.B), 0));

            brush.GradientStops.Add(
                new GradientStop(
                    Color.FromArgb(15, c.R, c.G, c.B), .50));

            brush.GradientStops.Add(
                new GradientStop(
                    Color.FromArgb(35, c.R, c.G, c.B), 1));

            brush.Freeze();

            return brush;
        }

        public Rect Deflate(
            Rect rect,
            double amount)
        {
            return new Rect(
                rect.Left + amount,
                rect.Top + amount,
                rect.Width - amount * 2,
                rect.Height - amount * 2);
        }

        public Point Center(Rect rect)
        {
            return new Point(
                rect.Left + rect.Width / 2,
                rect.Top + rect.Height / 2);
        }

        public Size Size(Rect rect)
        {
            return new Size(
                rect.Width,
                rect.Height);
        }
        public static HudPanel CreateRegisteredPanel(
            string id)
        {
            if (!_definitions.TryGetValue(
                    id,
                    out var definition))
            {
                throw new KeyNotFoundException(
                    $"HUD Panel '{id}' was not registered.");
            }


            var panel =
                CreatePanel(definition);


            _activePanels[id] =
                panel;


            return panel;
        }



        /// <summary>
        /// Returns an existing active panel.
        /// </summary>
        public HudPanel GetPanel(
            string id)
        {
            if (_activePanels.TryGetValue(
                    id,
                    out var panel))
            {
                return panel;
            }


            return null;
        }



        /// <summary>
        /// Removes a panel from active memory.
        /// </summary>
        public void RemovePanel(
            string id)
        {
            if (_activePanels.ContainsKey(id))
            {
                _activePanels.Remove(id);
            }
        }



        /// <summary>
        /// Clears all active HUD panels.
        /// </summary>
        public void ClearPanels()
        {
            _activePanels.Clear();
        }



        /// <summary>
        /// Checks whether a panel definition exists.
        /// </summary>
        public bool HasDefinition(
            string id)
        {
            return _definitions.ContainsKey(id);
        }



        /// <summary>
        /// Checks whether a panel is currently active.
        /// </summary>
        public bool IsActive(
            string id)
        {
            return _activePanels.ContainsKey(id);
        }

        public void LoadDefinitions(
            HudLayoutConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(
                    nameof(configuration));


            foreach (var panel in configuration.Panels)
            {
                RegisterPanel(panel);
            }
        }
        private static void ApplyPosition(
                HudPanel panel,
                HudPanelDefinition definition)
        {

            Canvas.SetLeft(
                panel,
                definition.Position.X);


            Canvas.SetTop(
                panel,
                definition.Position.Y);



            Panel.SetZIndex(
                panel,
                (int)definition.Position.ZIndex);



            panel.Visibility =
                definition.IsVisible
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
        /// <summary>
        /// Creates a fully configured HUD panel.
        /// </summary>
        public static HudPanel CreatePanel(HudPanelDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));


            var panel = new HudPanel
            {
                Width = definition.Width,
                Height = definition.Height,

                HorizontalAlignment = definition.HorizontalAlignment,
                VerticalAlignment = definition.VerticalAlignment,

                Margin = definition.Margin,

                Opacity = definition.Opacity
            };

            panel.PanelId =
                    definition.Id;

            ConfigurePanelStyle(panel, definition);

            ApplyPosition(panel, definition);

            AddHeader(panel, definition);

            //AddDecorations(panel, definition);


            return panel;
        }


        /// <summary>
        /// Applies the visual HUD styling.
        /// </summary>
        private static void ConfigurePanelStyle(
            HudPanel panel,
            HudPanelDefinition definition)
        {
            panel.Background =
                definition.Background ??
                new SolidColorBrush(Color.FromArgb(
                    180,
                    10,
                    15,
                    25));


            panel.BorderBrush =
                definition.BorderBrush ??
                new SolidColorBrush(
                    Color.FromRgb(
                        0,
                        255,
                        255));


            panel.BorderThickness =
                definition.BorderThickness ??
                new Thickness(1);


            panel.CornerRadius =
                definition.CornerRadius ??
                new CornerRadius(8);
        }



        /// <summary>
        /// Creates the title/header section.
        /// </summary>
        private static void AddHeader(
            HudPanel panel,
            HudPanelDefinition definition)
        {
            if (!definition.ShowHeader)
                return;


            var header = new HudPanelHeader
            {
                Title = definition.Title,

                Height = definition.HeaderHeight,

                HorizontalAlignment =
                    HorizontalAlignment.Stretch
            };


            panel.Header = header;
        }


        /// <summary>
        /// Registers a reusable HUD panel definition.
        /// </summary>
        public static void RegisterPanel(
            HudPanelDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));


            if (string.IsNullOrWhiteSpace(definition.Id))
                throw new ArgumentException(
                    "HUD Panel definition requires an Id");


            _definitions[definition.Id] =
                definition;
        }
        public HudPanel CreateRegisteredPanelByID(
           string id)
        {
            if (!_definitions.TryGetValue(
                    id,
                    out var definition))
            {
                throw new KeyNotFoundException(
                    $"HUD Panel '{id}' was not registered.");
            }


            var panel =
                CreatePanel(definition);


            _activePanels[id] =
                panel;


            return panel;
        }
    }
}

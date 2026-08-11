using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace UI.Controls.HUD.Models
{
    public class HudPanelHeader : Border
    {
        private readonly TextBlock _titleText;

        public HudPanelHeader()
        {
            Height = 35;

            Background =
                new SolidColorBrush(
                    Color.FromArgb(
                        100,
                        0,
                        255,
                        255));


            BorderThickness =
                new Thickness(0, 0, 0, 1);


            BorderBrush =
                new SolidColorBrush(
                    Color.FromRgb(
                        0,
                        255,
                        255));


            _titleText = new TextBlock
            {
                VerticalAlignment =
                    VerticalAlignment.Center,

                HorizontalAlignment =
                    HorizontalAlignment.Left,

                Margin =
                    new Thickness(15, 0, 0, 0),

                FontSize = 14,

                FontWeight =
                    FontWeights.Bold,

                Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(
                            0,
                            255,
                            255))
            };


            Child = _titleText;
        }



        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(HudPanelHeader),
                new PropertyMetadata(
                    string.Empty,
                    OnTitleChanged));



        public string Title
        {
            get =>
                (string)GetValue(TitleProperty);

            set =>
                SetValue(
                    TitleProperty,
                    value);
        }



        private static void OnTitleChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is HudPanelHeader header)
            {
                header._titleText.Text =
                    e.NewValue?.ToString() ?? string.Empty;
            }
        }
    }
}

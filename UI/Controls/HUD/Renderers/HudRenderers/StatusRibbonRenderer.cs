using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Factories;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Managers;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Renderers.HudRenderers
{
    public sealed class StatusRibbonRenderer : IHudRenderer
    {
        private readonly Canvas _canvas;

        private readonly HudTheme _theme;


        private Rectangle? _background;

        private TextBlock? _titleText;

        private TextBlock? _statusText;

        private double _width;



        public StatusRibbonRenderer(
            HudLayerManager layers,
            HudTheme theme)
        {
            _canvas =
                layers.GetLayer(HudLayer.Overlay);

            _theme = theme;
        }



        //--------------------------------------------------
        // Initialize
        //--------------------------------------------------

        public void Initialize(
            double width,
            double height)
        {
            _width = width;

            _canvas.Children.Clear();

            CreateRibbon();
        }



        //--------------------------------------------------
        // Create Ribbon
        //--------------------------------------------------

        private void CreateRibbon()
        {
            //--------------------------------------------------
            // Background
            //--------------------------------------------------
            
            _background =
                HudShapeFactory.Rectangle(
                    new Point(40, 20),
                    _width - 80,
                    _theme.StatusRibbonHeight);
            

            Canvas.SetLeft(
                _background,
                40);


            Canvas.SetTop(
                _background,
                20);


            _canvas.Children.Add(
                _background);



            //--------------------------------------------------
            // Jarvis Title
            //--------------------------------------------------

            _titleText =
                CreateText(
                    "J.A.R.V.I.S.",
                    _theme.StatusFontSize);


            Canvas.SetLeft(
                _titleText,
                _theme.StatusMargin);


            Canvas.SetTop(
                _titleText,
                28);


            _canvas.Children.Add(
                _titleText);



            //--------------------------------------------------
            // Status
            //--------------------------------------------------

            _statusText =
                CreateText(
                    "ONLINE",
                    _theme.StatusFontSize);


            Canvas.SetLeft(
                _statusText,
                _width - 170);


            Canvas.SetTop(
                _statusText,
                28);


            _canvas.Children.Add(
                _statusText);
        }



        //--------------------------------------------------
        // Text Factory
        //--------------------------------------------------

        private TextBlock CreateText(
            string text,
            double size)
        {
            return new TextBlock
            {
                Text = text,

                Foreground =
                    HudBrushFactory.Solid(
                        _theme.PrimaryColor),

                FontSize = size,

                FontFamily =
                    new FontFamily(
                        "Segoe UI"),

                FontWeight =
                    FontWeights.Light,

                Opacity =
                    _theme.StatusOpacity
            };
        }



        //--------------------------------------------------
        // Update
        //--------------------------------------------------

        public void Update(
            double deltaTime)
        {
            if (_statusText == null)
                return;


            //
            // Placeholder
            // Later this will come from Jarvis state
            //

            _statusText.Text =
                "ONLINE";
        }



        //--------------------------------------------------
        // Resize
        //--------------------------------------------------

        public void Resize(
            double width,
            double height)
        {
            Initialize(
                width,
                height);
        }



        //--------------------------------------------------
        // Dispose
        //--------------------------------------------------

        public void Dispose()
        {
            _canvas.Children.Clear();
        }
    }
}

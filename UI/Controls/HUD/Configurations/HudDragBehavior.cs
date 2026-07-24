using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UI.Controls.HUD.Abstract;
using UI.Controls.HUD.Managers;

namespace UI.Controls.HUD.Configurations
{
    public class HudDragBehavior
    {

        private Point _start;

        private bool _dragging;

        private readonly HudEditorManager _editor;

        public HudDragBehavior(
            HudEditorManager editor)
        {
            _editor = editor;
        }

        public void Attach(
            HudPanel panel)
        {

            panel.MouseLeftButtonDown +=
                (s, e) =>
                {
                    _dragging = true;

                    _start =
                        e.GetPosition(
                            null);


                    panel.CaptureMouse();
                };



            panel.MouseMove +=
                (s, e) =>
                {

                    if (!_dragging)
                        return;


                    var position =
                        e.GetPosition(
                            null);


                    Canvas.SetLeft(
                        panel,
                        Canvas.GetLeft(panel)
                        + (position.X - _start.X));


                    Canvas.SetTop(
                        panel,
                        Canvas.GetTop(panel)
                        + (position.Y - _start.Y));

                    _editor.UpdatePosition(
                        panel.PanelId,
                        Canvas.GetLeft(panel)
                        + (position.X - _start.X),
                        Canvas.GetTop(panel)
                        + (position.Y - _start.Y));

                    _start = position;

                };



            panel.MouseLeftButtonUp +=
                (s, e) =>
                {

                    _dragging = false;

                    panel.ReleaseMouseCapture();

                };

        }

    }
}

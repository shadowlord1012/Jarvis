using System;
using System.Collections.Generic;
using System.Text;

namespace UI.Controls.HUD.Enums
{
    /// <summary>
    /// Identifies one of the four HUD corner orientations.
    /// Used by factories and renderers when creating
    /// decorative corner geometry.
    /// </summary>
    public enum CornerOrientation
    {
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft,

        Left,
        Top,
        Right,
        Bottom,

        Center
    }
}

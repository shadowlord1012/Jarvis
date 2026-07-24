using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Renderers.ReactorRenderers;

namespace UI.Controls.HUD.Models
{
    /// <summary>
    /// Represents a complete HUD visual theme.
    /// </summary>
    public sealed class HudTheme
    {
        public string Name { get; init; } = "Default";

        #region Brushes

        public Brush PrimaryBrush { get; init; } = Brushes.Cyan;

        public Brush SecondaryBrush { get; init; } = Brushes.DeepSkyBlue;

        public Brush AccentBrush { get; init; } = Brushes.Lime;

        public Brush WarningBrush { get; init; } = Brushes.Orange;

        public Brush DangerBrush { get; init; } = Brushes.Red;

        public Brush SuccessBrush { get; init; } = Brushes.LimeGreen;

        public Brush TextBrush { get; init; } = Brushes.White;

        //Add the code after
        public Brush ReactorCoreBrush { get; init; }
        public Brush ReactorRingBrush { get; init; }
        public Brush ReactorEnergyBrush { get; init; }
        public Brush ReactorWarningBrush { get; init; }
        public Brush ReactorDangerBrush { get; init; }
        public Brush ReactorHighlightBrush { get; init; }


        public Brush BackgroundBrush { get; init; }
            = new SolidColorBrush(Color.FromArgb(20, 0, 255, 255));

        public Brush GridBrush { get; init; }
            = new SolidColorBrush(Color.FromArgb(30, 0, 255, 255));

        public Brush PanelBrush { get; init; }
            = new SolidColorBrush(Color.FromArgb(15, 0, 255, 255));

        public Brush BorderBrush { get; init; } = Brushes.Cyan;

        #endregion

        #region Typography

        public FontFamily FontFamily { get; init; }
            = new FontFamily("Segoe UI");

        public double FontSizeSmall { get; init; } = 11;

        public double FontSizeNormal { get; init; } = 14;

        public double FontSizeLarge { get; init; } = 18;

        #endregion

        #region Geometry

        public double BorderThickness { get; init; } = 2;

        public double CornerLength { get; init; } = 18;

        public double PanelCornerRadius { get; init; } = 4;

        public double GridSpacing { get; init; } = 40;

        public Point DefaultScreenSize { get; init; } = new Point(800, 600);

        #endregion

        #region Effects

        public double GlowRadius { get; init; } = 12;

        public double GlowOpacity { get; init; } = 0.75;

        public double ScanlineOpacity { get; init; } = 0.12;

        public double GridOpacity { get; init; } = 0.18;

        #endregion

        #region Animation

        public double PulseSpeed { get; init; } = 1.2;

        public double ScanSpeed { get; init; } = 1.5;

        public double FadeSpeed { get; init; } = 0.3;

        #endregion

        public void Freeze()
        {
            FreezeBrush(PrimaryBrush);
            FreezeBrush(SecondaryBrush);
            FreezeBrush(AccentBrush);
            FreezeBrush(WarningBrush);
            FreezeBrush(DangerBrush);
            FreezeBrush(SuccessBrush);
            FreezeBrush(TextBrush);
            FreezeBrush(BackgroundBrush);
            FreezeBrush(GridBrush);
            FreezeBrush(PanelBrush);
            FreezeBrush(BorderBrush);
        }

        private static void FreezeBrush(Brush brush)
        {
            if (brush is Freezable freezable && freezable.CanFreeze)
            {
                freezable.Freeze();
            }
        }

        #region Reactor

        // Geometry

        public double CoreOuterTriangleRadius { get; init; } = 150;

        public double CoreInnerTriangleRadius { get; init; } = 90;

        public double CoreHexagonRadius { get; init; } = 65;

        public double CoreEnergyRingRadius { get; init; } = 42;

        public double CoreOrbRadius { get; init; } = 18;

        public double ReactorRadius { get; init; } = 220;

        public double ReactorRingSpacing { get; init; } = 18;

        public double ReactorSegmentGap { get; init; } = 3;


        // Lines

        public double CoreLineThickness { get; init; } = 10;

        public double ReactorRingThickness { get; init; } = 2;

        public double ReactorArcThickness { get; init; } = 3;


        // Animation

        public double CoreRotationSpeed { get; init; } = 20;

        public double CorePulseSpeed { get; init; } = 2.5;

        public double ReactorSweepSpeed { get; init; } = 20;

        public double ReactorScanSpeed { get; init; } = 9;


        // Opacity

        public double CoreOpacity { get; init; } = .9;

        public double ReactorBackgroundOpacity { get; init; } = .20;

        public double ReactorForegroundOpacity { get; init; } = .85;


        // Glow

        public double CoreGlowRadius { get; init; } = 18;

        public double CoreGlowOpacity { get; init; } = .75;

        #endregion

        #region Energy Beams

        public double BeamThickness { get; init; } = 2.0;

        public double BeamOpacity { get; init; } = 0.85;

        /// <summary>
        /// Speed multiplier for the beam pulse animation.
        /// </summary>
        public double BeamPulseSpeed { get; init; } = 2.5;

        /// <summary>
        /// Controls how much the beam thickness expands while pulsing.
        /// </summary>
        public double BeamPulseStrength { get; init; } = 0.25;

        #endregion

        #region Orbit

        public double OrbitOpacity { get; init; } = 0.90;

        public double OrbitGlowOpacity { get; init; } = 0.45;

        public bool ShowOrbitLabels { get; init; } = true;

        public double OrbitLabelFontSize { get; init; } = 10;

        public double OrbitLabelOffsetX { get; init; } = 10;

        public double OrbitLabelOffsetY { get; init; } = -8;

        #endregion

        #region Outer Ring

        public double ReactorOuterRotationSpeed { get; init; } = 18.0;

        public int ReactorSegmentCount { get; init; } = 24;

        public double ReactorGapAngle { get; init; } = 2.0;

        public double ReactorOuterRadius { get; init; } = 215;

        public double ReactorSegmentThickness { get; init; } = 2.5;

        #endregion

        #region Reactor Rings

        public IReadOnlyList<ReactorRingDefinition>
            ReactorRings
        {
            get;
            init;
        }
        =
        new List<ReactorRingDefinition>();

        #endregion

        #region Tick Ring

        public double ReactorTickRadius { get; init; } = 250;

        public double ReactorTickLength { get; init; } = 10;

        public int ReactorTickCount { get; init; } = 72;

        public double ReactorTickThickness { get; init; } = 1.5;

        public double ReactorTickOpacity { get; init; } = 0.85;

        public double ReactorTickRotationSpeed { get; init; } = 15.0;

        #endregion

        #region Glow

        public double ReactorGlowRadius { get; init; } = 230;

        public double ReactorCoreGlowRadius { get; init; } = 65;

        #endregion

        #region Background

        public Color BackgroundGradientStart { get; init; }
            = Color.FromRgb(3, 8, 18);

        public Color BackgroundGradientMiddle { get; init; }
            = Color.FromRgb(0, 15, 35);

        public Color BackgroundGradientEnd { get; init; }
            = Color.FromRgb(2, 5, 12);

        public double BackgroundAnimationSpeed { get; init; }
            = 0.03;

        public double BackgroundAnimationAmplitude { get; init; }
            = 0.05;

        #endregion

        #region Grid

        public double GridThickness { get; init; } = 1;

        public double GridScrollSpeed { get; init; } = 8;

        #endregion

        #region Frame

        public double FrameMargin { get; init; } = 40;

        public double FrameThickness { get; init; } = 1.5;

        public double FrameOpacity { get; init; } = 0.35;

        public double FrameCornerLength { get; init; } = 28;

        #endregion

        #region Scan

        public double ScanThickness { get; init; } = 4;

        public double ScanOpacity { get; init; } = 0.45;

        public IReadOnlyList<ScanDefinition> ScanEffects { get; init; }
            = new List<ScanDefinition>
        {
            new()
            {
                Type = ScanType.Horizontal,
                Brush = Brushes.Cyan,
                Speed = 140,
                Thickness = 4,
                Opacity = .45,
                Glow = true
            }
        };

        #endregion

        #region Old COde that is needed
        public Color PrimaryColor { get; set; }
            = Color.FromRgb(0, 255, 255);

        public Color BackgroundDark { get; set; }
            = Color.FromRgb(3, 8, 18);
        public double GlowSize { get; set; }
            = 700;

        //--------------------------------------------------
        // Status Ribbon
        //--------------------------------------------------

        public double StatusRibbonHeight { get; set; }
            = 45;


        public double StatusMargin { get; set; }
            = 25;


        public double StatusFontSize { get; set; }
            = 22;


        public double StatusOpacity { get; set; }
            = .85;

        public double ReactorRotationSpeed { get; set; } = 20;

        public double ReactorSegmentRotationSpeed { get; set; } = -15;
        #endregion

        #region Old Code
        /*
        //--------------------------------------------------
        // Colors
        //--------------------------------------------------

        public Color PrimaryColor { get; set; }
            = Color.FromRgb(0, 255, 255);


        public Color SecondaryColor { get; set; }
            = Color.FromRgb(40, 170, 255);


        


        public Color BackgroundLight { get; set; }
            = Color.FromRgb(0, 15, 35);


        //--------------------------------------------------
        // Grid
        //--------------------------------------------------

        public double GridThickness { get; set; }
            = 1;


        //--------------------------------------------------
        // Glow
        //--------------------------------------------------

        



        public double GlowPulseAmount { get; set; }
            = .03;


        public double GlowPulseSpeed { get; set; }
            = 2;


        //--------------------------------------------------
        // Scan
        //--------------------------------------------------

        public double ScanThickness { get; set; }
            = 3;


        public double ScanOpacity { get; set; }
            = .45;


        //--------------------------------------------------
        // Reactor
        //--------------------------------------------------

        public double ReactorRadius { get; set; }
            = 200;


        public double ReactorRingThickness { get; set; }
            = 3;


        public double ReactorGlowStrength { get; set; }
            = .15;


        //--------------------------------------------------
        // Animation
        //--------------------------------------------------

        public double AnimationSpeed { get; set; }
            = 1.0;

        //--------------------------------------------------
        // Corner Accents
        //--------------------------------------------------


        public double CornerThickness { get; set; }
            = 2;


        public double CornerOpacity { get; set; }
            = .65;

        
        //--------------------------------------------------
        // Dashboard Layout
        //--------------------------------------------------

        public double OuterMargin { get; set; } = 24;

        public double PanelSpacing { get; set; } = 18;

        public double LeftColumnWidth { get; set; } = 420;

        public double RightColumnWidth { get; set; } = 420;

        public double TopRibbonHeight { get; set; } = 60;

        public double BottomConsoleHeight { get; set; } = 150;

        public double ReactorDiameter { get; set; } = 520;

        //--------------------------------------------------
        // Reactor Rings
        //--------------------------------------------------

        public double ReactorOuterRadius { get; set; } = 280;

        public double ReactorInnerRadius { get; set; } = 235;

        public double ReactorSegmentThickness { get; set; } = 5;

        public int ReactorSegmentCount { get; set; } = 24;

        public double ReactorGapAngle { get; set; } = 6;


        //--------------------------------------------------
        // Reactor Tick Ring
        //--------------------------------------------------

        public int ReactorTickCount { get; set; } = 180;

        public double ReactorTickRadius { get; set; } = 240;

        public double ReactorTickLength { get; set; } = 10;

        public double ReactorTickThickness { get; set; } = 1.25;

        public double ReactorTickOpacity { get; set; } = 0.75;

        public double ReactorTickRotationSpeed { get; set; } = 8;

        //--------------------------------------------------
        // Reactor Segments
        //--------------------------------------------------
        public double ReactorSegmentRadius { get; set; } = 205;

        public double ReactorSegmentSweep { get; set; } = 12;

        public double ReactorSegmentGap { get; set; } = 8;

        public double ReactorSegmentOpacity { get; set; } = .90;

        public IReadOnlyList<ReactorRingDefinition> ReactorRings { get; } =
[
    new()
    {
        Radius = 280,
        SegmentCount = 24,
        SweepAngle = 10,
        GapAngle = 5,
        Thickness = 5,
        RotationSpeed = 12,
        Opacity = .85,
        Clockwise = true
    },

    new()
    {
        Radius = 240,
        SegmentCount = 18,
        SweepAngle = 14,
        GapAngle = 8,
        Thickness = 4,
        RotationSpeed = -20,
        Opacity = .90,
        Clockwise = false
    },

    new()
    {
        Radius = 205,
        SegmentCount = 12,
        SweepAngle = 18,
        GapAngle = 10,
        Thickness = 3,
        RotationSpeed = 8,
        Opacity = .65,
        Clockwise = true
    }
];
        //--------------------------------------------------
        // Reactor Core
        //--------------------------------------------------

        public double CoreOuterTriangleRadius { get; set; } = 72;

        public double CoreInnerTriangleRadius { get; set; } = 48;

        public double CoreHexagonRadius { get; set; } = 34;

        public double CoreEnergyRingRadius { get; set; } = 18;

        public double CoreOrbRadius { get; set; } = 10;

        public double CoreRotationSpeed { get; set; } = 18;

        public double CorePulseSpeed { get; set; } = 2.5;

        public double CoreLineThickness { get; set; } = 2;

        public double CoreOpacity { get; set; } = .95;

        //--------------------------------------------------
        // Orbit Nodes
        //--------------------------------------------------

        public double OrbitNodeRadius { get; set; } = 7;

        public double OrbitLineThickness { get; set; } = 1.5;

        public double OrbitGlowRadius { get; set; } = 16;

        public double OrbitOpacity { get; set; } = .90;

        public bool ShowOrbitLabels { get; set; } = false;

        //--------------------------------------------------
        // Energy Beams
        //--------------------------------------------------

        public double BeamThickness { get; set; } = 1.5;

        public double BeamOpacity { get; set; } = 0.65;

        public double BeamPulseStrength { get; set; } = 0.35;

        public bool BeamGlowEnabled { get; set; } = true;

        //--------------------------------------------------
        // Glow
        //--------------------------------------------------

        public double ReactorGlowRadius { get; set; } = 340;

        public double ReactorCoreGlowRadius { get; set; } = 95;

        public double PanelGlowRadius { get; set; } = 22;

        public double GlowStrength { get; set; } = 1.0;

        //--------------------------------------------------
        // Global Effects
        //--------------------------------------------------

        public double SlowPulseSpeed { get; set; } = 0.75;

        public double FastPulseSpeed { get; set; } = 6.0;

        public double GlowSpeed { get; set; } = 2.5;

        public double ShimmerSpeed { get; set; } = 9.0;

        public double ScanRotationSpeed { get; set; } = 140;

        public double WarningFlashSpeed { get; set; } = 12.0;
        */
        #endregion

    }
}

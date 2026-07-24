using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using UI.Controls.HUD.Enums;
using UI.Controls.HUD.Interfaces;
using UI.Controls.HUD.Managers;
using UI.Controls.HUD.Models;

namespace UI.Controls.HUD.Renderers.HudRenderers
{

    /// <summary>
    /// Master renderer responsible for coordinating all HUD renderers.
    /// </summary>
    public sealed class HudRenderer : IDisposable
    {
        private readonly HudLayerManager _layerManager;

        private readonly List<IHudRenderer> _renderers =
            new();

        public List<IHudRenderer> Renderers => _renderers;

        private readonly DispatcherTimer _timer;

        private DateTime _lastFrame;

        private double _width;

        private double _height;

        //---------------------------------------------------------

        public HudRenderer(
            HudLayerManager layerManager, HudTheme theme)
        {
            _layerManager = layerManager;

            _timer = new DispatcherTimer(
                DispatcherPriority.Render);

            _timer.Interval =
                TimeSpan.FromMilliseconds(16.666);

            _timer.Tick += OnRenderFrame;
        }

        //---------------------------------------------------------

        public void RegisterRenderer(
            IHudRenderer renderer)
        {
            _renderers.Add(renderer);
        }

        //---------------------------------------------------------

        public void Initialize(
            double width,
            double height)
        {
            _width = width;
            _height = height;

            foreach (IHudRenderer renderer in _renderers)
            {
                renderer.Initialize(width, height);
            }

            _lastFrame = DateTime.Now;
        }

        //---------------------------------------------------------

        public void Start()
        {
            _lastFrame = DateTime.Now;

            _timer.Start();
        }

        //---------------------------------------------------------

        public void Stop()
        {
            _timer.Stop();
        }

        //---------------------------------------------------------

        private void OnRenderFrame(
            object? sender,
            EventArgs e)
        {
            DateTime now = DateTime.Now;

            double deltaTime =
                (now - _lastFrame).TotalSeconds;

            _lastFrame = now;

            foreach (IHudRenderer renderer in _renderers)
            {
                renderer.Update(deltaTime);
            }
        }

        //---------------------------------------------------------

        public void Resize(
            double width,
            double height)
        {
            _width = width;
            _height = height;

            foreach (IHudRenderer renderer in _renderers)
            {
                renderer.Resize(width, height);
            }
        }

        //---------------------------------------------------------

        public void Rebuild()
        {
            foreach (IHudRenderer renderer in _renderers)
            {
                renderer.Initialize(
                    _width,
                    _height);
            }
        }

        //---------------------------------------------------------

        public void Dispose()
        {
            Stop();

            _timer.Tick -= OnRenderFrame;

            foreach (IHudRenderer renderer in _renderers)
            {
                renderer.Dispose();
            }

            _renderers.Clear();
        }
    }
    /*
    public sealed class HudRenderer : IDisposable
    {
        private readonly HudLayerManager _layerManager;
        public HudTheme Theme { get; }

        private readonly List<IHudRenderer> _renderers = new();

        private readonly ReactorManager _reactor;

        private readonly HudEffectManager _effects;

        private readonly HudPanelManager _panelManager;
        public ReactorManager GetReactorManager { get { return _reactor; } }
        public HudEffectManager GetHudEffectManager { get { return _effects; } }
        public bool IsRunning { get; private set; }

        public bool IsPaused { get; private set; }

        public HudRenderer(
                    HudLayerManager layerManager,
                    HudTheme theme, 
                    HudPanelManager panelManager)
        {
            _layerManager = layerManager;
            _reactor = new ReactorManager(theme);
            Theme = theme;
            _effects = new HudEffectManager(_reactor, theme);
            _panelManager = panelManager;
        }

        //--------------------------------------------------
        // Register Renderer
        //--------------------------------------------------

        public void AddRenderer(IHudRenderer renderer)
        {
            if (renderer == null)
                throw new ArgumentNullException(nameof(renderer));

            _renderers.Add(renderer);
        }

        //--------------------------------------------------
        // Initialize
        //--------------------------------------------------

        public void Initialize()
        {
            double width = _layerManager.GetLayer(HudLayer.Background).ActualWidth;
            double height = _layerManager.GetLayer(HudLayer.Background).ActualHeight;

            _reactor.State.Center = new Point(
                width / 2,
                height / 2);


            foreach (var renderer in _renderers)
            {
                renderer.Initialize(width, height);
            }

            IsRunning = true;
            IsPaused = false;
        }

        //--------------------------------------------------
        // Update
        //--------------------------------------------------

        public void Update(double deltaTime)
        {
            if (!IsRunning)
                return;

            if (IsPaused)
                return;

            _reactor.Update(deltaTime);

            _effects.Update(deltaTime);

            foreach (IHudRenderer renderer in _renderers)
            {
                renderer.Update(deltaTime);
            }
        }

        //--------------------------------------------------
        // Resize
        //--------------------------------------------------

        public void Resize()
        {
            double width = _layerManager.GetLayer(HudLayer.Background).ActualWidth;
            double height = _layerManager.GetLayer(HudLayer.Background).ActualHeight;

            _reactor.Resize(width, height);

            foreach (var renderer in _renderers)
            {
                renderer.Resize(width, height);
            }
        }

        //--------------------------------------------------
        // Pause
        //--------------------------------------------------

        public void Pause()
        {
            IsPaused = true;
        }

        //--------------------------------------------------
        // Resume
        //--------------------------------------------------

        public void Resume()
        {
            IsPaused = false;
        }

        //--------------------------------------------------
        // Stop
        //--------------------------------------------------

        public void Stop()
        {
            IsRunning = false;
        }

        //--------------------------------------------------
        // Start Again
        //--------------------------------------------------

        public void Start()
        {
            IsRunning = true;
        }

        //--------------------------------------------------
        // Dispose
        //--------------------------------------------------

        public void Dispose()
        {
            Stop();

            _renderers.Clear();

            _layerManager.ClearAll();
        }

        //--------------------------------------------------
        // Async Task
        //--------------------------------------------------

        public async Task InitializeAsync()
        {
            await _panelManager.InitializeAsync();


            LoadPanels();


            StartRendering();
        }
        private void LoadPanels()
        {
            var reactor =
                _panelManager.GetPanel(
                    "ReactorCore");


            if (reactor != null)
            {
                AddVisual(
                    reactor);
            }



            var system =
                _panelManager.GetPanel(
                    "SystemStatus");


            if (system != null)
            {
                AddVisual(
                    system);
            }
        }
    }
    */
}

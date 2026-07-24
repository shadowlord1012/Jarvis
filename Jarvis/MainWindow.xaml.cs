using Loader;
using System.Windows;
using System.Windows.Input;
using UI.Controls.HUD;

namespace Jarvis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly StartupStatusService _startupStatus;
        private readonly HudCanvas _hudCanvas;

        public MainWindow(StartupStatusService startupStatus, HudCanvas hudCanvas)
        {
            InitializeComponent();

            _startupStatus = startupStatus;
            _hudCanvas = hudCanvas;

            DataContext = _startupStatus;

            Loaded += MainWindow_Loaded;

        }

        public StartupStatusService StartupStatusService { get; }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void DragRegion_MouseLeftButtonDown(
            object sender,
            MouseButtonEventArgs e)
        {
            DragMove();
        }
        private async void MainWindow_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            // Add HudCanvas to the container
            HudContainer.Children.Add(_hudCanvas);
        }
    }
}
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NP.Ava.Visuals.Controls;
using NP.Demos.LiveTradingDemo.ViewModels;

namespace NP.Demos.LiveTradingDemo
{
    public partial class MainWindow : CustomWindow
    {

        #region WindowPosition Styled Avalonia Property
        public PixelPoint WindowPosition
        {
            get { return GetValue(WindowPositionProperty); }
            set { SetValue(WindowPositionProperty, value); }
        }

        public static readonly StyledProperty<PixelPoint> WindowPositionProperty =
            AvaloniaProperty.Register<MainWindow, PixelPoint>
            (
                nameof(WindowPosition)
            );
        #endregion WindowPosition Styled Avalonia Property

        LiveTradesViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _viewModel = new LiveTradesViewModel();

            this.DataContext = _viewModel;
        }

        private void OnMyEvent(object? sender, RoutedEventArgs e)
        {

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void OnPressed()
        {

        }
    }
}

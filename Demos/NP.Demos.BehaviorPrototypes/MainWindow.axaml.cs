using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NP.Avalonia.Visuals.Controls;
using System;

namespace NP.Demos.BehaviorPrototypes
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


        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.AddHandler(Events.MyEvent, OnMyEvent);
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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NP.Avalonia.Visuals.Behaviors;
using NP.Avalonia.Visuals.Controls;
using NP.Avalonia.Visuals.ThemingAndL10N;

namespace NP.ControlsDemo
{
    public partial class MainWindow : Window
    {
        private ThemeLoader _themeLoader;

        AutoGrid _autoGrid;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }



        #region Angle Styled Avalonia Property
        public double Angle
        {
            get { return GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public static readonly StyledProperty<double> AngleProperty =
            AvaloniaProperty.Register<MainWindow, double>
            (
                nameof(Angle)
            );
        #endregion Angle Styled Avalonia Property

    }
}

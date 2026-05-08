using Avalonia;
using Avalonia.Controls;

using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NP.Ava.Visuals.Behaviors;
using NP.Ava.Visuals.Controls;
using NP.Ava.Visuals.ThemingAndL10N;
using NP.Utilities;

namespace NP.Demos.DragDropInPanel
{
    public partial class MainWindow : CustomWindow
    {
        ThemeLoader _lightDarkThemeLoader;
        ThemeLoader _accentThemeLoader;

        ReactiveVisualDesendantsBehavior _flattenVisualTreeBehavior;


        #region ThePoint Styled Avalonia Property
        public Point2D<double> ThePoint
        {
            get { return GetValue(ThePointProperty); }
            set { SetValue(ThePointProperty, value); }
        }

        public static readonly StyledProperty<Point2D<double>> ThePointProperty =
            AvaloniaProperty.Register<MainWindow, Point2D<double>>
            (
                nameof(ThePoint)
            );
        #endregion ThePoint Styled Avalonia Property


        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            //this.AttachDeveloperTools();
#endif

        }
    }
}

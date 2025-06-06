global using Point2D = NP.Utilities.Point2D<double>;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NP.Ava.Visuals.Behaviors;
using NP.Ava.Visuals.Controls;
using NP.Ava.Visuals.ThemingAndL10N;
using NP.Utilities;

namespace NP.ThemingPrototype
{
    public partial class MainWindow : CustomWindow
    {
        ThemeLoader _lightDarkThemeLoader;
        ThemeLoader _accentThemeLoader;

        ReactiveVisualDesendantsBehavior _flattenVisualTreeBehavior;


        #region ThePoint Styled Avalonia Property
        public Point ThePoint
        {
            get { return GetValue(ThePointProperty); }
            set { SetValue(ThePointProperty, value); }
        }

        public static readonly StyledProperty<Point> ThePointProperty =
            AvaloniaProperty.Register<MainWindow, Point>
            (
                nameof(ThePoint)
            );
        #endregion ThePoint Styled Avalonia Property


        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _lightDarkThemeLoader = 
                Application.Current.Resources.GetThemeLoader("LightDarkThemeLoader")!;

            _accentThemeLoader =
                Application.Current.Resources.GetThemeLoader("AccentThemeLoader")!;

            Button button = this.FindControl<Button>("ChangeThemeButton");

            _flattenVisualTreeBehavior = 
                new ReactiveVisualDesendantsBehavior(button);
            button.Click += Button_Click;
        }

        private void Button_Click(object? sender, RoutedEventArgs e)
        {
            //Popup p = comboBox.TheNameScope.Get<Popup>("PART_Popup");

            _flattenVisualTreeBehavior.DetachCollections();

            _flattenVisualTreeBehavior.AttachCollections();

            _lightDarkThemeLoader.SwitchTheme();

            if (_lightDarkThemeLoader.SelectedThemeId == "Light")
            {
                _accentThemeLoader.SelectedThemeId = "DarkBlue";
            }
            else
            {
                _accentThemeLoader.SelectedThemeId = "LightBlue";
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NP.Ava.Visuals.Controls;
using NP.Ava.Visuals.ThemingAndL10N;

namespace NP.LabeledControlDemo
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
    }
}

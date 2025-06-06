using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using NP.Ava.Visuals.Behaviors;
using NP.Ava.Visuals.Controls;
using NP.Ava.Visuals.ThemingAndL10N;
using System.Linq;

namespace NP.TradesGridDemo
{
    public partial class MainWindow : Window
    {
        private ThemeLoader _themeLoader;

        AutoGrid _autoGrid;

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}

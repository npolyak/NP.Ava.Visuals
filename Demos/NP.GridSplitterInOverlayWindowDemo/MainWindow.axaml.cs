using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace NP.GridSplitterInOverlayWindowDemo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object? sender, System.EventArgs e)
        {
            Environment.Exit(0);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

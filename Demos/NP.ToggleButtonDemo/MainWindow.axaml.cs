using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NP.DataGridFilteringDemo
{
    public partial class MainWindow : Window
    {
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

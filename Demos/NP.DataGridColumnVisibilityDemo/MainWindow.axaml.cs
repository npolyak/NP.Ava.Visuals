using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace NP.DataGridColumnVisibilityDemo
{
    public partial class MainWindow : Window
    {
        public People ThePeople { get; } = new People();

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

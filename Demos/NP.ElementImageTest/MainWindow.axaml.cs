using Avalonia.Controls;
using Avalonia.Interactivity;
using NP.Ava.Visuals;

namespace NP.ElementImageTest
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TakeElementImageButton.Click += TakeElementImageButton_Click;
        }

        private void TakeElementImageButton_Click(object? sender, RoutedEventArgs e)
        { 
            TheImage.Source = ButtonPanel.ControlToImage();
        }
    }
}

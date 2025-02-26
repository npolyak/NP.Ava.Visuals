using Avalonia;
using Avalonia.Controls;
using NP.TradesDemo.Models;

namespace NP.TradesDemo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif

    }
}

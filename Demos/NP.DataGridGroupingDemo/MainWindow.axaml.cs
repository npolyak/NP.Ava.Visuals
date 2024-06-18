using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Simple;
using Avalonia.VisualTree;
using System.Linq;

namespace NP.DataGridGroupingDemo
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
            DoButton.Click += DoButton_Click;
        }

        private void DoButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            DataGridRowsPresenter dataGridRowsPresenter = 
                TheDataGrid.GetVisualDescendants().OfType<DataGridRowsPresenter>().FirstOrDefault();

            dataGridRowsPresenter.Children.CollectionChanged -= Children_CollectionChanged;
            dataGridRowsPresenter.Children.CollectionChanged += Children_CollectionChanged;

            DataGridRow firstRow = dataGridRowsPresenter.Children.OfType<DataGridRow>().FirstOrDefault();   
        }

        private void Children_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            
        }
    }
}

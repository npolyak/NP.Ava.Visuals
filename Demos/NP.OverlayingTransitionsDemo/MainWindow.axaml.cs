using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using NP.Ava.Visuals.Behaviors;

namespace NP.OverlayingTransitionsDemo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //OverlayBehavior.SetIsOpen(GridContainingOverlayPanel, true);
        }

        public void SelectTopLeftQuarter()
        {
            OverlayBehavior.SetOverlayedControl(GridContainingOverlayPanel, OverlayedControl1);
            OverlayBehavior.SetIsOpen(GridContainingOverlayPanel, true);
        }

        public void SelectTopRightQuarter()
        {
            OverlayBehavior.SetOverlayedControl(GridContainingOverlayPanel, OverlayedControl2);
            OverlayBehavior.SetIsOpen(GridContainingOverlayPanel, true);

        }


        public void SelectBottomLeftQuarter()
        {
            OverlayBehavior.SetOverlayedControl(GridContainingOverlayPanel, OverlayedControl3);
            OverlayBehavior.SetIsOpen(GridContainingOverlayPanel, true);

        }

        public void SelectBottomRightQuarter()
        {
            OverlayBehavior.SetOverlayedControl(GridContainingOverlayPanel, OverlayedControl4);
            OverlayBehavior.SetIsOpen(GridContainingOverlayPanel, true);

        }
    }
}

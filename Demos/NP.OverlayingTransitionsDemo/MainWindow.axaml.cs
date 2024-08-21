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

        public void SetCenter()
        {
            OverlayBehavior.SetCurrentSide(GridContainingOverlayPanel, Utilities.Side2D.Center);
            OverlayBehavior.SetIsOpen(GridContainingOverlayPanel, true);
        }

        public void SetLeft()
        {
            OverlayBehavior.SetCurrentSide(GridContainingOverlayPanel, Utilities.Side2D.Left);
            OverlayBehavior.SetIsOpen(GridContainingOverlayPanel, true);
        }

        public void SetRight()
        {
            OverlayBehavior.SetCurrentSide(GridContainingOverlayPanel, Utilities.Side2D.Right);
            OverlayBehavior.SetIsOpen(GridContainingOverlayPanel, true);
        }

        public void SetTop()
        {
            OverlayBehavior.SetCurrentSide(GridContainingOverlayPanel, Utilities.Side2D.Top);
            OverlayBehavior.SetIsOpen(GridContainingOverlayPanel, true);
        }

        public void SetBottom()
        {
            OverlayBehavior.SetCurrentSide(GridContainingOverlayPanel, Utilities.Side2D.Bottom);
            OverlayBehavior.SetIsOpen(GridContainingOverlayPanel, true);
        }
    }
}

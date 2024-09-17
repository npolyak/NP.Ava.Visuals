global using Point2D = NP.Utilities.Point2D<double>;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using NP.Ava.Visuals.Behaviors;
using NP.Ava.Visuals.Controls;
using NP.Utilities;
using System;

namespace NP.Demos.CustomWindowSample
{
    public partial class MainWindow : CustomWindow
    {
        #region CurrentCapturedPointerPositionInScreen Styled Avalonia Property
        public Point2D CurrentCapturedPointerPositionInScreen
        {
            get { return GetValue(CurrentCapturedPointerPositionInScreenProperty); }
            set { SetValue(CurrentCapturedPointerPositionInScreenProperty, value); }
        }

        public static readonly StyledProperty<Point2D> CurrentCapturedPointerPositionInScreenProperty =
            AvaloniaProperty.Register<MainWindow, Point2D>
            (
                nameof(CurrentCapturedPointerPositionInScreen)
            );
        #endregion CurrentCapturedPointerPositionInScreen Styled Avalonia Property


        #region WindowPosition Styled Avalonia Property
        public PixelPoint WindowPosition
        {
            get { return GetValue(WindowPositionProperty); }
            set { SetValue(WindowPositionProperty, value); }
        }

        public static readonly StyledProperty<PixelPoint> WindowPositionProperty =
            AvaloniaProperty.Register<MainWindow, PixelPoint>
            (
                nameof(WindowPosition)
            );
        #endregion WindowPosition Styled Avalonia Property


        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            this.PointerPressed += MainWindow_PointerPressed;
        }

        private void MainWindow_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            CurrentScreenPointBehavior.Capture(HeaderControl, true, e);

            CurrentScreenPointBehavior.CurrentScreenPoint.Subscribe(OnScreenPositionChanged);
        }

        private void OnScreenPositionChanged(Point2D<double> d)
        {
            CurrentCapturedPointerPositionInScreen = d;
        }

        private void MainWindow_PositionChanged(object? sender, PixelPointEventArgs e)
        {
            //WindowPosition = Position;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NP.Avalonia.Visuals.Behaviors;
using NP.Avalonia.Visuals.Controls;
using NP.Avalonia.Visuals.WindowsOnly;
using System;
using System.Diagnostics;

namespace NP.Demos.BehaviorPrototypes
{
    public partial class MainWindow : CustomWindow
    {
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

        ImplantedWindowHostContainer _control;
        public MainWindow()
        {
            InitializeComponent();

            _control = new ImplantedWindowHostContainer();

            var path = @"C:\Program Files\Notepad++\notepad++.exe";

            //_control.ProcessExePath = path;

            _control.GetObservable(ProcessControllerBehavior.MainWindowHandleProperty).Subscribe(OnWindowHandleChanged);

            ProcessControllerBehavior.SetProcessExePath(_control, path);

            var grid = this.FindControl<Grid>("TheGrid");

            grid.Children.Add(_control);
        }

        private void OnWindowHandleChanged(IntPtr windowHandle)
        {
            _control.ImplantedWindowHandle = windowHandle;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

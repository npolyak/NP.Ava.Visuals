﻿// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.
//
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using NP.Ava.Visuals.Behaviors;
using NP.Utilities;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Avalonia.Controls.Templates;
using System.Reactive.Linq;

namespace NP.Ava.Visuals.Controls
{
    public class CustomWindow : Window
    {
        private const string ComponentName = nameof(CustomWindow);

        private (string ControlName, StandardCursorType CursorType, WindowEdge TheWindowEdge)[] ResizeCursorInfos =
        {
            ("Left", StandardCursorType.LeftSide, WindowEdge.West),
            ("Right", StandardCursorType.RightSide, WindowEdge.East),
            ("Top", StandardCursorType.TopSide, WindowEdge.North),
            ("Bottom", StandardCursorType.BottomSide, WindowEdge.South),
            ("TopLeft", StandardCursorType.TopLeftCorner, WindowEdge.NorthWest),
            ("TopRight", StandardCursorType.TopRightCorner, WindowEdge.NorthEast),
            ("BottomLeft", StandardCursorType.BottomLeftCorner, WindowEdge.SouthWest),
            ("BottomRight", StandardCursorType.BottomRightCorner, WindowEdge.SouthEast)
        };

        private Control? _headerControl = null;

        protected Control? HeaderControl => _headerControl;

        IDisposable _windowStateChangeDisposer;

        IDisposable _windowCustomFeatureDisposer;


        private ResizeBehavior _resizeBehavior;

        public CustomWindow()
        {
            (this as INotifyPropertyChanged).PropertyChanged += CustomWindow_PropertyChanged;

            _windowStateChangeDisposer =
                WindowStateProperty.Changed.Subscribe(OnWindowStateChanged);

            _windowCustomFeatureDisposer =
                HasCustomWindowFeaturesProperty.Changed.Subscribe(OnHasCustomFeaturesChanged);

            _resizeBehavior = new ResizeBehavior(this);

            var canMaximizeObservable =
                this.GetObservable(CanMaximizeProperty)
                .CombineLatest
                (
                    this.GetObservable(CanMaximizeFlagProperty),
                    (canMaximize, canMaximizeFlag) => canMaximize && canMaximizeFlag);

            this.Bind<bool>(CanReallyMaximizeProperty, canMaximizeObservable);

            var canRestoreObservable =
                this.GetObservable(CanRestoreProperty)
                .CombineLatest(this.GetObservable(CanRestoreFlagProperty),
                (canRestore, canRestoreFlag) => canRestore && canRestoreFlag);

            this.Bind<bool>(CanReallyRestoreProperty, canRestoreObservable);
        }

        public bool IsTemplateApplied { get; private set; }


        private void OnHasCustomFeaturesChanged(AvaloniaPropertyChangedEventArgs<bool> hasCustomFeaturesContainer)
        {
            if (!hasCustomFeaturesContainer.NewValue.Value)
            {
                SetIsHitVisibleOnResizeControls(false);
                this.IsCustomHeaderVisible = false;
                this.SystemDecorations = SystemDecorations.Full;
            }
            else
            {
                SetIsHitVisibleOnResizeControls(true);
                this.IsCustomHeaderVisible = true;
            }
        }

        private void OnWindowStateChanged(AvaloniaPropertyChangedEventArgs<WindowState> windowStateChange)
        {
            OnWindowStateChanged();
        }

        private void OnWindowStateChanged()
        {
            if (!IsTemplateApplied)
                return;

            if (this.WindowState == WindowState.Maximized)
            {
                SetIsHitVisibleOnResizeControls(false);
            }
            else if (HasCustomWindowFeatures)
            {
                var oldChromeHints = this.ExtendClientAreaChromeHints;
                this.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.NoChrome;
                SetIsHitVisibleOnResizeControls(true);

                this.ExtendClientAreaChromeHints = oldChromeHints;
            }
        }

        private void CustomWindow_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WindowState))
            {
                RaisePropertyChanged(CanMaximizeProperty, !CanMaximize, CanMaximize);
                RaisePropertyChanged(CanRestoreProperty, !CanRestore, CanRestore);
            }
        }

        private Control SetCursorGetControl(string name, StandardCursorType cursorType)
        {
            var ctl = this.GetSubControlByName(name);
            ctl.Cursor = new Cursor(cursorType);

            return ctl;
        }

        private void MaximizeOrRestore()
        {
            if (WindowState == WindowState.Maximized || WindowState == WindowState.FullScreen)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                Maximize();
            }
        }

        public static readonly DirectProperty<CustomWindow, bool> CanMaximizeProperty =
            AvaloniaProperty.RegisterDirect<CustomWindow, bool>
            (
                nameof(CanMaximize),
                o => o.CanMaximize);

        public bool CanMaximize => this.WindowState != WindowState.Maximized;

        public virtual void Maximize()
        {
            this.WindowState = WindowState.Maximized;
        }

        public static readonly DirectProperty<CustomWindow, bool> CanRestoreProperty =
            AvaloniaProperty.RegisterDirect<CustomWindow, bool>
            (
                nameof(CanRestore),
                o => o.CanRestore);

        public bool CanRestore =>
            this.WindowState == WindowState.Maximized || this.WindowState == WindowState.FullScreen;

        public void Restore()
        {
            this.WindowState = WindowState.Normal;
        }

        public virtual void Minimize()
        {
            this.WindowState = WindowState.Minimized;
        }

        void SetupSide(string name, StandardCursorType cursorType, WindowEdge edge)
        {
            var ctl = SetCursorGetControl(name, cursorType);

            EventHandler<PointerPressedEventArgs>? handler = null;

            handler = (i, e) =>
            {
                Logger.Log(LogKind.Info, ComponentName, $"Inside PointerPressed Handler - Cursor is {cursorType}.");

                Cursor? oldWindowCursor = this.Cursor;
                this.Cursor = new Cursor(cursorType);

                SetIsHitVisibleOnResizeControls(false);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    _resizeBehavior.StartDrag(edge, e);
                }
                else
                {
                    this.BeginResizeDrag(edge, e);
                    //PlatformImpl?.BeginResizeDrag(edge, e);
                }
                SetIsHitVisibleOnResizeControls(true);

                this.Cursor = oldWindowCursor;
            };

            ctl.PointerPressed += handler;
        }

        private void SetIsHitVisibleOnResizeControls(bool isHitVisible)
        {
            foreach (var resizeCursorInfo in ResizeCursorInfos)
            {
                var ctl = this.GetSubControlByName(resizeCursorInfo.ControlName);
                ctl.IsHitTestVisible = isHitVisible;
            }
        }

        protected override Type StyleKeyOverride => typeof(CustomWindow);
        //Type IStyleable.StyleKey => typeof(CustomWindow);

        protected PixelPoint StartPointerPosition;
        protected PixelPoint StartWindowPosition;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            foreach (var resizeCursorInfo in ResizeCursorInfos)
            {
                SetupSide(resizeCursorInfo.ControlName, resizeCursorInfo.CursorType, resizeCursorInfo.TheWindowEdge);
            }

            _headerControl = this.GetSubControlByName("PART_HeaderControl");

            _headerControl.PointerPressed += OnHeaderPointerPressed;

            _headerControl.DoubleTapped += OnHeaderDoubleTapped;

            IsTemplateApplied = true;
            OnWindowStateChanged();
        }

        private void OnHeaderDoubleTapped(object? sender, RoutedEventArgs e)
        {
            if ( 
                ((!CanResize || !CanReallyMaximize) && (this.WindowState == WindowState.Normal)) ||
                (!CanReallyRestore && this.WindowState != WindowState.Normal))
            {
                return;
            }

            MaximizeOrRestore();
            HeaderControl.PointerMoved -= OnPointerMoved;
        }

        private void OnHeaderPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (!_headerControl.IsLeftMousePressed(e))
                return;

            if (DragOnBeginMove)
            {
                SetDragOnMovePointer(e);
                //CurrentScreenPointBehavior.Capture(_headerControl, !DragOnBeginMove, e);
                BeginMoveDrag(e);
            }
            else
            {
                SetDragWindowOnMovePointer(e);
            }
        }

        #region PointerShift Styled Avalonia Property
        public PixelPoint PointerShift
        {
            get { return GetValue(PointerShiftProperty); }
            set { SetValue(PointerShiftProperty, value); }
        }

        public static readonly StyledProperty<PixelPoint> PointerShiftProperty =
            AvaloniaProperty.Register<CustomWindow, PixelPoint>
            (
                nameof(PointerShift)
            );
        #endregion PointerShift Styled Avalonia Property

        bool _startMoving = false;
        public virtual void SetDragWindowOnMovePointer(PointerEventArgs e)
        {
            if (!e.GetCurrentPoint(_headerControl).Properties.IsLeftButtonPressed)
            {
                return;
            }


            _startMoving = false;
            StartPointerPosition = GetCurrentPointInScreen(e);
            StartWindowPosition = this.Position;
            PointerShift = new PixelPoint();
            SetDragOnMovePointer(e);
        }

        protected virtual void SetDragOnMovePointer(PointerEventArgs e)
        {
            if (HeaderControl != null)
            {
                if (!DragOnBeginMove)
                {
                    HeaderControl.PointerMoved -= OnPointerMoved;
                    HeaderControl.PointerMoved += OnPointerMoved;
                }

                HeaderControl.PointerReleased -= OnPointerReleased;
                HeaderControl.PointerReleased += OnPointerReleased; ;
            }
        }

        public PixelPoint GetCurrentPointInScreen(PointerEventArgs e)
        {
            return _headerControl.PointToScreen(e.GetPosition(_headerControl));
        }

        protected virtual void OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            _headerControl.PointerMoved -= OnPointerMoved;
            _headerControl.PointerReleased -= OnPointerReleased;

            if (!DragOnBeginMove && this.WindowState == WindowState.Normal)
            {
                UpdatePosition(e);
            }
            _startMoving = false;
        }

        protected virtual void OnPointerMoved(object sender, PointerEventArgs e)
        {
            UpdatePosition(e);
        }

        const double DefaultMinDistance = 5;

        private double MinDistance { get; set; } = DefaultMinDistance;

        protected void MinDistanceToZero()
        {
            MinDistance = 0;
        }

        protected void MinDistanceToDefault()
        {
            MinDistance = DefaultMinDistance;
        }

#if DEBUG
        #region CurrentPointerPositionInScreen Styled Avalonia Property
        public Point2D CurrentPointerPositionInScreen
        {
            get { return GetValue(CurrentPointerPositionInScreenProperty); }
            set { SetValue(CurrentPointerPositionInScreenProperty, value); }
        }

        public static readonly StyledProperty<Point2D> CurrentPointerPositionInScreenProperty =
            AvaloniaProperty.Register<CustomWindow, Point2D>
            (
                nameof(CurrentPointerPositionInScreen)
            );
        #endregion CurrentPointerPositionInScreen Styled Avalonia Property
#endif

        protected void UpdatePosition(PointerEventArgs e)
        {
            try
            {
#if DEBUG
                CurrentPointerPositionInScreen = GetCurrentPointInScreen(e).ToPoint2D();
#endif
                PointerShift = GetCurrentPointInScreen(e) - StartPointerPosition;
            }
            catch
            {
                return;
            }
            Point2D pointerShift = PointerShift.ToPoint2D();

            if (!_startMoving && pointerShift.AbsSquared() >= MinDistance)
            {
                _startMoving = true;
            }

            if (_startMoving)
            {
                if (this.CanRestore)
                {
                    this.Restore();
                    StartWindowPosition = CurrentScreenPointBehavior.CurrentScreenPointValue.ToPixelPoint();
                    Position = StartWindowPosition;
                }
                else
                {
                    this.Position = StartWindowPosition + PointerShift;
                }
            }
        }

        internal void Resize(Size size)
        {
            this.ArrangeSetBounds(size);
            //PlatformImpl?.Resize(size);
        }

        #region DragOnBeginMove Styled Avalonia Property
        public bool DragOnBeginMove
        {
            get { return GetValue(DragOnBeginMoveProperty); }
            set { SetValue(DragOnBeginMoveProperty, value); }
        }

        public static readonly StyledProperty<bool> DragOnBeginMoveProperty =
            AvaloniaProperty.Register<CustomWindow, bool>
            (
                nameof(DragOnBeginMove),
                true
            );
        #endregion DragOnBeginMove Styled Avalonia Property


        #region HeaderTemplate Avalonia Property
        public ControlTemplate HeaderTemplate
        {
            get { return GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly StyledProperty<ControlTemplate> HeaderTemplateProperty =
            AvaloniaProperty.Register<CustomWindow, ControlTemplate>
            (
                "HeaderTemplate"
            );
        #endregion HeaderTemplate Avalonia Property


        #region ResizeMargin Avalonia Property
        public double ResizeMargin
        {
            get { return GetValue(ResizeMarginProperty); }
            set { SetValue(ResizeMarginProperty, value); }
        }

        public static readonly AttachedProperty<double> ResizeMarginProperty =
            AvaloniaProperty.RegisterAttached<CustomWindow, Control, double>
            (
                "ResizeMargin"
            );
        #endregion ResizeMargin Avalonia Property


        #region HeaderBackground Avalonia Property
        public IBrush HeaderBackground
        {
            get { return GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        public static readonly AttachedProperty<IBrush> HeaderBackgroundProperty =
            AvaloniaProperty.RegisterAttached<CustomWindow, Control, IBrush>
            (
                "HeaderBackground"
            );
        #endregion HeaderBackground Avalonia Property


        #region HeaderHeight Avalonia Property
        public double HeaderHeight
        {
            get { return GetValue(HeaderHeightProperty); }
            set { SetValue(HeaderHeightProperty, value); }
        }

        public static readonly AttachedProperty<double> HeaderHeightProperty =
            AvaloniaProperty.RegisterAttached<CustomWindow, Control, double>
            (
                "HeaderHeight"
            );
        #endregion HeaderHeight Avalonia Property


        #region HeaderSeparatorHeight Avalonia Property
        public double HeaderSeparatorHeight
        {
            get { return GetValue(HeaderSeparatorHeightProperty); }
            set { SetValue(HeaderSeparatorHeightProperty, value); }
        }

        public static readonly AttachedProperty<double> HeaderSeparatorHeightProperty =
            AvaloniaProperty.RegisterAttached<CustomWindow, Control, double>
            (
                "HeaderSeparatorHeight"
            );
        #endregion HeaderSeparatorHeight Avalonia Property


        #region HeaderSeparatorBrush Avalonia Property
        public IBrush HeaderSeparatorBrush
        {
            get { return GetValue(HeaderSeparatorBrushProperty); }
            set { SetValue(HeaderSeparatorBrushProperty, value); }
        }

        public static readonly AttachedProperty<IBrush> HeaderSeparatorBrushProperty =
            AvaloniaProperty.RegisterAttached<CustomWindow, Control, IBrush>
            (
                "HeaderSeparatorBrush"
            );
        #endregion HeaderSeparatorBrush Avalonia Property


        #region CustomHeaderIcon Avalonia Property
        public Bitmap CustomHeaderIcon
        {
            get { return GetValue(CustomHeaderIconProperty); }
            set { SetValue(CustomHeaderIconProperty, value); }
        }

        public static readonly AttachedProperty<Bitmap> CustomHeaderIconProperty =
            AvaloniaProperty.RegisterAttached<CustomWindow, Control, Bitmap>
            (
                "CustomHeaderIcon"
            );
        #endregion CustomHeaderIcon Avalonia Property

        #region TitleMargin Styled Avalonia Property
        public Thickness TitleMargin
        {
            get { return GetValue(TitleMarginProperty); }
            set { SetValue(TitleMarginProperty, value); }
        }

        public static readonly StyledProperty<Thickness> TitleMarginProperty =
            AvaloniaProperty.Register<CustomWindow, Thickness>
            (
                nameof(TitleMargin)
            );
        #endregion TitleMargin Styled Avalonia Property


        #region CanMaximizeFlag Styled Avalonia Property
        public bool CanMaximizeFlag
        {
            get { return GetValue(CanMaximizeFlagProperty); }
            set { SetValue(CanMaximizeFlagProperty, value); }
        }

        public static readonly StyledProperty<bool> CanMaximizeFlagProperty =
            AvaloniaProperty.Register<CustomWindow, bool>
            (
                nameof(CanMaximizeFlag),
                true
            );
        #endregion CanMaximizeFlag Styled Avalonia Property


        #region CanReallyMaximize Styled Avalonia Property
        public bool CanReallyMaximize
        {
            get { return GetValue(CanReallyMaximizeProperty); }
            private set { SetValue(CanReallyMaximizeProperty, value); }
        }

        public static readonly StyledProperty<bool> CanReallyMaximizeProperty =
            AvaloniaProperty.Register<CustomWindow, bool>
            (
                nameof(CanReallyMaximize)
            );
        #endregion CanReallyMaximize Styled Avalonia Property


        #region CanRestoreFlag Styled Avalonia Property
        public bool CanRestoreFlag
        {
            get { return GetValue(CanRestoreFlagProperty); }
            set { SetValue(CanRestoreFlagProperty, value); }
        }

        public static readonly StyledProperty<bool> CanRestoreFlagProperty =
            AvaloniaProperty.Register<CustomWindow, bool>
            (
                nameof(CanRestoreFlag),
                true
            );
        #endregion CanRestoreFlag Styled Avalonia Property


        #region CanReallyRestore Styled Avalonia Property
        public bool CanReallyRestore
        {
            get { return GetValue(CanReallyRestoreProperty); }
            private set { SetValue(CanReallyRestoreProperty, value); }
        }

        public static readonly StyledProperty<bool> CanReallyRestoreProperty =
            AvaloniaProperty.Register<CustomWindow, bool>
            (
                nameof(CanReallyRestore)
            );
        #endregion CanReallyRestore Styled Avalonia Property


        #region CustomHeaderIconWidth Avalonia Property
        public double CustomHeaderIconWidth
        {
            get { return GetValue(CustomHeaderIconWidthProperty); }
            set { SetValue(CustomHeaderIconWidthProperty, value); }
        }

        public static readonly AttachedProperty<double> CustomHeaderIconWidthProperty =
            AvaloniaProperty.RegisterAttached<CustomWindow, Control, double>
            (
                "CustomHeaderIconWidth",
                double.NaN
            );
        #endregion CustomHeaderIconWidth Avalonia Property


        #region CustomHeaderIconHeight Avalonia Property
        public double CustomHeaderIconHeight
        {
            get { return GetValue(CustomHeaderIconHeightProperty); }
            set { SetValue(CustomHeaderIconHeightProperty, value); }
        }

        public static readonly AttachedProperty<double> CustomHeaderIconHeightProperty =
            AvaloniaProperty.RegisterAttached<CustomWindow, Control, double>
            (
                "CustomHeaderIconHeight",
                double.NaN
            );
        #endregion CustomHeaderIconHeight Avalonia Property


        #region CustomHeaderIconMargin Avalonia Property
        public Thickness CustomHeaderIconMargin
        {
            get { return GetValue(CustomHeaderIconMarginProperty); }
            set { SetValue(CustomHeaderIconMarginProperty, value); }
        }

        public static readonly AttachedProperty<Thickness> CustomHeaderIconMarginProperty =
            AvaloniaProperty.RegisterAttached<CustomWindow, Control, Thickness>
            (
                "CustomHeaderIconMargin"
            );
        #endregion CustomHeaderIconMargin Avalonia Property


        #region TitleClasses Styled Avalonia Property
        public string TitleClasses
        {
            get { return GetValue(TitleClassesProperty); }
            set { SetValue(TitleClassesProperty, value); }
        }

        public static readonly StyledProperty<string> TitleClassesProperty =
            AvaloniaProperty.Register<CustomWindow, string>
            (
                nameof(TitleClasses)
            );
        #endregion TitleClasses Styled Avalonia Property

        #region HeaderContent Styled Avalonia Property
        public object HeaderContent
        {
            get { return GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }

        public static readonly StyledProperty<object> HeaderContentProperty =
            AvaloniaProperty.Register<CustomWindow, object>
            (
                nameof(HeaderContent)
            );
        #endregion HeaderContent Styled Avalonia Property

        #region HeaderContentTemplate Styled Avalonia Property
        public IDataTemplate HeaderContentTemplate
        {
            get { return GetValue(HeaderContentTemplateProperty); }
            set { SetValue(HeaderContentTemplateProperty, value); }
        }

        public static readonly StyledProperty<IDataTemplate> HeaderContentTemplateProperty =
            AvaloniaProperty.Register<CustomWindow, IDataTemplate>
            (
                nameof(HeaderContentTemplate)
            );
        #endregion HeaderContentTemplate Styled Avalonia Property


        #region IsCustomHeaderVisible Styled Avalonia Property
        public bool IsCustomHeaderVisible
        {
            get { return GetValue(IsCustomHeaderVisibleProperty); }
            set { SetValue(IsCustomHeaderVisibleProperty, value); }
        }

        public static readonly StyledProperty<bool> IsCustomHeaderVisibleProperty =
            AvaloniaProperty.Register<CustomWindow, bool>
            (
                nameof(IsCustomHeaderVisible),
                true
            );
        #endregion IsCustomHeaderVisible Styled Avalonia Property


        #region HasCustomWindowFeatures Styled Avalonia Property
        public bool HasCustomWindowFeatures
        {
            get { return GetValue(HasCustomWindowFeaturesProperty); }
            set { SetValue(HasCustomWindowFeaturesProperty, value); }
        }

        public static readonly StyledProperty<bool> HasCustomWindowFeaturesProperty =
            AvaloniaProperty.Register<CustomWindow, bool>
            (
                nameof(HasCustomWindowFeatures),
                true
            );
        #endregion HasCustomWindowFeatures Styled Avalonia Property


        #region CanClose Styled Avalonia Property
        public bool CanClose
        {
            get { return GetValue(CanCloseProperty); }
            set { SetValue(CanCloseProperty, value); }
        }

        public static readonly StyledProperty<bool> CanCloseProperty =
            AvaloniaProperty.Register<CustomWindow, bool>
            (
                nameof(CanClose),
                true
            );
        #endregion CanClose Styled Avalonia Property

        #region TitleAreaContent Styled Avalonia Property
        public object TitleAreaContent
        {
            get { return GetValue(TitleAreaContentProperty); }
            set { SetValue(TitleAreaContentProperty, value); }
        }

        public static readonly StyledProperty<object> TitleAreaContentProperty =
            AvaloniaProperty.Register<CustomWindow, object>
            (
                nameof(TitleAreaContent)
            );
        #endregion TitleAreaContent Styled Avalonia Property

        #region TitleAreaContentTemplate Styled Avalonia Property
        public IDataTemplate TitleAreaContentTemplate
        {
            get { return GetValue(TitleAreaContentTemplateProperty); }
            set { SetValue(TitleAreaContentTemplateProperty, value); }
        }

        public static readonly StyledProperty<IDataTemplate> TitleAreaContentTemplateProperty =
            AvaloniaProperty.Register<CustomWindow, IDataTemplate>
            (
                nameof(TitleAreaContentTemplate)
            );
        #endregion TitleAreaContentTemplate Styled Avalonia Property


        #region ButtonsAreaTemplate Styled Avalonia Property
        public ControlTemplate ButtonsAreaTemplate
        {
            get { return GetValue(ButtonsAreaTemplateProperty); }
            set { SetValue(ButtonsAreaTemplateProperty, value); }
        }

        public static readonly StyledProperty<ControlTemplate> ButtonsAreaTemplateProperty =
            AvaloniaProperty.Register<CustomWindow, ControlTemplate>
            (
                nameof(ButtonsAreaTemplate)
            );
        #endregion ButtonsAreaTemplate Styled Avalonia Property


        #region AreCustomWindowButtonsVisible Styled Avalonia Property
        public bool AreCustomWindowButtonsVisible
        {
            get { return GetValue(AreCustomWindowButtonsVisibleProperty); }
            set { SetValue(AreCustomWindowButtonsVisibleProperty, value); }
        }

        public static readonly StyledProperty<bool> AreCustomWindowButtonsVisibleProperty =
            AvaloniaProperty.Register<CustomWindow, bool>
            (
                nameof(AreCustomWindowButtonsVisible),
                true
            );
        #endregion AreCustomWindowButtonsVisible Styled Avalonia Property

        #region MenuPathClasses Styled Avalonia Property
        public string MenuPathClasses
        {
            get { return GetValue(MenuPathClassesProperty); }
            set { SetValue(MenuPathClassesProperty, value); }
        }

        public static readonly StyledProperty<string> MenuPathClassesProperty =
            AvaloniaProperty.Register<CustomWindow, string>
            (
                nameof(MenuPathClasses)
            );
        #endregion MenuPathClasses Styled Avalonia Property


        #region WindowButtonClasses Styled Avalonia Property
        public string WindowButtonClasses
        {
            get { return GetValue(WindowButtonClassesProperty); }
            set { SetValue(WindowButtonClassesProperty, value); }
        }

        public static readonly StyledProperty<string> WindowButtonClassesProperty =
            AvaloniaProperty.Register<CustomWindow, string>
            (
                nameof(WindowButtonClasses)
            );
        #endregion WindowButtonClasses Styled Avalonia Property


        #region MenuForeground Styled Avalonia Property
        public IBrush MenuForeground
        {
            get { return GetValue(MenuForegroundProperty); }
            set { SetValue(MenuForegroundProperty, value); }
        }

        public static readonly StyledProperty<IBrush> MenuForegroundProperty =
            AvaloniaProperty.Register<CustomWindow, IBrush>
            (
                nameof(MenuForeground)
            );
        #endregion MenuForeground Styled Avalonia Property
    }
}
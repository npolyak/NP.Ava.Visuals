// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using NP.Utilities;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace NP.Ava.Visuals.Behaviors
{
    internal abstract class CurrentScreenPointBehaviorBase
    {
        private Control? _capturedControl;
        public Window? CapturedWindow => _capturedControl?.GetControlsWindow<Window>();
        public IInputElement CapturedControl =>
            Mouse?.TryGetPointer(null)?.Captured;

        private IMouseDevice? Mouse
        {
            get
            {
                return CapturedWindow
                        ?.PlatformImpl
                        ?.GetPropValue<IMouseDevice>("MouseDevice", true);
            }
        }

        protected Subject<Point2D> _currentScreenPoint = new Subject<Point2D>();
        public IObservable<Point2D> CurrentScreenPoint => _currentScreenPoint;

        public Point2D CurrentScreenPointValue { get; protected set; } = new Point2D();

        public event Action? PointerReleasedEvent;

        protected virtual void OnCapturedControlChanged(Control? newControl, Control? oldControl, PointerEventArgs e)
        {
            DisconnectControl(oldControl);
            DisconnectControl(newControl);
            if (newControl != null)
            {
                newControl.PointerReleased += Control_PointerReleased;
            }
        }

        protected Control? CaptureImpl(Control control, PointerEventArgs e = null)
        {
            Control? oldControl = _capturedControl;

            _capturedControl = control;

            IPointer? pointer = e?.Pointer ?? Mouse?.TryGetPointer(null);

            if (pointer != null)
            {
                pointer?.Capture(control);
            }

            return oldControl;
        }

        public void Capture(Control control, PointerEventArgs e)
        {
            var oldControl = CaptureImpl(control, e);
            OnCapturedControlChanged(_capturedControl, oldControl, e);
        }

        public abstract void CaptureOnInitPointerPosition(Control control, Point initPointerPositionWithinWindow);

        protected abstract void DisconnectControl(Control? control);

        protected void ReleaseCapture(PointerEventArgs e)
        {
            DisconnectControl(_capturedControl);

            if (e?.Pointer != null)
            {
                e.Pointer.Capture(null);
            }
            else
            {
                Mouse?.TryGetPointer(null)?.Capture(null);
            }
            _capturedControl = null;
        }

        protected void Control_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            BeforeCaptureRelease(e);

            ReleaseCapture(e);

            PointerReleasedEvent?.Invoke();
        }

        protected virtual void BeforeCaptureRelease(PointerReleasedEventArgs e)
        {

        }

        protected void SetCurrentPosition(Point position)
        {
            CurrentScreenPointValue = position.ToPoint2D();
            _currentScreenPoint.OnNext(CurrentScreenPointValue);
        }
    }

    internal class CurrentScreenPointBehaviorPointerEventControlBasedImpl : CurrentScreenPointBehaviorBase
    {
        public override void CaptureOnInitPointerPosition(Control control, Point initPointerPositionWithinWindow)
        {
            throw new NotImplementedException();
        }

        protected override void OnCapturedControlChanged(Control? newControl, Control? oldControl, PointerEventArgs e)
        {
            base.OnCapturedControlChanged(newControl, oldControl, e);

            if (newControl != null)
            {
                newControl.PointerMoved += CapturedControl_PointerMoved;
            }
        }

        private void CapturedControl_PointerMoved(object? sender, PointerEventArgs e)
        {
            SetPosition(e);
        }

        private void SetPosition(PointerEventArgs e)
        {
            Visual? capturedControl = CapturedControl as Visual;

            if (capturedControl == null)
                return;

            SetCurrentPosition(capturedControl.PointToScreen(e.GetPosition(capturedControl)).ToPoint(1));
        }

        protected override void BeforeCaptureRelease(PointerReleasedEventArgs e)
        {
            base.BeforeCaptureRelease(e);
            SetPosition(e);
        }

        protected override void DisconnectControl(Control? control)
        {
            if (control != null)
            {
                control.PointerMoved -= CapturedControl_PointerMoved;
                control.PointerReleased -= Control_PointerReleased;
            }
        }
    }

    internal class CurrentScreenPointBehaviorPointerEventImpl : CurrentScreenPointBehaviorBase
    {
        public CurrentScreenPointBehaviorPointerEventImpl()
        {
            InputManager.Instance.Process.Subscribe(OnInputReceived);
        }

        private void OnInputReceived(RawInputEventArgs e)
        {


            bool handled = e.Handled;
            if (!handled && e is RawPointerEventArgs margs)
                ProcessRawEvent(margs);
        }

        private void ProcessRawEvent(RawPointerEventArgs e)
        {
            if (CapturedWindow == null)
                return;

            var position = CapturedWindow.PointToScreen(e.Position);

#if DEBUG
            Debug.WriteLine($"Current Position: {position.ToPoint2D()}");
#endif
            SetCurrentPosition(position.ToPoint(1));
        }

        protected override void DisconnectControl(Control? control)
        {
            if (control != null)
            {
                control.PointerReleased -= Control_PointerReleased;
            }
        }

        public override void CaptureOnInitPointerPosition(Control control, Point initPointerPositionWithinWindow)
        {
            throw new NotImplementedException();
        }
    }

    internal class CurrentScreenPointBehaviorWindowPositionImpl : CurrentScreenPointBehaviorBase
    {
        private Point? _startPointerInWindowPosition { get; set; }

        protected override void DisconnectControl(Control? control)
        {
            var window = control?.GetControlsWindow<Window>();

            if (window != null)
            {
                window.PointerReleased -= Control_PointerReleased;
            }
        }

        protected override void OnCapturedControlChanged(Control? newControl, Control? oldControl, PointerEventArgs e)
        {
            base.OnCapturedControlChanged(newControl, oldControl, e);

            if (CapturedWindow != null)
            {
                if (e != null)
                {
                    _startPointerInWindowPosition = e.GetPosition(CapturedWindow);
                }
                else
                {

                }
                CapturedWindow.PositionChanged += CapturedWindow_PositionChanged;
            }
        }

        private void CapturedWindow_PositionChanged(object? sender, PixelPointEventArgs e)
        {
            SetCurrentPosition(e.Point.ToPoint(1).Add(_startPointerInWindowPosition.Value));
        }

        public override void CaptureOnInitPointerPosition(Control control, Point initPointerPositionWithinWindow)
        {
            _startPointerInWindowPosition = initPointerPositionWithinWindow;

            // first capture, then add pointermoved handler and on the first pointer move
            // event set initiali window position in screen, 
            // then unset the pointer moved event and use windows position
            var oldControl = CaptureImpl(control, null);

            DisconnectControl(oldControl);
            DisconnectControl(control);

            control.PointerMoved += Control_PointerMoved;

            // just in case, pointer moved never happens, use pointer released to 
            // release all handlers (if it happens before pointer moves)
            control.PointerReleased += OnPointerReleased;
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            ClearPointerHandlers(sender);

            ReleaseCapture(e);
        }

        private async void Control_PointerMoved(object? sender, PointerEventArgs e)
        {
            ClearPointerHandlers(sender);

            Point pointerScreenPosition = CapturedWindow.PointToScreen(e.GetPosition(CapturedWindow)).ToPoint(1);

            CapturedWindow.Position = pointerScreenPosition.Subtract(_startPointerInWindowPosition.Value).ToPixelPoint();

            CapturedWindow.PositionChanged += CapturedWindow_PositionChanged;

            Control capturedControl = sender as Control;
            if (capturedControl != null)
            {
                capturedControl.PointerReleased += Control_PointerReleased;
            }

            //await Task.Delay(500);

            var args =
                new PointerPressedEventArgs
                (
                    e.Source, 
                    e.Pointer,
                    capturedControl, 
                    _startPointerInWindowPosition.Value, 
                    e.Timestamp, 
                    e.GetPropValue<PointerPointProperties>("Properties", true), 
                    e.KeyModifiers);

            CapturedControl.RaiseEvent(args);
        }

        private void ClearPointerHandlers(object? sender)
        {
            Control? control = sender as Control;

            if (control == null)
            {
                return;
            }
            DisconnectControl(control);
            control.PointerMoved -= Control_PointerMoved;
            control.PointerReleased -= OnPointerReleased;
        }
    }

    public static class CurrentScreenPointBehavior
    {
        public static bool IsPointerEventBased
        {
            get; private set;
        }

        private static CurrentScreenPointBehaviorBase _pointerEventsImplementation =
            OSUtils.IsMac ? new CurrentScreenPointBehaviorPointerEventControlBasedImpl() : new CurrentScreenPointBehaviorPointerEventImpl();

        private static CurrentScreenPointBehaviorBase _windowPositionImplementation =
            new CurrentScreenPointBehaviorWindowPositionImpl();

        private static CurrentScreenPointBehaviorBase? _currentImplementation;

        public static Action? PointerReleasedEvent;

        public static void SetCurrent(bool pointerEventImpl)
        {
            if (_currentImplementation != null)
            {
                _currentImplementation.PointerReleasedEvent -= _currentImplementation_PointerReleasedEvent;
            }

            _currentImplementation = pointerEventImpl ? _pointerEventsImplementation : _windowPositionImplementation;

            if (_currentImplementation != null)
            {
                _currentImplementation.PointerReleasedEvent += _currentImplementation_PointerReleasedEvent;
            }
        }

        private static void _currentImplementation_PointerReleasedEvent()
        {
            PointerReleasedEvent?.Invoke();
            _currentImplementation = null;
        }

        public static Window? CapturedWindow => _currentImplementation.CapturedWindow;

        public static IInputElement? CapturedControl => _currentImplementation.CapturedControl;

        public static IObservable<Point2D> CurrentScreenPoint => _currentImplementation.CurrentScreenPoint;

        public static Point2D CurrentScreenPointValue => _currentImplementation.CurrentScreenPointValue;

        public static void Capture(Control control, bool pointerEventImplementation, PointerEventArgs e)
        {
            IsPointerEventBased = pointerEventImplementation;

            _currentImplementation = pointerEventImplementation ? _pointerEventsImplementation : _windowPositionImplementation;

            _currentImplementation.Capture(control, e);
        }

        public static void CaptureOnInitPointerPosition
        (
            Control control,
            Point initPointerPositionWithinWindow)
        {
            IsPointerEventBased = false;
            _currentImplementation = _windowPositionImplementation;

            _currentImplementation.CaptureOnInitPointerPosition(control, initPointerPositionWithinWindow);
        }
    }
}

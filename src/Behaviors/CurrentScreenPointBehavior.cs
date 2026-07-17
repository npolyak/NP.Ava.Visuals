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
using Avalonia.VisualTree;
using NP.Utilities;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace NP.Ava.Visuals.Behaviors
{
    public static class CurrentScreenPointBehavior
    {
        private static Subject<PixelPoint> _currentScreenPoint = new Subject<PixelPoint>();
        public static IObservable<PixelPoint> CurrentScreenPoint => _currentScreenPoint;

        public static PixelPoint CurrentScreenPointValue { get; private set; } = PixelPoint.Origin;

        public static event Action PointerReleasedEvent;

        static CurrentScreenPointBehavior()
        {
            InputManager.Instance!.Process.Subscribe(OnInputReceived);
        }

        private static void OnInputReceived(RawInputEventArgs e)
        {
            bool handled = e.Handled;
            if (!handled && e is RawPointerEventArgs margs)
                ProcessRawEvent(margs);
        }

        private static void ProcessRawEvent(RawPointerEventArgs e)
        {
            if (_capturedWindow == null)
                return;

            if (_releasing)
            {
                // for some reason after realease, the 
                // pointer position is skewed in 12.0.1 
                // avalonia version
                return;
            }
            var position = _capturedWindow.PointToScreen(e.Position);
            //System.Diagnostics.Debug.WriteLine($"CurrentScreenPointBehavior: Captured window = {_capturedWindow.Title}");
            //System.Diagnostics.Debug.WriteLine($"CurrentScreenPointBehavior: position = {position}");

            // var rootPoint = _capturedWindow.PointToClient(position);
            // var transform = _capturedWindow.TransformToVisual(_capturedWindow);
            // CurrentScreenPoint = _capturedWindow.PointToScreen(rootPoint * transform!.Value);

            CurrentScreenPointValue = position;

            _currentScreenPoint.OnNext(CurrentScreenPointValue);
        }

        public static Window _capturedWindow;

        public static Window? CapturedWindow => _capturedWindow;
        public static IInputElement CapturedControl =>
            Mouse?.TryGetPointer(null)?.Captured;


        static IMouseDevice _mouseDevice = null;
        private static IMouseDevice Mouse
        {
            get
            {
                /// Somehow I need to renew the mouse device every time (otherwise it won't work under 
                /// Avalonia 11
                //if (_mouseDevice == null)
                //{
                _mouseDevice = 
                    _capturedWindow
                        ?.PlatformImpl
                        ?.GetPropValue<IMouseDevice>("MouseDevice", true);
                
                //}
                return _mouseDevice;
            }
        }

        public static void Capture(Control control, PointerEventArgs e)
        {
            if (_releasing)
            {
                return;
            }

            _capturedWindow = 
                control.GetSelfAndVisualAncestors()
                       .OfType<Window>()
                       .FirstOrDefault()!;

            CurrentScreenPointValue = _capturedWindow.PointToScreen(e.GetPosition(_capturedWindow));

            var pointer = e?.Pointer ?? Mouse?.TryGetPointer(null);
                
            if (pointer != null)
            {
                 pointer?.Capture(control);
            }   


            control.PointerReleased -= Control_PointerReleased;
            control.PointerReleased += Control_PointerReleased;
        }

        static bool _releasing = false;

        public static void ReleaseCapture(PointerEventArgs e)
        {
            if (CapturedControl != null)
            {
                CapturedControl.PointerReleased -= Control_PointerReleased;
            }

            if (e?.Pointer != null)
            {
                _releasing = true;
                try
                {
                    //System.Diagnostics.Debug.WriteLine("CurrentScreenPointBehavior: NO CAPTURED WINDOW");
                    e.Pointer.Capture(null);
                    _releasing = false;
                }
                finally
                {
                    _releasing = false;
                }

            }
            else
            {
                Mouse?.TryGetPointer(null)?.Capture(null);
            }
            _capturedWindow = null;
        }

        private static void Control_PointerReleased(object sender, PointerReleasedEventArgs e)
        {
            ReleaseCapture(e);

            PointerReleasedEvent?.Invoke();
        }
    }
}

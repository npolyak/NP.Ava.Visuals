using Avalonia;
using Avalonia.Controls;
using NP.Utilities;
using System;

namespace NP.Ava.Visuals.Behaviors
{
    public static class WindowSystemDecorationChangeBehavior
    {

        #region TheWindowState Attached Avalonia Property
        public static WindowState? GetTheWindowState(Window obj)
        {
            return obj.GetValue(TheWindowStateProperty);
        }

        public static void SetTheWindowState(Window obj, WindowState? value)
        {
            obj.SetValue(TheWindowStateProperty, value);
        }

        public static readonly AttachedProperty<WindowState?> TheWindowStateProperty =
            AvaloniaProperty.RegisterAttached<Window, Window, WindowState?>
            (
                "TheWindowState"
            );
        #endregion TheWindowState Attached Avalonia Property

        static WindowSystemDecorationChangeBehavior()
        {
            if (OSUtils.IsWindows)
            {
                TheWindowStateProperty.Changed.Subscribe(OnWindowStateChanged);
            }
        }

        private static void OnWindowStateChanged(AvaloniaPropertyChangedEventArgs<WindowState?> args)
        {
            Window window = (Window)args.Sender;

            if (window.WindowState == WindowState.Normal)
            {
                window.SystemDecorations = SystemDecorations.Full;
            }
            else if (window.WindowState is WindowState.Maximized or WindowState.FullScreen && OSUtils.IsWindows)
            {
                window.SystemDecorations = SystemDecorations.Full;
                window.SystemDecorations = SystemDecorations.BorderOnly;
            }
        }
    }
}

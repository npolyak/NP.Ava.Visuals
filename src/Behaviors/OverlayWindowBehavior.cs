using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Templates;
using NP.Avalonia.Visuals;
using NP.Utilities;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class OverlayWindowBehavior
    {
        #region IsOpen Attached Avalonia Property
        public static bool GetIsOpen(Control obj)
        {
            return obj.GetValue(IsOpenProperty);
        }

        public static void SetIsOpen(Control obj, bool value)
        {
            obj.SetValue(IsOpenProperty, value);
        }

        public static readonly AttachedProperty<bool> IsOpenProperty =
            AvaloniaProperty.RegisterAttached<OverlayWindowBehavior, Control, bool>
            (
                "IsOpen"
            );
        #endregion IsOpen Attached Avalonia Property

        #region Padding Attached Avalonia Property
        public static Thickness GetPadding(Control obj)
        {
            return obj.GetValue(PaddingProperty);
        }

        public static void SetPadding(Control obj, Thickness value)
        {
            obj.SetValue(PaddingProperty, value);
        }

        public static readonly AttachedProperty<Thickness> PaddingProperty =
            AvaloniaProperty.RegisterAttached<OverlayWindowBehavior, Control, Thickness>
            (
                "Padding"
            );
        #endregion Padding Attached Avalonia Property


        #region Content Attached Avalonia Property
        public static object GetContent(Control obj)
        {
            return obj.GetValue(ContentProperty);
        }

        public static void SetContent(Control obj, object value)
        {
            obj.SetValue(ContentProperty, value);
        }

        public static readonly AttachedProperty<object> ContentProperty =
            AvaloniaProperty.RegisterAttached<OverlayWindowBehavior, Control, object>
            (
                "Content"
            );
        #endregion Content Attached Avalonia Property


        #region ContentTemplate Attached Avalonia Property
        public static DataTemplate GetContentTemplate(Control obj)
        {
            return obj.GetValue(ContentTemplateProperty);
        }

        public static void SetContentTemplate(Control obj, DataTemplate value)
        {
            obj.SetValue(ContentTemplateProperty, value);
        }

        public static readonly AttachedProperty<DataTemplate> ContentTemplateProperty =
            AvaloniaProperty.RegisterAttached<OverlayWindowBehavior, Control, DataTemplate>
            (
                "ContentTemplate"
            );
        #endregion ContentTemplate Attached Avalonia Property


        #region OverlayWindow Attached Avalonia Property
        public static Window GetOverlayWindow(Control obj)
        {
            return obj.GetValue(OverlayWindowProperty);
        }

        private static void SetOverlayWindow(Control obj, Window value)
        {
            obj.SetValue(OverlayWindowProperty, value);
        }

        private static readonly AttachedProperty<Window> OverlayWindowProperty =
            AvaloniaProperty.RegisterAttached<OverlayWindowBehavior, Control, Window>
            (
                "OverlayWindow"
            );
        #endregion OverlayWindow Attached Avalonia Property


        #region IsTopmost Attached Avalonia Property
        public static bool GetIsTopmost(Control obj)
        {
            return obj.GetValue(IsTopmostProperty);
        }

        public static void SetIsTopmost(Control obj, bool value)
        {
            obj.SetValue(IsTopmostProperty, value);
        }

        public static readonly AttachedProperty<bool> IsTopmostProperty =
            AvaloniaProperty.RegisterAttached<OverlayWindowBehavior, Control, bool>
            (
                "IsTopmost",
                true
            );
        #endregion IsTopmost Attached Avalonia Property


        #region OverlayedControl Attached Avalonia Property
        public static Control GetOverlayedControl(Control obj)
        {
            return obj.GetValue(OverlayedControlProperty);
        }

        public static void SetOverlayedControl(Control obj, Control value)
        {
            obj.SetValue(OverlayedControlProperty, value);
        }

        public static readonly AttachedProperty<Control> OverlayedControlProperty =
            AvaloniaProperty.RegisterAttached<OverlayWindowBehavior, Control, Control>
            (
                "OverlayedControl"
            );
        #endregion OverlayedControl Attached Avalonia Property


        static OverlayWindowBehavior()
        {
            IsOpenProperty.Changed.Subscribe(OnIsOpenChanged);
        }

        private static async void OnIsOpenChanged(AvaloniaPropertyChangedEventArgs<bool> args)
        {
            Control control = (Control) args.Sender;

            Window? overlayWindow = GetOverlayWindow(control);

            if (args.NewValue.Value)
            {
                if (overlayWindow == null)
                {
                    overlayWindow =
                        new Window()
                        {
                            TransparencyLevelHint = WindowTransparencyLevel.Transparent.ToCollection().ToImmutableList(),
                            Background = null,
                            CanResize = false,
                            SystemDecorations = SystemDecorations.None,
                            Topmost = GetIsTopmost(control),
                            Content = GetContent(control),
                            ContentTemplate = GetContentTemplate(control),
                            Padding = GetPadding(control)
                        };

                    SetOverlayWindow(control, overlayWindow);
                }

                Control overlayedControl = GetOverlayedControl(control) ?? control;

                Rect2D screenBounds = overlayedControl.GetScreenBounds();

                double renderScaling = overlayWindow.PlatformImpl.GetPropValue<double>("RenderScaling", true);

                double scale = 1d / renderScaling;
                overlayWindow.Position = screenBounds.StartPoint.ToPixelPoint();

                overlayWindow.PlatformImpl.CallMethod("Resize", screenBounds.GetSize(scale).ToSize(), WindowResizeReason.Application);
                overlayWindow.Show();
            }
            else
            {
                overlayWindow?.Hide();
            }
        }
    }
}

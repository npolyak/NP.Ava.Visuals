using Avalonia;
using Avalonia.Controls;
using NP.Ava.Visuals;
using NP.Utilities;
using System;
using System.Collections.Immutable;
using Avalonia.Controls.Templates;
using System.Linq;
using Avalonia.Animation;
using Avalonia.Markup.Xaml.Templates;

namespace NP.Ava.Visuals.Behaviors
{
    public class OverlayBehavior
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
            AvaloniaProperty.RegisterAttached<OverlayBehavior, Control, bool>
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
            AvaloniaProperty.RegisterAttached<OverlayBehavior, Control, Thickness>
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
            AvaloniaProperty.RegisterAttached<OverlayBehavior, Control, object>
            (
                "Content"
            );
        #endregion Content Attached Avalonia Property


        #region ContentTemplate Attached Avalonia Property
        public static IDataTemplate GetContentTemplate(Control obj)
        {
            return obj.GetValue(ContentTemplateProperty);
        }

        public static void SetContentTemplate(Control obj, IDataTemplate value)
        {
            obj.SetValue(ContentTemplateProperty, value);
        }

        public static readonly AttachedProperty<IDataTemplate> ContentTemplateProperty =
            AvaloniaProperty.RegisterAttached<OverlayBehavior, Control, IDataTemplate>
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
            AvaloniaProperty.RegisterAttached<OverlayBehavior, Control, Window>
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
            AvaloniaProperty.RegisterAttached<OverlayBehavior, Control, bool>
            (
                "IsTopmost",
                true
            );
        #endregion IsTopmost Attached Avalonia Property


        #region CurrentSide Attached Avalonia Property
        public static Side2D GetCurrentSide(Control obj)
        {
            return obj.GetValue(CurrentSideProperty);
        }

        public static void SetCurrentSide(Control obj, Side2D value)
        {
            obj.SetValue(CurrentSideProperty, value);
        }

        public static readonly AttachedProperty<Side2D> CurrentSideProperty =
            AvaloniaProperty.RegisterAttached<OverlayBehavior, Control, Side2D>
            (
                "CurrentSide",
                Side2D.Center
            );
        #endregion CurrentSide Attached Avalonia Property


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
            AvaloniaProperty.RegisterAttached<OverlayBehavior, Control, Control>
            (
                "OverlayedControl"
            );
        #endregion OverlayedControl Attached Avalonia Property


        #region IsWindowOverlay Attached Avalonia Property
        public static bool GetIsWindowOverlay(Control obj)
        {
            return obj.GetValue(IsWindowOverlayProperty);
        }

        private static void SetIsWindowOverlay(Control obj, bool value)
        {
            obj.SetValue(IsWindowOverlayProperty, value);
        }

        public static readonly AttachedProperty<bool> IsWindowOverlayProperty =
            AvaloniaProperty.RegisterAttached<Control, Control, bool>
            (
                "IsWindowOverlay"
            );
        #endregion IsWindowOverlay Attached Avalonia Property

        #region OverlayContainingPanel Attached Avalonia Property
        // this is non-null when we use overlay panel and not overlay window
        public static Panel GetOverlayContainingPanel(Control obj)
        {
            return obj.GetValue(OverlayContainingPanelProperty);
        }

        public static void SetOverlayContainingPanel(Control obj, Panel value)
        {
            obj.SetValue(OverlayContainingPanelProperty, value);
        }

        public static readonly AttachedProperty<Panel> OverlayContainingPanelProperty =
            AvaloniaProperty.RegisterAttached<Control, Control, Panel>
            (
                "OverlayContainingPanel"
            );
        #endregion OverlayContainingPanel Attached Avalonia Property


        static OverlayBehavior()
        {
            IsOpenProperty.Changed.Subscribe(OnIsOpenChanged);
            OverlayedControlProperty.Changed.Subscribe(OnOverlayedControlChanged);
            OverlayContainingPanelProperty.Changed.Subscribe(OnOverlayContainingPanelChanged);
            CurrentSideProperty.Changed.Subscribe(OnCurrentSideChanged);
        }

        private static void OnCurrentSideChanged(AvaloniaPropertyChangedEventArgs<Side2D> args)
        {
            Control rootContainer = (Control)args.Sender;

            AdjustOverlay(rootContainer);
        }

        private static void OnOverlayedControlChanged(AvaloniaPropertyChangedEventArgs<Control> args)
        {
            Control rootContainer = (Control)args.Sender;

            AdjustOverlay(rootContainer);
        }

        private static void OnOverlayContainingPanelChanged(AvaloniaPropertyChangedEventArgs<Panel> args)
        {
            Control control = (Control) args.Sender;

            // it is a window overlay only if OverlayContainingPanel is not set (null)
            bool isWindowOverlay = (!args.NewValue.HasValue) || (args.NewValue.Value == null);

            SetIsWindowOverlay(control, isWindowOverlay);
        }

        private static string ChildContentControlName = "ChildContentControl";

        private static void AdjustOverlay(Control topContainer)
        {
            Control overlayedControl = GetOverlayedControl(topContainer) ?? topContainer;

            if (overlayedControl == null)
                return;

            if (!overlayedControl.IsLoaded)
                return;

            Panel overlayPanel = GetOverlayContainingPanel(topContainer);

            if ((overlayPanel == null))
            {
                return;
            }

            if (!GetIsOpen(topContainer))
            {
                overlayPanel.IsVisible = false;
                return;
            }


            if (overlayedControl.GetControlsWindow<Window>() != overlayPanel.GetControlsWindow<Window>())
                return;

            var contentControl =
                overlayPanel.Children
                     .OfType<ContentControl>()
                     .FirstOrDefault(child => child.Name == ChildContentControlName);

            if (contentControl == null)
            {
                contentControl = new ContentControl
                {
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
                    Name = ChildContentControlName,
                    Content = GetContent(topContainer),
                    ContentTemplate = GetContentTemplate(topContainer),
                    Padding = GetPadding(topContainer),
                };

                overlayPanel.Children.Add(contentControl);

                Transitions transitions = [new ThicknessTransition { Property = Control.MarginProperty, Duration = TimeSpan.FromSeconds(0.2) }];

                contentControl.Transitions = transitions;
            }

            var currentSide = GetCurrentSide(topContainer);

            Thickness margin = overlayedControl.ToMargin(overlayPanel, currentSide);

            contentControl.Margin = margin;

            overlayPanel.IsVisible = true;
        }

        private static async void OnIsOpenChanged(AvaloniaPropertyChangedEventArgs<bool> args)
        {
            Control rootContainer = (Control) args.Sender;
            Control overlayedControl = GetOverlayedControl(rootContainer) ?? rootContainer;

            Panel panel = GetOverlayContainingPanel(rootContainer);

            bool shouldOpen = args.NewValue.Value;

            if (panel != null)
            {
                AdjustOverlay(rootContainer);
            }
            else
            { 
                Window? overlayWindow = GetOverlayWindow(rootContainer);

                if (shouldOpen)
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
                                Topmost = GetIsTopmost(rootContainer),
                                Content = GetContent(rootContainer),
                                ContentTemplate = GetContentTemplate(rootContainer),
                                Padding = GetPadding(rootContainer)
                            };

                        SetOverlayWindow(rootContainer, overlayWindow);
                    }

                    Rect2D screenBounds = overlayedControl.GetScreenBounds();

                    double renderScaling = overlayWindow.PlatformImpl.GetPropValue<double>("RenderScaling", true);

                    double scale = 1d / renderScaling;
                    overlayWindow.Position = screenBounds.StartPoint.ToPixelPoint();

                    overlayWindow.PlatformImpl.CallMethod("Resize", screenBounds.GetSize(scale).ToSize(), WindowResizeReason.Application);
                    overlayWindow.Show();
                }
                else // close
                {
                    overlayWindow?.Hide();
                }
            }
        }
    }
}

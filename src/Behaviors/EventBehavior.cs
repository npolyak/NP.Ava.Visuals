// (c) Nick Polyak 2022 - http://awebpros.com/
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
using Avalonia.Interactivity;
using System;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class EventBehavior
    {
        #region TheEvent Attached Avalonia Property
        public static RoutedEvent GetTheEvent(AvaloniaObject obj)
        {
            return obj.GetValue(TheEventProperty);
        }

        public static void SetTheEvent(AvaloniaObject obj, RoutedEvent value)
        {
            obj.SetValue(TheEventProperty, value);
        }

        public static readonly AttachedProperty<RoutedEvent> TheEventProperty =
            AvaloniaProperty.RegisterAttached<EventBehavior, Control, RoutedEvent>
            (
                "TheEvent"
            );
        #endregion TheEvent Attached Avalonia Property


        #region TheRoutingStrategy Attached Avalonia Property
        public static RoutingStrategies? GetTheRoutingStrategy(AvaloniaObject obj)
        {
            return obj.GetValue(TheRoutingStrategyProperty);
        }

        public static void SetTheRoutingStrategy(AvaloniaObject obj, RoutingStrategies? value)
        {
            obj.SetValue(TheRoutingStrategyProperty, value);
        }

        public static readonly AttachedProperty<RoutingStrategies?> TheRoutingStrategyProperty =
            AvaloniaProperty.RegisterAttached<EventBehavior, Control, RoutingStrategies?>
            (
                "TheRoutingStrategy",
                RoutingStrategies.Bubble
            );
        #endregion TheRoutingStrategy Attached Avalonia Property

        private static void ResetEvent(AvaloniaPropertyChangedEventArgs<RoutedEvent> e)
        {
            Interactive sender = (Interactive) e.Sender;

            if (e.OldValue.HasValue)
            {
                RoutedEvent routedEvent = e.OldValue.Value;

                DisconnectEventHandling(sender, routedEvent);
            }

            if (e.NewValue.HasValue)
            {
                RoutedEvent routedEvent = e.NewValue.Value;

                ConnectEventHandling(sender, routedEvent);
            }
        }

        private static void DisconnectEventHandling(Interactive? sender, RoutedEvent routedEvent)
        {
            if (sender == null)
                return;

            if (routedEvent != null)
            {
                sender?.RemoveHandler(routedEvent, (EventHandler<RoutedEventArgs>)OnEvent);
            }
        }


        private static void ConnectEventHandling(Interactive sender, RoutedEvent routedEvent)
        {
            if (sender == null)
                return;

            var routingStr = GetTheRoutingStrategy(sender);

            RoutingStrategies routingStrategies = routingStr ?? RoutingStrategies.Bubble | RoutingStrategies.Direct | RoutingStrategies.Tunnel;

            if (routedEvent != null)
            {
                sender?.AddHandler
                    (
                    routedEvent,
                    (EventHandler<RoutedEventArgs>)OnEvent,
                    routingStrategies);
            }
        }

        private static void OnEvent(object? sender, RoutedEventArgs e)
        {
            Control control = (Control)sender!;

            RoutedEventArgs args = new RoutedEventArgs();
            args.Source = control;
            args.RoutedEvent = GetEventToFire(control);

            control.RaiseEvent(args);
        }

        private static IDisposable? _eventSubscription = null;
        private static void Init()
        {

            _eventSubscription?.Dispose();
            _eventSubscription = TheEventProperty.Changed.Subscribe(ResetEvent);
        }


        #region EventToFire Attached Avalonia Property
        public static RoutedEvent GetEventToFire(Control obj)
        {
            return obj.GetValue(EventToFireProperty);
        }

        public static void SetEventToFire(Control obj, RoutedEvent value)
        {
            obj.SetValue(EventToFireProperty, value);
        }

        public static readonly AttachedProperty<RoutedEvent> EventToFireProperty =
            AvaloniaProperty.RegisterAttached<EventBehavior, Control, RoutedEvent>
            (
                "EventToFire"
            );
        #endregion EventToFire Attached Avalonia Property


        private static void ResetRoutingStrategy(AvaloniaPropertyChangedEventArgs<RoutingStrategies?> e)
        {
            Interactive? sender = (Interactive) e.Sender;

            RoutedEvent routedEvent = GetTheEvent(sender);
            DisconnectEventHandling(sender, routedEvent);

            ConnectEventHandling(sender, routedEvent);
        }


        static EventBehavior()
        {
            Init();

            TheRoutingStrategyProperty.Changed.Subscribe(ResetRoutingStrategy);
        }
    }
}

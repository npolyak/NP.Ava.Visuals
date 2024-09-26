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
//
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NP.Ava.Visuals.Behaviors
{
    public static class CallAction
    {
        #region TheEvent Attached Avalonia Property
        public static RoutedEvent GetTheEvent(Visual obj)
        {
            return obj.GetValue(TheEventProperty);
        }

        public static void SetTheEvent(Visual obj, RoutedEvent value)
        {
            obj.SetValue(TheEventProperty, value);
        }

        public static readonly AttachedProperty<RoutedEvent> TheEventProperty =
            AvaloniaProperty.RegisterAttached<Visual, Visual, RoutedEvent>
            (
                "TheEvent"
            );
        #endregion TheEvent Attached Avalonia Property


        #region TargetObject Attached Avalonia Property
        public static object GetTargetObject(Visual obj)
        {
            return obj.GetValue(TargetObjectProperty);
        }

        public static void SetTargetObject(Visual obj, object value)
        {
            obj.SetValue(TargetObjectProperty, value);
        }

        public static readonly AttachedProperty<object> TargetObjectProperty =
            AvaloniaProperty.RegisterAttached<Visual, Visual, object>
            (
                "TargetObject"
            );
        #endregion TargetObject Attached Avalonia Property


        #region MethodName Attached Avalonia Property
        public static string GetMethodName(Visual obj)
        {
            return obj.GetValue(MethodNameProperty);
        }

        public static void SetMethodName(Visual obj, string value)
        {
            obj.SetValue(MethodNameProperty, value);
        }

        public static readonly AttachedProperty<string> MethodNameProperty =
            AvaloniaProperty.RegisterAttached<Visual, Visual, string>
            (
                "MethodName"
            );
        #endregion MethodName Attached Avalonia Property

        private static void ResetEvent(AvaloniaPropertyChangedEventArgs<RoutedEvent> e)
        {
            Interactive sender = e.Sender as Interactive;

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


        #region TheRoutingStrategy Attached Avalonia Property
        public static RoutingStrategies? GetTheRoutingStrategy(Visual obj)
        {
            return obj.GetValue(TheRoutingStrategyProperty);
        }

        public static void SetTheRoutingStrategy(Visual obj, RoutingStrategies? value)
        {
            obj.SetValue(TheRoutingStrategyProperty, value);
        }

        public static readonly AttachedProperty<RoutingStrategies?> TheRoutingStrategyProperty =
            AvaloniaProperty.RegisterAttached<Visual, Visual, RoutingStrategies?>
            (
                "TheRoutingStrategy",
                RoutingStrategies.Bubble
            );
        #endregion TheRoutingStrategy Attached Avalonia Property


        #region StaticType Attached Avalonia Property
        public static Type GetStaticType(Visual obj)
        {
            return obj.GetValue(StaticTypeProperty);
        }

        public static void SetStaticType(Visual obj, Type value)
        {
            obj.SetValue(StaticTypeProperty, value);
        }

        public static readonly AttachedProperty<Type> StaticTypeProperty =
            AvaloniaProperty.RegisterAttached<Visual, Visual, Type>
            (
                "StaticType"
            );
        #endregion StaticType Attached Avalonia Property


        #region EventArgsMatcher Attached Avalonia Property
        public static IRoutedEventArgsMatcher GetEventArgsMatcher(this Interactive obj)
        {
            return obj.GetValue(EventArgsMatcherProperty);
        }

        public static void SetEventArgsMatcher(this Interactive obj, IRoutedEventArgsMatcher value)
        {
            obj.SetValue(EventArgsMatcherProperty, value);
        }

        public static readonly AttachedProperty<IRoutedEventArgsMatcher> EventArgsMatcherProperty =
            AvaloniaProperty.RegisterAttached<Interactive, Interactive, IRoutedEventArgsMatcher>
            (
                "EventArgsMatcher"
            );
        #endregion EventArgsMatcher Attached Avalonia Property


        #region TriggerObj Attached Avalonia Property
        public static object? GetTriggerObj(Visual obj)
        {
            return obj.GetValue(TriggerObjProperty);
        }

        public static void SetTriggerObj(Visual obj, object? value)
        {
            obj.SetValue(TriggerObjProperty, value);
        }

        public static readonly AttachedProperty<object?> TriggerObjProperty =
            AvaloniaProperty.RegisterAttached<Visual, Visual, object?>
            (
                "TriggerObj"
            );
        #endregion TriggerObj Attached Avalonia Property


        #region UseTriggerObjAsArg Attached Avalonia Property
        public static bool GetUseTriggerObjAsArg(Visual obj)
        {
            return obj.GetValue(UseTriggerObjAsArgProperty);
        }

        public static void SetUseTriggerObjAsArg(Visual obj, bool value)
        {
            obj.SetValue(UseTriggerObjAsArgProperty, value);
        }

        public static readonly AttachedProperty<bool> UseTriggerObjAsArgProperty =
            AvaloniaProperty.RegisterAttached<Visual, Visual, bool>
            (
                "UseTriggerObjAsArg"
            );
        #endregion UseTriggerObjAsArg Attached Avalonia Property


        private static void OnEvent(object? sender, RoutedEventArgs eventArgs)
        {
            Interactive? avaloniaObject = sender as Interactive;

            if (avaloniaObject == null)
            {
                return;
            }

            IRoutedEventArgsMatcher eventArgsMatcher = avaloniaObject.GetEventArgsMatcher();

            if ((eventArgsMatcher != null) && (!eventArgsMatcher.Matches(eventArgs)))
            {
                return;
            }

            CallMethodImpl(avaloniaObject);
        }

        private static void CallMethodImpl(Interactive avaloniaObject, object? triggerObj = null)
        {
            string methodName = avaloniaObject.GetValue(MethodNameProperty);

            if (methodName == null)
            {
                return;
            }

            Type staticType = GetStaticType(avaloniaObject);

            bool isStatic = staticType != null;

            object? targetObject =
                avaloniaObject.GetValue(TargetObjectProperty) ?? avaloniaObject.DataContext;

            if ((targetObject == null) && (!isStatic))
                return;

            IEnumerable<object> args = Enumerable.Empty<object>();

            if (GetUseTriggerObjAsArg(avaloniaObject))
            {
                args = [triggerObj];
            }
            else if (GetUseTargetObjAsFirstArg(avaloniaObject))
            {
                args = [targetObject];
            }

            if (GetHasArg(avaloniaObject))
            {
                args = args.Union([GetArg1(avaloniaObject)]);
            }
            else
            {
                args = args.Union(GetArgs(avaloniaObject).NullToEmpty());
            }

            if (isStatic)
            {
                targetObject = staticType;
            }

            targetObject.CallMethodExtras(methodName, false, isStatic, args.ToArray());
        }


        #region HasArg Attached Avalonia Property
        public static bool GetHasArg(Visual obj)
        {
            return obj.GetValue(HasArgProperty);
        }

        public static void SetHasArg(Visual obj, bool value)
        {
            obj.SetValue(HasArgProperty, value);
        }

        public static readonly AttachedProperty<bool> HasArgProperty =
            AvaloniaProperty.RegisterAttached<Visual, Visual, bool>
            (
                "HasArg"
            );
        #endregion HasArg Attached Avalonia Property


        #region Arg1 Attached Avalonia Property
        public static object GetArg1(AvaloniaObject obj)
        {
            return obj.GetValue(Arg1Property);
        }

        public static void SetArg1(AvaloniaObject obj, object value)
        {
            obj.SetValue(Arg1Property, value);
        }

        public static readonly AttachedProperty<object> Arg1Property =
            AvaloniaProperty.RegisterAttached<object, Control, object>
            (
                "Arg1"
            );
        #endregion Arg1 Attached Avalonia Property


        #region Args Attached Avalonia Property
        public static List<object> GetArgs(AvaloniaObject obj)
        {
            return obj.GetValue(ArgsProperty);
        }

        public static void SetArgs(AvaloniaObject obj, List<object> value)
        {
            obj.SetValue(ArgsProperty, value);
        }

        public static readonly AttachedProperty<List<object>> ArgsProperty =
            AvaloniaProperty.RegisterAttached<object, Control, List<object>>
            (
                "Args"
            );
        #endregion Args Attached Avalonia Property

        private static IDisposable _eventSubscription = null;
        private static void Init()
        {

            _eventSubscription?.Dispose();
            _eventSubscription = TheEventProperty.Changed.Subscribe(ResetEvent);
        }


        #region UseTargetObjAsFirstArg Attached Avalonia Property
        public static bool GetUseTargetObjAsFirstArg(Visual obj)
        {
            return obj.GetValue(UseTargetObjAsFirstArgProperty);
        }

        public static void SetUseTargetObjAsFirstArg(Visual obj, bool value)
        {
            obj.SetValue(UseTargetObjAsFirstArgProperty, value);
        }

        public static readonly AttachedProperty<bool> UseTargetObjAsFirstArgProperty =
            AvaloniaProperty.RegisterAttached<Visual, Visual, bool>
            (
                "UseTargetObjAsFirstArg"
            );
        #endregion UseTargetObjAsFirstArg Attached Avalonia Property


        private static void ResetRoutingStrategy(AvaloniaPropertyChangedEventArgs<RoutingStrategies?> e)
        {
            Interactive sender = e.Sender as Interactive;

            if (sender == null)
                return;

            RoutedEvent routedEvent = GetTheEvent(sender);
            DisconnectEventHandling(sender, routedEvent);

            ConnectEventHandling(sender, routedEvent);
        }

        static CallAction()
        {
            Init();

            TheRoutingStrategyProperty.Changed.Subscribe(ResetRoutingStrategy);

            TriggerObjProperty.Changed.Subscribe(OnTriggerObjChanged);
        }

        private static void OnTriggerObjChanged(AvaloniaPropertyChangedEventArgs<object> args)
        {
            CallMethodImpl((Interactive) args.Sender, args.NewValue.Value);
        }
    }
}

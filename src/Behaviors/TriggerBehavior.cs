﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using NP.Utilities;
using System;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class TriggerBehavior
    {
        #region SourceProperty Attached Avalonia Property
        public static AvaloniaProperty GetSourceProperty(Control obj)
        {
            return obj.GetValue(SourcePropertyProperty);
        }

        public static void SetSourceProperty(Control obj, AvaloniaProperty value)
        {
            obj.SetValue(SourcePropertyProperty, value);
        }

        public static readonly AttachedProperty<AvaloniaProperty> SourcePropertyProperty =
            AvaloniaProperty.RegisterAttached<object, Control, AvaloniaProperty>
            (
                "SourceProperty"
            );
        #endregion SourceProperty Attached Avalonia Property


        #region SourceTriggerValue Attached Avalonia Property
        public static object GetSourceTriggerValue(Control obj)
        {
            return obj.GetValue(SourceTriggerValueProperty);
        }

        public static void SetSourceTriggerValue(Control obj, object value)
        {
            obj.SetValue(SourceTriggerValueProperty, value);
        }

        public static readonly AttachedProperty<object> SourceTriggerValueProperty =
            AvaloniaProperty.RegisterAttached<object, Control, object>
            (
                "SourceTriggerValue"
            );
        #endregion SourceTriggerValue Attached Avalonia Property


        #region TargetProperty Attached Avalonia Property
        public static AvaloniaProperty GetTargetProperty(Control obj)
        {
            return obj.GetValue(TargetPropertyProperty);
        }

        public static void SetTargetProperty(Control obj, AvaloniaProperty value)
        {
            obj.SetValue(TargetPropertyProperty, value);
        }

        public static readonly AttachedProperty<AvaloniaProperty> TargetPropertyProperty =
            AvaloniaProperty.RegisterAttached<object, Control, AvaloniaProperty>
            (
                "TargetProperty"
            );
        #endregion TargetProperty Attached Avalonia Property


        #region TargetValue Attached Avalonia Property
        public static object GetTargetValue(Control obj)
        {
            return obj.GetValue(TargetValueProperty);
        }

        public static void SetTargetValue(Control obj, object value)
        {
            obj.SetValue(TargetValueProperty, value);
        }

        public static readonly AttachedProperty<object> TargetValueProperty =
            AvaloniaProperty.RegisterAttached<object, Control, object>
            (
                "TargetValue"
            );
        #endregion TargetValue Attached Avalonia Property

        private static void SetTargetValueImpl(Control control)
        {
            AvaloniaProperty sourceProperty = GetSourceProperty(control);
            AvaloniaProperty targetProperty = GetTargetProperty(control);

            if (sourceProperty == null || targetProperty == null)
            {
                return;
            }

            object sourceTriggerValue = GetSourceTriggerValue(control);
            object sourceValue = control.GetValue(sourceProperty);

            if (sourceTriggerValue == null)
            {
                return;
            }

            if (sourceValue.ObjEquals(sourceTriggerValue))
            {
                object targetValue = GetTargetValue(control);

                control.SetValue(targetProperty, targetValue);
            }
            else
            {
                control.ClearValue(targetProperty);
            }
        }


        #region TheSubscription Attached Avalonia Property
        private static IDisposable GetTheSubscription(Control obj)
        {
            return obj.GetValue(TheSubscriptionProperty);
        }

        private static void SetTheSubscription(Control obj, IDisposable value)
        {
            obj.SetValue(TheSubscriptionProperty, value);
        }

        private static readonly AttachedProperty<IDisposable> TheSubscriptionProperty =
            AvaloniaProperty.RegisterAttached<object, Control, IDisposable>
            (
                "TheSubscription"
            );
        #endregion TheSubscription Attached Avalonia Property


        static TriggerBehavior()
        {
            SourcePropertyProperty.Changed.Subscribe(OnSourcePropChanged);
            TargetPropertyProperty.Changed.Subscribe(OnTargetPropChanged);

            SourceTriggerValueProperty.Changed.Subscribe(OnValChanged);
            TargetValueProperty.Changed.Subscribe(OnValChanged);
        }

        private static void OnSourcePropChanged(AvaloniaPropertyChangedEventArgs<AvaloniaProperty> args)
        {
            Control control = args.Sender as Control;
            SetTargetValueImpl(control);

            IDisposable oldSubscription = GetTheSubscription(control);

            oldSubscription?.Dispose();

            AvaloniaProperty sourceProperty = GetSourceProperty(control);
            if (sourceProperty == null)
            {
                return;
            }    

            IDisposable subscription = sourceProperty.Changed.Subscribe(OnSourcePropValueChanged);

            SetTheSubscription(control, subscription);
        }

        private static void OnSourcePropValueChanged(AvaloniaPropertyChangedEventArgs args)
        {
            Control control = args.Sender as Control;

            AvaloniaProperty sourceProperty = GetSourceProperty(control);

            if (sourceProperty == null)
                return;

            SetTargetValueImpl(control);
        }

        private static void OnTargetPropChanged(AvaloniaPropertyChangedEventArgs<AvaloniaProperty> args)
        {
            SetTargetValueImpl(args.Sender as Control);
        }

        private static void OnValChanged(AvaloniaPropertyChangedEventArgs<object> args)
        {
            SetTargetValueImpl(args.Sender as Control);
        }
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using System;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class SetDynamicResourceBehavior
    {
        #region ResourceKey Attached Avalonia Property
        public static object GetResourceKey(Control obj)
        {
            return obj.GetValue(ResourceKeyProperty);
        }

        public static void SetResourceKey(Control obj, object value)
        {
            obj.SetValue(ResourceKeyProperty, value);
        }

        public static readonly AttachedProperty<object> ResourceKeyProperty =
            AvaloniaProperty.RegisterAttached<object, Control, object>
            (
                "ResourceKey"
            );
        #endregion ResourceKey Attached Avalonia Property


        #region PropertyToSet Attached Avalonia Property
        public static AvaloniaProperty GetPropertyToSet(Control obj)
        {
            return obj.GetValue(PropertyToSetProperty);
        }

        public static void SetPropertyToSet(Control obj, AvaloniaProperty value)
        {
            obj.SetValue(PropertyToSetProperty, value);
        }

        public static readonly AttachedProperty<AvaloniaProperty> PropertyToSetProperty =
            AvaloniaProperty.RegisterAttached<object, Control, AvaloniaProperty>
            (
                "PropertyToSet"
            );
        #endregion PropertyToSet Attached Avalonia Property

        static SetDynamicResourceBehavior()
        {
            ResourceKeyProperty.Changed.Subscribe(OnPropChanged);
            PropertyToSetProperty.Changed.Subscribe(OnPropChanged);
        }

        private static void OnPropChanged(AvaloniaPropertyChangedEventArgs args)
        {
            Control control = (Control) args.Sender;

            SetDynamicResource(control);
        }

        private static void SetDynamicResource(Control control)
        {
            object resourceKey = GetResourceKey(control);
            if (resourceKey == null)
            {
                return;
            }

            AvaloniaProperty ap = GetPropertyToSet(control);

            if (ap == null)
            {
                return;
            }

            DynamicResourceExtension dynamicResourceExtension = new DynamicResourceExtension(resourceKey);

            control.Bind(ap, dynamicResourceExtension);
        }
    }
}

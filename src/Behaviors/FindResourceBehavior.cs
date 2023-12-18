using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using System;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class FindResourceBehavior
    {
        #region ResourceKey Attached Avalonia Property
        public static string GetResourceKey(AvaloniaObject obj)
        {
            return obj.GetValue(ResourceKeyProperty);
        }

        public static void SetResourceKey(AvaloniaObject obj, string value)
        {
            obj.SetValue(ResourceKeyProperty, value);
        }

        public static readonly AttachedProperty<string> ResourceKeyProperty =
            AvaloniaProperty.RegisterAttached<object, Control, string>
            (
                "ResourceKey"
            );
        #endregion ResourceKey Attached Avalonia Property


        #region TheProp Attached Avalonia Property
        public static AvaloniaProperty GetTheProp(AvaloniaObject obj)
        {
            return obj.GetValue(ThePropProperty);
        }

        public static void SetTheProp(AvaloniaObject obj, AvaloniaProperty value)
        {
            obj.SetValue(ThePropProperty, value);
        }

        public static readonly AttachedProperty<AvaloniaProperty> ThePropProperty =
            AvaloniaProperty.RegisterAttached<object, Control, AvaloniaProperty>
            (
                "TheProp"
            );
        #endregion TheProp Attached Avalonia Property


        static FindResourceBehavior()
        {
            ThePropProperty.Changed.Subscribe(OnPropChanged);
            ResourceKeyProperty.Changed.Subscribe(OnResourceKeyChanged);
        }


        private static void OnPropChanged(AvaloniaPropertyChangedEventArgs<AvaloniaProperty> eventArgs)
        {
            Control sender = (Control)eventArgs.Sender;

            sender.SetResourceOnProp();
        }

        private static void OnResourceKeyChanged(AvaloniaPropertyChangedEventArgs<string> eventArgs)
        {
            Control sender = (Control)eventArgs.Sender;

            sender.SetResourceOnProp();
        }

        private static void SetResourceOnProp(this Control sender)
        {
            string resourceKey = GetResourceKey(sender);

            if (resourceKey == null)
                return;

            //object resource = sender.FindResource(resourceKey);

            //if ( (resource == null) || (resource == AvaloniaProperty.UnsetValue))
            //    return;

            AvaloniaProperty prop = GetTheProp(sender);

            sender.Bind(prop, new DynamicResourceExtension(resourceKey));
        }
    }
}

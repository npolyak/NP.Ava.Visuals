using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using System;
using System.Linq;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class FindPartBehavior
    {
        #region VisualPart Attached Avalonia Property
        public static Control GetVisualPart(AvaloniaObject obj)
        {
            return obj.GetValue(VisualPartProperty);
        }

        public static void SetVisualPart(AvaloniaObject obj, Control value)
        {
            obj.SetValue(VisualPartProperty, value);
        }

        public static readonly AttachedProperty<Control> VisualPartProperty =
            AvaloniaProperty.RegisterAttached<object, Control, Control>
            (
                "VisualPart"
            );
        #endregion VisualPart Attached Avalonia Property


        #region AncestorObject Attached Avalonia Property
        public static Control GetAncestorObject(AvaloniaObject obj)
        {
            return obj.GetValue(AncestorObjectProperty);
        }

        public static void SetAncestorObject(AvaloniaObject obj, Control value)
        {
            obj.SetValue(AncestorObjectProperty, value);
        }

        public static readonly AttachedProperty<Control> AncestorObjectProperty =
            AvaloniaProperty.RegisterAttached<object, Control, Control>
            (
                "AncestorObject"
            );
        #endregion AncestorObject Attached Avalonia Property

        static FindPartBehavior()
        {
            AncestorObjectProperty.Changed.Subscribe(OnAncestorObjectChanged);
        }

        private static void OnAncestorObjectChanged(AvaloniaPropertyChangedEventArgs<Control> args)
        {
            Control? ancestor = args.NewValue.Value;

            if (ancestor == null)
                return;

            Control sender = (Control) args.Sender;

            ancestor.SetValue(VisualPartProperty, sender);
        }
    }
}

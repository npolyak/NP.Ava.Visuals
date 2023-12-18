using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using System;
using System.Linq;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class FindLogicalAncestorBehavior
    {
        #region AncestorType Attached Avalonia Property
        public static Type? GetAncestorType(Control obj)
        {
            return obj.GetValue(AncestorTypeProperty);
        }

        public static void SetAncestorType(Control obj, Type? value)
        {
            obj.SetValue(AncestorTypeProperty, value);
        }

        public static readonly AttachedProperty<Type?> AncestorTypeProperty =
            AvaloniaProperty.RegisterAttached<FindLogicalAncestorBehavior, Control, Type?>
            (
                "AncestorType"
            );
        #endregion AncestorType Attached Avalonia Property


        #region AncestorName Attached Avalonia Property
        public static string? GetAncestorName(Control obj)
        {
            return obj.GetValue(AncestorNameProperty);
        }

        public static void SetAncestorName(Control obj, string? value)
        {
            obj.SetValue(AncestorNameProperty, value);
        }

        public static readonly AttachedProperty<string?> AncestorNameProperty =
            AvaloniaProperty.RegisterAttached<FindLogicalAncestorBehavior, Control, string?>
            (
                "AncestorName"
            );
        #endregion AncestorName Attached Avalonia Property


        #region Ancestor Attached Avalonia Property
        public static Control? GetAncestor(Control obj)
        {
            return obj.GetValue(AncestorProperty);
        }

        private static void SetAncestor(Control obj, Control? value)
        {
            obj.SetValue(AncestorProperty, value);
        }

        public static readonly AttachedProperty<Control?> AncestorProperty =
            AvaloniaProperty.RegisterAttached<FindLogicalAncestorBehavior, Control, Control?>
            (
                "Ancestor"
            );
        #endregion Ancestor Attached Avalonia Property


        static FindLogicalAncestorBehavior()
        {
            AncestorTypeProperty.Changed.Subscribe(OnTypeChanged!);
            AncestorNameProperty.Changed.Subscribe(OnNameChanged!);
        }

        private static void OnTypeChanged(AvaloniaPropertyChangedEventArgs<Type> args)
        {
            SetFinding((Control)args.Sender);
        }

        private static void OnNameChanged(AvaloniaPropertyChangedEventArgs<string> args)
        {

            SetFinding((Control)args.Sender);
        }

        private static void SetFinding(Control control)
        {
            control.AttachedToLogicalTree -= Control_AttachedToLogicalTree;

            if (!SetAncestor(control))
            {
                control.AttachedToLogicalTree += Control_AttachedToLogicalTree;
            }
        }

        private static void Control_AttachedToLogicalTree(object? sender, LogicalTreeAttachmentEventArgs e)
        {
            Control control = (Control)sender!;

            SetAncestor(control);
        }

        private static Control? FindAncestor(Control control)
        {
            if ((control as ILogical)?.IsAttachedToLogicalTree != true)
            {
                return null;
            }

            Type? type = GetAncestorType(control);
            string? name = GetAncestorName(control);

            if (type == null && name == null)
            {
                return null;
            }

            Control? result = 
                control.GetLogicalAncestors()
                   .OfType<Control>()
                   .FirstOrDefault
                   (
                    ancestor => 
                        ((type == null) || type.IsAssignableFrom(ancestor.GetType())) && 
                        ((name == null) || (ancestor.Name == name)));

            return result;
        }

        public static bool SetAncestor(Control control)
        {
            var ancestor = FindAncestor(control);

            bool foundAncestor = false;
            if (ancestor != null)
            {
                control.AttachedToLogicalTree -= Control_AttachedToLogicalTree;

                foundAncestor = true;
            }

            SetAncestor(control, ancestor);

            return foundAncestor;
        }
    }
}

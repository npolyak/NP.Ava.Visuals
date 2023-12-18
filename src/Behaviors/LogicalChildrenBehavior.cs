using Avalonia;
using Avalonia.Controls;
using System;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class LogicalChildrenBehavior
    {
        #region TheLogicalChildBehavior Attached Avalonia Property
        public static LogicalChildBehavior GetTheLogicalChildBehavior(AvaloniaObject obj)
        {
            return obj.GetValue(TheLogicalChildBehaviorProperty);
        }

        public static void SetTheLogicalChildBehavior(AvaloniaObject obj, LogicalChildBehavior value)
        {
            obj.SetValue(TheLogicalChildBehaviorProperty, value);
        }

        public static readonly AttachedProperty<LogicalChildBehavior> TheLogicalChildBehaviorProperty =
            AvaloniaProperty.RegisterAttached<object, Control, LogicalChildBehavior>
            (
                "TheLogicalChildBehavior"
            );
        #endregion TheLogicalChildBehavior Attached Avalonia Property

        static LogicalChildrenBehavior()
        {
            TheLogicalChildBehaviorProperty.Changed.Subscribe(OnLogicalChildBehaviorChanged);
        }

        private static void OnLogicalChildBehaviorChanged(AvaloniaPropertyChangedEventArgs<LogicalChildBehavior> change)
        {
            Control control = (Control) change.Sender;

            LogicalChildBehavior oldBehavior = change.OldValue.Value;

            oldBehavior?.Dispose();

            LogicalChildBehavior logicalChildBehavior = change.NewValue.Value;

            if (logicalChildBehavior != null)
            {
                logicalChildBehavior.TheControl = control;
            }
        }
    }
}

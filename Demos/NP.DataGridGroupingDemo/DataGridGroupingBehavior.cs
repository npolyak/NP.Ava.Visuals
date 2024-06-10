using Avalonia;
using Avalonia.Controls;

namespace NP.DataGridGroupingDemo
{
    public class DataGridGroupingBehavior 
    {
        #region CanGroup Attached Avalonia Property
        public static bool GetCanGroup(Control obj)
        {
            return obj.GetValue(CanGroupProperty);
        }

        public static void SetCanGroup(Control obj, bool value)
        {
            obj.SetValue(CanGroupProperty, value);
        }

        public static readonly AttachedProperty<bool> CanGroupProperty =
            AvaloniaProperty.RegisterAttached<DataGridGroupingBehavior, Control, bool>
            (
                "CanGroup"
            );
        #endregion CanGroup Attached Avalonia Property
    }
}

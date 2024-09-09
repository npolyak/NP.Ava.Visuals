using Avalonia;
using Avalonia.Controls;
using System;

namespace NP.Ava.Visuals.Behaviors
{
    public static class GridColumnsBehavior
    {

        #region ColDefsToSet Attached Avalonia Property
        public static ColumnDefinitions GetColDefsToSet(Grid obj)
        {
            return obj.GetValue(ColDefsToSetProperty);
        }

        public static void SetColDefsToSet(Grid obj, ColumnDefinitions value)
        {
            obj.SetValue(ColDefsToSetProperty, value);
        }

        public static readonly AttachedProperty<ColumnDefinitions> ColDefsToSetProperty =
            AvaloniaProperty.RegisterAttached<Grid, Grid, ColumnDefinitions>
            (
                "ColDefsToSet"
            );

        static GridColumnsBehavior()
        {
            ColDefsToSetProperty.Changed.Subscribe(OnColDefsChanged);


        }

        private static void OnColDefsChanged(AvaloniaPropertyChangedEventArgs<ColumnDefinitions> args)
        {
            Grid g = (Grid)args.Sender;

            g.ColumnDefinitions.Clear();

            g.ColumnDefinitions.AddRange(GetColDefsToSet(g));
        }
        #endregion ColDefsToSet Attached Avalonia Property
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Reactive;

namespace NP.Ava.Visuals.Behaviors
{
    public static class LeftOverSizeBehavior
    {
        #region IsSet Attached Avalonia Property
        public static bool GetIsSet(AvaloniaObject obj)
        {
            return obj.GetValue(IsSetProperty);
        }

        public static void SetIsSet(AvaloniaObject obj, bool value)
        {
            obj.SetValue(IsSetProperty, value);
        }

        public static readonly AttachedProperty<bool> IsSetProperty =
            AvaloniaProperty.RegisterAttached<AvaloniaObject, AvaloniaObject, bool>
            (
                "IsSet"
            );
        #endregion IsSet Attached Avalonia Property


        #region ContainerControlSize Attached Avalonia Property
        public static double GetContainerControlSize(Control obj)
        {
            return obj.GetValue(ContainerControlSizeProperty);
        }

        public static void SetContainerControlSize(Control obj, double value)
        {
            obj.SetValue(ContainerControlSizeProperty, value);
        }

        public static readonly AttachedProperty<double> ContainerControlSizeProperty =
            AvaloniaProperty.RegisterAttached<Control, Control, double>
            (
                "ContainerControlSize"
            );
        #endregion ContainerControlSize Attached Avalonia Property


        #region ContainedControlSize Attached Avalonia Property
        public static double GetContainedControlSize(Control obj)
        {
            return obj.GetValue(ContainedControlSizeProperty);
        }

        public static void SetContainedControlSize(Control obj, double value)
        {
            obj.SetValue(ContainedControlSizeProperty, value);
        }

        public static readonly AttachedProperty<double> ContainedControlSizeProperty =
            AvaloniaProperty.RegisterAttached<Control, Control, double>
            (
                "ContainedControlSize"
            );
        #endregion ContainedControlSize Attached Avalonia Property



        #region LeftOverSize Attached Avalonia Property
        public static double GetLeftOverSize(Control obj)
        {
            return obj.GetValue(LeftOverSizeProperty);
        }

        private static void SetLeftOverSize(Control obj, double value)
        {
            obj.SetValue(LeftOverSizeProperty, value);
        }

        public static readonly AttachedProperty<double> LeftOverSizeProperty =
            AvaloniaProperty.RegisterAttached<Control, Control, double>
            (
                "LeftOverSize"
            );
        #endregion LeftOverSize Attached Avalonia Property


        static LeftOverSizeBehavior()
        {
            IsSetProperty.Changed.Subscribe(OnSizeChanged);

            ContainerControlSizeProperty.Changed.Subscribe(OnSizeChanged);

            ContainedControlSizeProperty.Changed.Subscribe(OnSizeChanged);
        }

        private static void OnSizeChanged(AvaloniaPropertyChangedEventArgs args)
        {
            Control currentControl = (Control)args.Sender!;

            bool isSet = GetIsSet(currentControl);

            if (!isSet)
                return;

            var containerControlSize = GetContainerControlSize(currentControl);

            if (containerControlSize == 0 || double.IsNaN(containerControlSize))
                return;

            var containedControlSize = GetContainedControlSize(currentControl);

            if (double.IsNaN(containedControlSize))
                return;

            if (containerControlSize < containedControlSize)
                return;

            SetLeftOverSize(currentControl, containerControlSize - containedControlSize);
        }
    }
}

using Avalonia;
using NP.Ava.Visuals.ThemingAndL10N;
using System;
using Avalonia.Styling;

namespace NP.Ava.Visuals.Behaviors
{
    public class ThemeVariantBehavior
    {
        #region TheThemeVariant Attached Avalonia Property
        public static ThemeVariantEnum GetTheThemeVariant(AvaloniaObject obj)
        {
            return obj.GetValue(TheThemeVariantProperty);
        }

        public static void SetTheThemeVariant(AvaloniaObject obj, ThemeVariantEnum value)
        {
            obj.SetValue(TheThemeVariantProperty, value);
        }

        public static readonly AttachedProperty<ThemeVariantEnum> TheThemeVariantProperty =
            AvaloniaProperty.RegisterAttached<ThemeVariantBehavior, AvaloniaObject, ThemeVariantEnum>
            (
                "TheThemeVariant"
            );
        #endregion TheThemeVariant Attached Avalonia Property

        static ThemeVariantBehavior()
        {
            TheThemeVariantProperty.Changed.Subscribe(OnThemeChanged);
        }

        private static void OnThemeChanged(AvaloniaPropertyChangedEventArgs<ThemeVariantEnum> eventArgs)
        {
            AvaloniaObject sender = eventArgs.Sender;

            if (sender is not IThemeVariantHost)
            {
                throw new Exception("Coding ERROR: ThemeVariantBehavior.TheThemeVariant attached property can only be set on an IThemeVariantHost");
            }

            sender.SetCurrentValue(ThemeVariant.RequestedThemeVariantProperty, GetTheThemeVariant(sender).ToTheme());
        }
    }
}

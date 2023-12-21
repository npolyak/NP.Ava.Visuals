using Avalonia;
using NP.Ava.Visuals.ThemingAndL10N;
using System;
using Avalonia.Styling;

namespace NP.Ava.Visuals.Behaviors
{
    public class ThemeVariantBehavior
    {
        #region TheThemeVariant Attached Avalonia Property
        public static ThemeVariantEnum GetTheThemeVariant(StyledElement obj)
        {
            return obj.GetValue(TheThemeVariantProperty);
        }

        public static void SetTheThemeVariant(StyledElement obj, ThemeVariantEnum value)
        {
            obj.SetValue(TheThemeVariantProperty, value);
        }

        public static readonly AttachedProperty<ThemeVariantEnum> TheThemeVariantProperty =
            AvaloniaProperty.RegisterAttached<ThemeVariantBehavior, StyledElement, ThemeVariantEnum>
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
            StyledElement sender = (StyledElement)eventArgs.Sender;

            sender.SetCurrentValue(ThemeVariant.RequestedThemeVariantProperty, GetTheThemeVariant(sender).ToTheme());
        }
    }
}

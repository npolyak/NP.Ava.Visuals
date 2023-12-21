using Avalonia.Styling;

namespace NP.Ava.Visuals.ThemingAndL10N
{
    public enum ThemeVariantEnum
    {
        Default,
        Light,
        Dark
    }

    public static class ThemeVariantEnumHelper
    {
        public static ThemeVariant ToTheme(this ThemeVariantEnum themeVariant)
        {
            return themeVariant switch
            {
                ThemeVariantEnum.Dark => ThemeVariant.Dark,
                ThemeVariantEnum.Light => ThemeVariant.Light,
                _ => ThemeVariant.Default
            };
        }
    }
}

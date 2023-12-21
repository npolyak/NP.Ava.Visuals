using Avalonia.Data.Converters;
using Avalonia.Styling;
using NP.Ava.Visuals.ThemingAndL10N;
using System;
using System.Globalization;

namespace NP.Ava.Visuals.Converters
{
    public class ThemeVariantConverter : IValueConverter
    {
        public static ThemeVariantConverter Instance { get; } = new ThemeVariantConverter();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is ThemeVariantEnum themeVariant)
            {
                return themeVariant.ToTheme();
            }

            return ThemeVariant.Default;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

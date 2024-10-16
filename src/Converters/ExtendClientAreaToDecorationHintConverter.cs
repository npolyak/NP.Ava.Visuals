using Avalonia.Controls;
using Avalonia.Data.Converters;
using NP.Utilities;
using System;
using System.Globalization;

namespace NP.Ava.Visuals.Converters;

public class ExtendClientAreaToDecorationHintConverter : IValueConverter
{
    public static IValueConverter Instance { get; } = 
        new ExtendClientAreaToDecorationHintConverter();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (!OSUtils.IsWindows) || ((value is WindowState windowState) && (windowState is WindowState.Normal or WindowState.Minimized));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

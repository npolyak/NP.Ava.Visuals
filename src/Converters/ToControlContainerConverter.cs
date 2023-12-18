using Avalonia.Controls;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace NP.Avalonia.Visuals.Converters
{
    public class ToControlContainerConverter : IValueConverter
    {
        public static ToControlContainerConverter Instance { get; } = 
            new ToControlContainerConverter();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            Control? control = value as Control;

            if (control == null)
                return value;

            return new ControlContainer(control);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }
}

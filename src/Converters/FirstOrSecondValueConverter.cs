using Avalonia.Controls;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NP.Ava.Visuals.Converters
{
    public class FirstOrSecondValueConverter : IMultiValueConverter
    {
        public static FirstOrSecondValueConverter Instance { get; } = new FirstOrSecondValueConverter();

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values == null || values.Count < 2) 
                return null;

            if (values[0] is bool b)
            {
                if (b)
                    return values[1];
                else
                {
                    return (values.Count > 2) ? values[2] : null;
                }
            }

            return null;
        }
    }
}

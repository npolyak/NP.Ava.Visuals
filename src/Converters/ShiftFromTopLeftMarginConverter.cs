using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace NP.Ava.Visuals.Converters
{
    public class ShiftFromTopLeftMarginConverter : IValueConverter
    {
        public static ShiftFromTopLeftMarginConverter TheInstance { get; } =
       new ShiftFromTopLeftMarginConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
            {
                return new Thickness(d, d, 0, 0);
            }
            else if (value is Point p)
            {
                return new Thickness(p.X, p.Y, 0, 0);
            }

            return new Thickness();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

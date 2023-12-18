using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace NP.Avalonia.Visuals.Converters
{
    public class ThicknessToDoubleConverter : IValueConverter
    {
        public static ThicknessToDoubleConverter Instance { get; } = new ThicknessToDoubleConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Thickness th)
            {
                return th.Left;
            }

            return 0d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

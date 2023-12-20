using Avalonia.Data.Converters;
using Avalonia.Media;
using NP.Ava.Visuals.ColorUtils;
using System;
using System.Globalization;

namespace NP.Ava.Visuals.Converters
{
    public class InvertColorConverter : IValueConverter
    {
        public static InvertColorConverter Instance { get; } = 
            new InvertColorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = (Color)value;

            return color.Invert();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

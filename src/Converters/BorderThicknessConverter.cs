// (c) Nick Polyak 2021 - http://awebpros.com/
// License: MIT License (https://opensource.org/licenses/MIT)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author of this software if something goes wrong. 
// 
// Also, please, mention this software in any documentation for the 
// products that use it.
//
using Avalonia;
using Avalonia.Data.Converters;
using NP.Utilities;
using System;
using System.Globalization;
using System.Numerics;

namespace NP.Ava.Visuals.Converters
{
    public class BorderThicknessConverter : IValueConverter
    {
        public static BorderThicknessConverter Instance { get; } = 
            new BorderThicknessConverter(new Thickness(1,1,1,1));

        public static BorderThicknessConverter LeftMarginConverter { get; } = 
            new BorderThicknessConverter(new Thickness(1,0,0,0));

        private Thickness ThicknessShape { get; }

        public BorderThicknessConverter(Thickness thicknessShape)
        {
            ThicknessShape = thicknessShape;
        }

        public Thickness Convert(object value)
        {
            if (value is double d)
                return new Thickness(ThicknessShape.Left * d, ThicknessShape.Top * d, ThicknessShape.Right * d, ThicknessShape.Bottom * d);
            if (value is int i)
                return new Thickness(ThicknessShape.Left * i, ThicknessShape.Top * i, ThicknessShape.Right * i, ThicknessShape.Bottom * i);

            return new Thickness();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness result = Convert(value);

            if (parameter is string s)
            {
                if (double.TryParse(s, out double d))
                {
                    return new Thickness(result.Left * d, result.Top * d, result.Right * d, result.Bottom * d);
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

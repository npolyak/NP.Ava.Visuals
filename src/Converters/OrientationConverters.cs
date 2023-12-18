using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using NP.Utilities;
using System;
using System.Globalization;

namespace NP.Avalonia.Visuals.Converters
{
    public class OrientationConverters<T> : IValueConverter
    {
        public T HorizontalValue { get; set; }

        public T VerticalValue { get; set; }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Orientation o)
                return o == Orientation.Horizontal ? HorizontalValue : VerticalValue;

            return VerticalValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (HorizontalValue.ObjEquals(value))
                return Orientation.Horizontal;

            return Orientation.Vertical;
        }
    }

    public class OrientationToDoubleConverter : OrientationConverters<double>
    {

    }

    public class OrientationToHorizontalAlignmentConverter : OrientationConverters<HorizontalAlignment>
    {

    }

    public class OrientationToVerticalAlignmentConverter : OrientationConverters<VerticalAlignment>
    {

    }


    public class OrientationToTransformConverter : OrientationConverters<Transform>
    {

    }
}

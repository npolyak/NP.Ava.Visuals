using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using NP.Utilities.Point;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NP.Ava.Visuals.Converters;

public class WithinControlBoundariesConverter : IMultiValueConverter
{
    public static IMultiValueConverter TheBoundariesConverter { get; } = 
        new WithinControlBoundariesConverter();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values?.Count != 3)
            return null;

        Control boundariesControl = (Control)values[0];

        Rect2D dragBounds = boundariesControl.ToRect().ToRect2D();

        Control dragControl = (Control)values[1];

        PixelPoint dragShift = (PixelPoint)values[2];

        Rect2D dragControlBounds = dragControl.ToRect().Shift(dragShift.ToPoint(1)).ToRect2D();

        (var shift, _) = dragBounds.FitRectangleToRectangle<double>(dragControlBounds);

        return shift.ToPixelPoint();
    }
}

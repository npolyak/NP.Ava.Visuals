using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace NP.Ava.Visuals.Behaviors;

public static class ShiftBehavior
{

    #region Shift Attached Avalonia Property
    public static PixelPoint GetShift(Control obj)
    {
        return obj.GetValue(ShiftProperty);
    }

    public static void SetShift(Control obj, PixelPoint value)
    {
        obj.SetValue(ShiftProperty, value);
    }

    public static readonly AttachedProperty<PixelPoint> ShiftProperty =
        AvaloniaProperty.RegisterAttached<Control, Control, PixelPoint>
        (
            "Shift"
        );
    #endregion Shift Attached Avalonia Property


    static IDisposable _shiftSubscriptionDisposable;
    static ShiftBehavior()
    {
        _shiftSubscriptionDisposable = 
            ShiftProperty.Changed
                         .Subscribe(OnShiftChanged);
    }

    private static void OnShiftChanged(AvaloniaPropertyChangedEventArgs<PixelPoint> args)
    {
        Control control = (Control)args.Sender;

        control.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);
        if (!args.NewValue.HasValue)
        {
            control.RenderTransform = null;
        }
        else
        {
            control.RenderTransform = 
                new TranslateTransform 
                { 
                    X = args.NewValue.Value.X,
                    Y = args.NewValue.Value.Y
                };
        }
            
    }
}

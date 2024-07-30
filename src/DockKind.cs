using Avalonia.Layout;

namespace NP.Ava.Visuals;

public enum DockSide
{
    Center,
    Left,
    Top,
    Right,
    Bottom
}

public static class DockKindHelper
{
    public static Orientation? ToOrientation(this DockSide? dock)
    {
        return dock switch
        {
            DockSide.Left => Orientation.Horizontal,
            DockSide.Right => Orientation.Horizontal,
            DockSide.Top => Orientation.Vertical,
            DockSide.Bottom => Orientation.Vertical,
            _ => null
        };
    }
}

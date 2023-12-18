using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using System;

namespace NP.GridSplitterInOverlayWindowDemo
{
    public class MyGridSplitter : GridSplitter, IStyleable
    {
        Type IStyleable.StyleKey => typeof(GridSplitter);

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            
        }

        protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
        {
            base.OnLostFocus(e);
            base.OnPointerCaptureLost(e);
        }
    }
}

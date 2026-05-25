using Avalonia.Controls;
using Avalonia.Interactivity;

namespace NP.Ava.Visuals.Behaviors.DragDrop;

public class DropEventArgs(Control DragControl, Control DropControl, RoutedEvent routedEvent) : 
    RoutedEventArgs(routedEvent)
{
    public Control DragControl { get; } = DragControl;

    public Control DropControl { get; } = DropControl;
}

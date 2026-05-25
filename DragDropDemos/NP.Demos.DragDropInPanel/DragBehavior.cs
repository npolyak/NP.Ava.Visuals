using Avalonia;
using Avalonia.Controls;
using System;

namespace NP.Ava.Visuals.Behaviors.DragDrop;

public static class DragBehavior
{
    #region DragManagerOnDragControl Attached Avalonia Property
    public static DragManager GetDragManagerOnDragControl(Control obj)
    {
        return obj.GetValue(DragManagerOnDragControlProperty);
    }

    public static void SetDragManagerOnDragControl(Control obj, DragManager value)
    {
        obj.SetValue(DragManagerOnDragControlProperty, value);
    }

    public static readonly AttachedProperty<DragManager> DragManagerOnDragControlProperty =
        AvaloniaProperty.RegisterAttached<Control, Control, DragManager>
        (
            "DragManagerOnDragControl"
        );
    #endregion DragManagerOnDragControl Attached Avalonia Property


    #region DragManagerOnDropControl Attached Avalonia Property
    public static DragManager GetDragManagerOnDropControl(Control obj)
    {
        return obj.GetValue(DragManagerOnDropControlProperty);
    }

    public static void SetDragManagerOnDropControl(Control obj, DragManager value)
    {
        obj.SetValue(DragManagerOnDropControlProperty, value);
    }

    public static readonly AttachedProperty<DragManager> DragManagerOnDropControlProperty =
        AvaloniaProperty.RegisterAttached<Control, Control, DragManager>
        (
            "DragManagerOnDropControl"
        );
    #endregion DragManagerOnDropControl Attached Avalonia Property

    static IDisposable _dragManagerOnDragControlChanged;
    static IDisposable _dragManagerOnDropControlChanged;
    static DragBehavior()
    {
        _dragManagerOnDragControlChanged =
            DragManagerOnDragControlProperty.Changed.Subscribe(OnDragManagerOnDragControlChanged);
        _dragManagerOnDropControlChanged =
            DragManagerOnDropControlProperty.Changed.Subscribe(OnDragManagerOnDropControlChanged);
    }

    private static void OnDragManagerOnDragControlChanged(AvaloniaPropertyChangedEventArgs<DragManager> args)
    {
        if (args.OldValue.HasValue)
        {
            DragManager? oldDragManager = args.OldValue.Value;
            if (oldDragManager != null)
            {
                oldDragManager.DragControl = null;
            }
        }

        if (args.NewValue.HasValue)
        {
            DragManager? newDragManager = args.NewValue.Value;

            if (newDragManager != null)
            {
                Control? dragControl = (Control)args.Sender;
                newDragManager.DragControl = dragControl;
            }
        }
    }

    private static void OnDragManagerOnDropControlChanged(AvaloniaPropertyChangedEventArgs<DragManager> args)
    {
        if (args.OldValue.HasValue)
        {
            DragManager? oldDragManager = args.OldValue.Value;
            if (oldDragManager != null)
            {
                oldDragManager.DropControls.Remove((Control)args.Sender);
            }
        }

        if (args.NewValue.HasValue)
        {
            DragManager? newDragManager = args.NewValue.Value;
            if (newDragManager != null)
            {
                Control? dropControl = (Control)args.Sender;
                newDragManager.DropControls.Add(dropControl);
            }
        }
    }
}

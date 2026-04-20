using Avalonia;
using Avalonia.Data;
using Avalonia.PropertyStore;

namespace NP.Ava.Visuals.MarkupExtensions;

public class BindingExpressionBaseWrapper : BindingExpressionBase
{
    public BindingExpressionBase BindingInstance { get; }

    bool _disposed = false;

    public BindingExpressionBaseWrapper(BindingExpressionBase bindingInstance)
    {
        BindingInstance = bindingInstance;
    }

    public override void Dispose()
    {
        if (!_disposed)
        {
            BindingInstance.Dispose();
            _disposed = true;
        }
    }

    public override void UpdateSource()
    {
        BindingInstance.UpdateSource();
    }


    public override void UpdateTarget()
    {
        BindingInstance.UpdateTarget();
    }

    public override void Attach
    (
        ValueStore valueStore, 
        ImmediateValueFrame? frame, 
        AvaloniaObject target, 
        AvaloniaProperty targetProperty, 
        BindingPriority priority)
    {
        BindingInstance
            .Attach
            (
                valueStore,
                frame,
                target,
                targetProperty,
                priority
            );
    }
}

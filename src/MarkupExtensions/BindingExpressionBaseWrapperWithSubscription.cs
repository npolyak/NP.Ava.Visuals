using Avalonia.Data;
using System;

namespace NP.Ava.Visuals.MarkupExtensions;

public class BindingExpressionBaseWrapperWithSubscription : BindingExpressionBaseWrapper
{
    private IDisposable? _subscription = null;
    public BindingExpressionBaseWrapperWithSubscription
    (
        BindingExpressionBase bindingInstance,
        IDisposable subscription
    ) 
        : 
        base(bindingInstance)
    {
        _subscription = subscription;
    }

    public override void Dispose()
    {
        base.Dispose();

        _subscription?.Dispose();
        _subscription = null;
    }
}

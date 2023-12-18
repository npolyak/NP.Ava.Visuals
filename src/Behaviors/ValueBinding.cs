using Avalonia;
using Avalonia.Data;
using NP.Utilities;
using System;
using System.Reactive.Subjects;

namespace NP.Avalonia.Visuals.Behaviors
{
    //internal class ValueBinding<T> : IBinding
    //{
    //    private T? _value;
    //    public T? Value
    //    {
    //        get => _value;
    //        set
    //        {
    //            if (_value.ObjEquals(value))
    //                return;

    //            _value = value;

    //            if (_value != null)
    //            {
    //                _subject = new BehaviorSubject<object>(_value);
    //            }
    //        }
    //    }

    //    private BehaviorSubject<object>? _subject;

    //    public InstancedBinding Initiate
    //    (
    //        AvaloniaObject target, 
    //        AvaloniaProperty targetProperty, 
    //        object? anchor = null, 
    //        bool enableDataValidation = false)
    //    {
    //        return InstancedBinding.OneWay(_subject, BindingPriority.LocalValue);
    //    }
    //}
}

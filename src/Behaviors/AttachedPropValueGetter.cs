using Avalonia;
using NP.Concepts.Behaviors;
using System;

namespace NP.Ava.Visuals.Behaviors
{
    public class AttachedPropValueGetter<TProp> : IValueGetter<TProp>
    {
        public AvaloniaObject Source { get; }

        private AvaloniaProperty<TProp> _attachedProperty;

        public TProp GetValue() => Source.GetValue<TProp>(_attachedProperty);

        public IObservable<TProp> ValueObservable { get; }

        public AttachedPropValueGetter(AvaloniaObject source, AvaloniaProperty<TProp> attachedProperty)
        {
            Source = source;
            _attachedProperty = attachedProperty;

            ValueObservable = source.GetObservable(_attachedProperty);
        }
    }
}

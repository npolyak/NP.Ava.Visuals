using Avalonia.Controls;
using Avalonia.LogicalTree;
using NP.Concepts.Behaviors;
using System;

namespace NP.Avalonia.Visuals.Behaviors
{
    public abstract class LogicalChildBehavior : IDisposable
    {
        private IDisposable? _subscription = null;
        Control? _control;

        protected internal Control? TheControl 
        {
            get => _control; 
            set
            {
                if (ReferenceEquals(_control, value))
                    return;

                ClearSubscription();

                _control = value;

                if (_control != null)
                {
                    _subscription = _control.GetLogicalChildren().AddBehavior(OnChildAdded, OnChildRemoved);
                }
            }
        }

        private void ClearSubscription()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        public void Dispose()
        {
            TheControl = null;
        }

        protected abstract void OnChildAdded(ILogical child);

        protected abstract void OnChildRemoved(ILogical child);
    }
}

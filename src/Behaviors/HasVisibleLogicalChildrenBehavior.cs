using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using NP.Avalonia.Visuals.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class HasVisibleLogicalChildrenBehavior : LogicalChildBehavior
    {
        private Dictionary<ILogical, IDisposable> _subscriptionDictionary = 
            new Dictionary<ILogical, IDisposable>();

        protected override void OnChildAdded(ILogical childObj)
        {
            Control child = (Control)childObj;

            IDisposable subscriptionToken = 
                child.GetObservable(Visual.IsVisibleProperty).Subscribe(OnIsChildVisibleChanged);

            _subscriptionDictionary[childObj] = subscriptionToken;

            ResetHasVisibleChildren();
        }

        private void OnIsChildVisibleChanged(bool isChildVisible)
        {
            ResetHasVisibleChildren();
        }

        private void ResetHasVisibleChildren()
        {
            bool hasVisibleLogicalChildren = TheControl.GetLogicalChildren().Any(c => (c as Control).IsVisible);

            AttachedProperties.SetHasVisibleLogicalChildren(TheControl, hasVisibleLogicalChildren);
        }

        protected override void OnChildRemoved(ILogical childObj)
        {
            IDisposable subscriptionToken = _subscriptionDictionary[childObj];

            subscriptionToken?.Dispose();

            _subscriptionDictionary.Remove(childObj);

            ResetHasVisibleChildren();
        }
    }
}

using Avalonia.LogicalTree;
using NP.Concepts.Behaviors;

namespace NP.Ava.Visuals.Behaviors
{
    public class ReactiveLogicalDescendantsBehavior : FlattenReactiveTreeBehavior<ILogical>
    {
        public ReactiveLogicalDescendantsBehavior(ILogical root)
            : 
            base(root, logical => logical.LogicalChildren)
        {
        }
    }
}

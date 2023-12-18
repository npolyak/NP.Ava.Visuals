using Avalonia;
using Avalonia.Media;
using NP.Avalonia.Visuals.Behaviors;
using System;

namespace NP.Avalonia.Visuals.Controls
{
    public class HorizontalTransform : Transform
    {
        #region TheVisualFlow Styled Avalonia Property
        public VisualFlow TheVisualFlow
        {
            get { return GetValue(TheVisualFlowProperty); }
            set { SetValue(TheVisualFlowProperty, value); }
        }

        public static readonly StyledProperty<VisualFlow> TheVisualFlowProperty =
            AvaloniaProperty.Register<HorizontalTransform, VisualFlow>
            (
                nameof(TheVisualFlow)
            );
        #endregion TheVisualFlow Styled Avalonia Property

        public HorizontalTransform()
        {
            this.GetObservable(TheVisualFlowProperty).Subscribe(OnVisualFlowChanged);
        }

        public override Matrix Value => Matrix.CreateScale(TheVisualFlow == VisualFlow.Normal ? 1d : -1d, 1d);

        private void OnVisualFlowChanged(VisualFlow visualFlow)
        {
            RaiseChanged();
        }
    }
}

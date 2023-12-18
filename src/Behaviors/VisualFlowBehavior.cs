using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class VisualFlowBehavior
    {

        #region ReadVisualFlow Attached Avalonia Property
        public static VisualFlow GetReadVisualFlow(Control obj)
        {
            return obj.GetValue(ReadVisualFlowProperty);
        }

        internal static void SetReadVisualFlow(Control obj, VisualFlow value)
        {
            obj.SetValue(ReadVisualFlowProperty, value);
        }

        public static readonly AttachedProperty<VisualFlow> ReadVisualFlowProperty =
            AvaloniaProperty.RegisterAttached<object, Control, VisualFlow>
            (
                "ReadVisualFlow",
                VisualFlow.Normal, 
                true
            );
        #endregion ReadVisualFlow Attached Avalonia Property


        #region TheVisualFlow Attached Avalonia Property
        public static VisualFlow GetTheVisualFlow(Control obj)
        {
            return obj.GetValue(TheVisualFlowProperty);
        }

        public static void SetTheVisualFlow(Control obj, VisualFlow value)
        {
            obj.SetValue(TheVisualFlowProperty, value);
        }

        public static readonly AttachedProperty<VisualFlow> TheVisualFlowProperty =
            AvaloniaProperty.RegisterAttached<object, Control, VisualFlow>
            (
                "TheVisualFlow",
                VisualFlow.Normal
            );
        #endregion TheVisualFlow Attached Avalonia Property

        static VisualFlowBehavior()
        {
            TheVisualFlowProperty.Changed.Subscribe(OnVisualFlowChanged);
        }

        private static void CheckTransform(this Control control, bool isNormalFlow)
        {
            if (!isNormalFlow && control.RenderTransform != null && !control.RenderTransform.Value.IsIdentity)
            {
                throw new Exception("Error - VisualFlowBehavior might be ruining the current render transform");
            }
        }

        private static void SetTransform(this Control control, bool isNormalFlow)
        {
            control.CheckTransform(isNormalFlow);

            RelativePoint renderTransformOrigin =
                new RelativePoint(0.5, 0.5, RelativeUnit.Relative);

            if (isNormalFlow)
            {
                control.ClearValue(Visual.RenderTransformOriginProperty);
                control.ClearValue(Visual.RenderTransformProperty);
            }
            else
            {
                ScaleTransform flowTransform = isNormalFlow ? null : new ScaleTransform(-1, 1);

                control.RenderTransformOrigin = renderTransformOrigin;

                control.RenderTransform = flowTransform;
            }
        }

        private static void OnVisualFlowChanged(AvaloniaPropertyChangedEventArgs<VisualFlow> args)
        {
            Control control = (Control) args.Sender;

            VisualFlow visualFlow = args.NewValue.Value;
            bool isNormalFlow = (visualFlow == VisualFlow.Normal);

            control.SetTransform(isNormalFlow);

            SetReadVisualFlow(control, visualFlow);
        }
    }
}

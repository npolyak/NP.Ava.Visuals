using Avalonia;
using Avalonia.Controls;
using NP.Avalonia.Visuals.Behaviors;
using System;

namespace NP.Avalonia.Visuals.Controls
{
    public class ImplantedWindowHostContainer : Decorator
    {
        #region ImplantedWindowHandle Styled Avalonia Property
        public IntPtr ImplantedWindowHandle
        {
            get { return GetValue(ImplantedWindowHandleProperty); }
            set { SetValue(ImplantedWindowHandleProperty, value); }
        }

        public static readonly StyledProperty<IntPtr> ImplantedWindowHandleProperty =
            AvaloniaProperty.Register<ImplantedWindowHostContainer, IntPtr>
            (
                nameof(ImplantedWindowHandle),
                IntPtr.Zero
            );
        #endregion ImplantedWindowHandle Styled Avalonia Property

        #region ParentWindow Property
        private Window? _parentWindow;
        public Window? ParentWindow
        {
            get
            {
                return this._parentWindow;
            }
            private set
            {
                if (this._parentWindow == value)
                {
                    return;
                }

                if (_parentWindow != null)
                {
                    _parentWindow.Closed -= _parentWindow_Closed;
                }

                this._parentWindow = value;


                if (_parentWindow != null)
                {
                    _parentWindow.Closed += _parentWindow_Closed;
                }
            }
        }

        private async void _parentWindow_Closed(object? sender, EventArgs e)
        {
            this.DestroyProcess();
        }
        #endregion ParentWindow Property

        public ImplantedWindowHostContainer()
        {
            this.GetObservable(ImplantedWindowHandleProperty).Subscribe(OnImplantedWindowHandleChanged);
        }

        private void OnImplantedWindowHandleChanged(IntPtr implantedWindowHandle)
        {
            if (implantedWindowHandle != IntPtr.Zero)
            {
                this.Child = new ImplantedWindowHost(implantedWindowHandle);
            }
            else
            {
                this.Child = null;
            }
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            ParentWindow = (Window)e.Root;

            base.OnAttachedToVisualTree(e);
        }

        protected override async void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            if (ParentWindow != null)
            {
                this.DestroyProcess();
            }

            ParentWindow = null;

            base.OnDetachedFromVisualTree(e);
        }
    }
}

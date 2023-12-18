using Avalonia.Controls.Platform;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using NP.Avalonia.Visuals.WindowsOnly;

namespace NP.Avalonia.Visuals.Controls
{
    public class ImplantedWindowHost : NativeControlHost
    {
        private IntPtr _implantedWindowHandle;

        public ImplantedWindowHost(IntPtr implantedWindowHandle)
        {
            _implantedWindowHandle = implantedWindowHandle;
        }

        protected void ImplantWindowAndDestroyPreviousHandle(Window parentWindow)
        {
            parentWindow.ImplantWindow(_implantedWindowHandle);

            // force refreshing the handle
            MethodInfo? methodInfo =
                typeof(NativeControlHost)
                    .GetMethod("DestroyNativeControl", BindingFlags.Instance | BindingFlags.NonPublic);

            methodInfo?.Invoke(this, null);
        }

        bool _firstTime = true;
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            var parentWindow = (Window)e.Root;

            if (_firstTime)
            {
                ImplantWindowAndDestroyPreviousHandle(parentWindow);
                _firstTime = false;
            }

            base.OnAttachedToVisualTree(e);
        }

        protected override async void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
        }

        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new PlatformHandle(_implantedWindowHandle, "CTRL");
            }
            else
            {
                return base.CreateNativeControlCore(parent);
            }
        }

        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            if (control is INativeControlHostDestroyableControlHandle)
            {
                base.DestroyNativeControlCore(control);
            }
        }
    }
}

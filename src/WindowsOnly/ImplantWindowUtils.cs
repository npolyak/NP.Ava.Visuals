using Avalonia.Controls;
using NP.Avalonia.Visuals.Controls;
using System;
using System.Runtime.InteropServices;

namespace NP.Avalonia.Visuals.WindowsOnly
{
    public static class ImplantWindowUtils
    {
        public static void ImplantWindow(this Window parentWindow, nint windowToImplantHandle)
        {
            IntPtr parentWinHandle = parentWindow.GetWinHandle();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // set the parent of the ProcessWindowHandle to be the main window's handle
                //WinApi.SetParent(windowToImplantHandle, parentWinHandle);

                WindowStyles windowToImplantStyle =
                    (WindowStyles)WinApi.GetWindowLongPtr(windowToImplantHandle, (int)WindowLongFlags.GWL_STYLE);


                windowToImplantStyle &= ~WindowStyles.WS_BORDER;
                windowToImplantStyle &= ~WindowStyles.WS_HSCROLL;
                windowToImplantStyle &= ~WindowStyles.WS_VSCROLL;
                windowToImplantStyle &= ~WindowStyles.WS_SIZEFRAME;
                windowToImplantStyle &= ~WindowStyles.WS_POPUP;
                windowToImplantStyle &= ~WindowStyles.WS_TABSTOP;
                windowToImplantStyle &= ~WindowStyles.WS_DISABLED;
                windowToImplantStyle &= ~WindowStyles.WS_SYSMENU;
                windowToImplantStyle &= ~WindowStyles.WS_MINIMIZEBOX;
                windowToImplantStyle &= ~WindowStyles.WS_DLGFRAME;
                windowToImplantStyle &= ~WindowStyles.WS_MAXIMIZEBOX;
                windowToImplantStyle &= ~WindowStyles.WS_CAPTION;
                //windowToImplantStyle |= WindowStyles.WS_CHILD;

                HandleRef handleRef =
                    new HandleRef(null, windowToImplantHandle);

                // set the new style of the schild window
                WinApi.SetWindowLongPtr(handleRef, WindowLongFlags.GWL_STYLE, windowToImplantStyle);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {

            }
        }
    }
}

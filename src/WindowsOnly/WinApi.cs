using System;
using System.Runtime.InteropServices;

namespace NP.Avalonia.Visuals.WindowsOnly
{
    // from http://pinvoke.net/
    public static class WinApi
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong32b(IntPtr hWnd, int nIndex, uint value);


        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLong64b(IntPtr hWnd, int nIndex, IntPtr value);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

        internal static IntPtr SetWindowLongPtr(HandleRef hWnd, WindowLongFlags flag, WindowStyles dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, (int) flag, (IntPtr)dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, (int)flag, ((IntPtr)dwNewLong).ToInt32()));
        }
    }
}

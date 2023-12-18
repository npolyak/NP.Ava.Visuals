namespace NP.Avalonia.Visuals.WindowsOnly
{
    /// <summary>
    /// borrowed from http://pinvoke.net/default.aspx/Enums/WindowLongFlags.html
    /// </summary>
    enum WindowLongFlags : int
    {
        GWL_EXSTYLE = -20,
        GWLP_HINSTANCE = -6,
        GWLP_HWNDPARENT = -8,
        GWL_ID = -12,
        GWL_STYLE = -16,
        GWL_USERDATA = -21,
        GWL_WNDPROC = -4,
        DWLP_USER = 0x8,
        DWLP_MSGRESULT = 0x0,
        DWLP_DLGPROC = 0x4
    }
}

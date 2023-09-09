using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32;

namespace PowerStatus;

static class SafeWindowHandleExtensions
{
    /// <summary>
    /// Politely asks this window to close by posting <c>WM_CLOSE</c> to its message loop.
    /// </summary>
    public static void RequestClose(this SafeWindowHandle window)
    {
        PInvoke.PostMessage(window.Handle, PInvoke.WM_CLOSE, default, default);
    }

    /// <summary>
    /// Synchronously pumps messages for this window.
    /// </summary>
    public static void RunMessageLoop(this SafeWindowHandle window)
    {
        while (true)
        {
            var getMessageResult = PInvoke.GetMessage(out var msg, window.Handle, default, default).Value;
            if (getMessageResult < 0)
                throw new Win32Exception(Marshal.GetLastSystemError());
            if (getMessageResult == 0)
                break;
            if (msg.message == PInvoke.WM_CLOSE)
                break;
            PInvoke.TranslateMessage(in msg);
            PInvoke.DispatchMessage(in msg);
        }
    }
}
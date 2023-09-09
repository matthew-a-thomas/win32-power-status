using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace PowerStatus;

sealed class SafeWindowHandle : SafeOneShotHandle<HWND>
{
    public SafeWindowHandle(HWND window) : base(window)
    { }

    protected override void Release(bool silent, HWND handle)
    {
        if (!PInvoke.DestroyWindow(handle) && !silent)
            throw new Win32Exception(Marshal.GetLastSystemError());
    }
}
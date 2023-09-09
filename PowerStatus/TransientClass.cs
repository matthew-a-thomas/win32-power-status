using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace PowerStatus;

sealed class TransientClass : IDisposable
{
    IntPtr _classNamePointer;
    readonly HMODULE _moduleHandle = PInvoke.GetModuleHandle(default(PCWSTR));
    WNDPROC? _process;

    public TransientClass(WNDPROC process)
    {
        _process = process;
        var className = Guid.NewGuid().ToString();
        _classNamePointer = Marshal.StringToHGlobalAnsi(className);
        ClassAtom = PInvoke.RegisterClassEx(new WNDCLASSEXW
        {
            cbSize = (uint)Unsafe.SizeOf<WNDCLASSEXW>(),
            lpfnWndProc = process,
            hInstance = _moduleHandle,
            lpszClassName = ClassName
        });
        if (ClassAtom == 0)
            throw new Win32Exception(Marshal.GetLastSystemError());
    }

    public ushort ClassAtom { get; }
    public unsafe PCWSTR ClassName => (char*)_classNamePointer;

    ~TransientClass()
    {
        DisposeCore(false);
    }

    public void Dispose()
    {
        DisposeCore(true);
        GC.SuppressFinalize(this);
    }

    unsafe void DisposeCore(bool managed)
    {
        var classNamePointer = Interlocked.Exchange(ref _classNamePointer, IntPtr.Zero);
        if (classNamePointer == IntPtr.Zero)
            return;
        if (!PInvoke.UnregisterClass((char*)classNamePointer, _moduleHandle) && managed)
            throw new Win32Exception(Marshal.GetLastSystemError());
        GC.KeepAlive(_process);
        _process = default;
        Marshal.FreeHGlobal(classNamePointer);
    }
}
using System;
using System.Threading;

namespace PowerStatus;

sealed class Disposable : IDisposable
{
    Action? _callback;

    public Disposable(Action? callback)
    {
        _callback = callback;
    }

    public void Dispose()
    {
        Interlocked.Exchange(ref _callback, null)?.Invoke();
    }
}
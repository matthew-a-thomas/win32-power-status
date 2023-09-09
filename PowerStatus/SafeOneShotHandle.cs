using System;

namespace PowerStatus;

abstract class SafeOneShotHandle<T> : IDisposable
    where T : unmanaged, IEquatable<T>
{
    readonly object _gate = new();

    public T Handle { get; private set; }

    protected SafeOneShotHandle(T handle)
    {
        Handle = handle;
    }

    ~SafeOneShotHandle()
    {
        DisposeCore(false);
    }

    public void Dispose()
    {
        DisposeCore(true);
        GC.SuppressFinalize(this);
    }

    void DisposeCore(bool managed)
    {
        lock (_gate)
        {
            if (Handle.Equals(default))
                return;
            Release(!managed, Handle);
            Handle = default;
        }
    }

    /// <summary>
    /// Release the given <paramref name="handle"/> without throwing any exceptions.
    /// </summary>
    protected abstract void Release(bool silent, T handle);
}
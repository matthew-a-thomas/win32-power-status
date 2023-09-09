using System;
using System.Collections.Generic;

namespace PowerStatus;

/// <summary>
/// Extension methods for <see cref="PowerStatusProvider"/>.
/// </summary>
public static class PowerStatusProviderExtensions
{
    /// <inheritdoc cref="PowerStatusProvider.Subscribe"/>
    public static IDisposable Subscribe(
        this PowerStatusProvider provider,
        Action<Exception> handleUncaughtExceptions,
        params PowerStatusNotification[] notifications) =>
        provider.Subscribe(
            handleUncaughtExceptions,
            // ReSharper disable RedundantCast // Makes it less likely for me to fat finger an infinite loop into existence
            (IEnumerable<PowerStatusNotification>)notifications);
            // ReSharper restore RedundantCast
}
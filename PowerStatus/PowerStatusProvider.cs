using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Power;
using Windows.Win32.UI.WindowsAndMessaging;

namespace PowerStatus;

/// <summary>
/// Provides system power information.
/// </summary>
public sealed class PowerStatusProvider
{
    /// <summary>
    /// Gets a snapshot of the current system power status.
    /// </summary>
    /// <returns>An instance of <see cref="PowerStatus"/>.</returns>
    public PowerStatus GetStatus()
    {
        if (!PInvoke.GetSystemPowerStatus(out var status))
        {
            throw new Win32Exception(Marshal.GetLastSystemError());
        }

        AcLineStatus? acLineStatus = status.ACLineStatus switch
        {
            0 => AcLineStatus.Offline,
            1 => AcLineStatus.Online,
            _ => null
        };
        BatteryFlags? batteryFlags = status.BatteryFlag switch
        {
            byte.MaxValue => null,
            var flags => (BatteryFlags)flags
        };
        double? batteryLifeProportion = status.BatteryLifePercent switch
        {
            var percent and <= 100 => percent / 100.0,
            _ => null
        };
        var batterySaver = status.SystemStatusFlag switch
        {
            0 => false,
            _ => true
        };
        TimeSpan? batteryTime = status.BatteryLifeTime switch
        {
            uint.MaxValue => null,
            var seconds => TimeSpan.FromSeconds(seconds)
        };
        TimeSpan? fullTime = status.BatteryFullLifeTime switch
        {
            uint.MaxValue => null,
            var seconds => TimeSpan.FromSeconds(seconds)
        };

        return new PowerStatus(
            acLineStatus,
            batteryFlags,
            batteryLifeProportion,
            batterySaver,
            batteryTime,
            fullTime
        );
    }

    /// <summary>
    /// Subscribe to receive power status updates. The subscriptions will be active until the returned object is
    /// disposed of.
    /// </summary>
    public unsafe IDisposable Subscribe(
        Action<Exception> handleUncaughtExceptions,
        IEnumerable<PowerStatusNotification> notifications)
    {
        // Create a message-only window, use it to accept WM_* messages related to the relevant power status updates,
        // and pump its message queue in a background thread.
        var gate = new object();
        var stop = false;
        SafeWindowHandle? window = null;
        var powerHandlers = new Dictionary<Guid, List<PowerStatusNotification>>();
        foreach (var notification in notifications)
        {
            if (!powerHandlers.TryGetValue(notification.Guid, out var list))
                powerHandlers[notification.Guid] = list = new List<PowerStatusNotification>(1);
            list.Add(notification);
        }
        var started = new ManualResetEventSlim(false);
        Exception? exception = null;
        new Thread(() =>
        {
            try
            {
                // Apparently all the messages we're interested in get processed by the window's class instead of being
                // received by the message queue for the window itself.
                using var windowClass = new TransientClass((hWnd, msg, wParam, lParam) =>
                {
                    if (msg != PInvoke.WM_POWERBROADCAST || wParam != PInvoke.PBT_POWERSETTINGCHANGE)
                        return PInvoke.DefWindowProc(hWnd, msg, wParam, lParam);
                    var powerBroadcast = (POWERBROADCAST_SETTING*)lParam.Value;
                    var powerSetting = powerBroadcast->PowerSetting;
                    if (!powerHandlers.TryGetValue(powerSetting, out var list))
                        return PInvoke.DefWindowProc(hWnd, msg, wParam, lParam);
                    var data = new ReadOnlySpan<byte>(powerBroadcast->Data.Value, (int)powerBroadcast->DataLength);
                    foreach (var notification in list)
                    {
                        try
                        {
                            notification.AcceptData(data);
                        }
                        catch (Exception e)
                        {
                            handleUncaughtExceptions(e);
                        }
                    }

                    return PInvoke.DefWindowProc(hWnd, msg, wParam, lParam);
                });
                using var _ = window = new SafeWindowHandle(PInvoke.CreateWindowEx(
                    default,
                    windowClass.ClassName,
                    default,
                    default,
                    default,
                    default,
                    default,
                    default,
                    HWND.HWND_MESSAGE,
                    default,
                    default));
                if (window.Handle.IsNull)
                    throw new Win32Exception(Marshal.GetLastSystemError());
                lock (gate)
                {
                    if (stop)
                        return;
                }

                var registrations = new List<SafePowerNotifyRegistration>();
                try
                {
                    foreach (var guid in powerHandlers.Keys)
                    {
                        var localGuid = guid;
                        var registration = new SafePowerNotifyRegistration(PInvoke.RegisterPowerSettingNotification(
                            new HANDLE(window.Handle),
                            &localGuid,
                            (uint)REGISTER_NOTIFICATION_FLAGS.DEVICE_NOTIFY_WINDOW_HANDLE));
                        if (registration.Handle.IsNull)
                            throw new Win32Exception(Marshal.GetLastSystemError());
                        registrations.Add(registration);
                    }

                    // Run the message loop until the returned object is disposed of
                    started.Set();
                    window.RunMessageLoop();
                }
                finally
                {
                    foreach (var registration in registrations)
                    {
                        registration.Dispose();
                    }

                    registrations.Clear();
                }
            }
            catch (Exception e)
            {
                exception = e;
                Trace.WriteLine(e.Message, nameof(PowerStatusProvider));
            }
            finally
            {
                started.Set();
            }
        })
        {
            IsBackground = true,
            Name = nameof(PowerStatusProvider) + "." + nameof(Subscribe)
        }.Start();
        started.Wait();
        if (exception is not null)
            throw exception;
        return new Disposable(() =>
        {
            lock (gate)
            {
                stop = true;
                window?.RequestClose();
            }
        });
    }
}
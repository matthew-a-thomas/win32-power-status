using System;
using System.Diagnostics;
using Windows.Win32;

namespace PowerStatus;

/// <summary>
/// A power status update about which you'd like to be notified.
/// </summary>
public sealed class PowerStatusNotification
{
    readonly AcceptData _acceptData;

    PowerStatusNotification(
        Guid guid,
        AcceptData acceptData)
    {
        _acceptData = acceptData;
        Guid = guid;
    }

    internal Guid Guid { get; }

    internal void AcceptData(ReadOnlySpan<byte> data) => _acceptData(data);

    static void Complain(string message) => Trace.WriteLine(message, nameof(PowerStatusNotification));

    /// <summary>
    /// Creates a new <see cref="PowerStatusNotification"/> that will handle when the remaining battery capacity has
    /// changed. Values are in the inclusive range [0, 1] and represent the proportion of a full battery. Granularity
    /// varies from system to system but the finest granularity is 0.01.
    /// </summary>
    public static PowerStatusNotification BatteryProportionChanged(Action<float> handle) =>
        new(
            PInvoke.GUID_BATTERY_PERCENTAGE_REMAINING,
            data =>
            {
                if (data.Length > 0)
                {
                    var percent = data[0];
                    var proportion = percent / 100.0f;
                    handle(proportion);
                }
                else
                {
                    Complain("Bad data for GUID_BATTERY_PERCENTAGE_REMAINING");
                }
            });

    /// <summary>
    /// Creates a new <see cref="PowerStatusNotification"/> that will handle when battery saver has been turned off or
    /// on in response to changing power conditions. This notification is useful for components that participate in
    /// energy conservation. These applications should register for this notification and save power when battery saver
    /// is on.
    /// </summary>
    public static PowerStatusNotification BatterySaverIsOn(Action<bool> handle) =>
        new(
            PInvoke.GUID_POWER_SAVING_STATUS,
            data =>
            {
                if (data.Length > 0)
                {
                    var batterySaverIsOn = data[0] != 0;
                    handle(batterySaverIsOn);
                }
                else
                {
                    Complain("Bad data for GUID_POWER_SAVING_STATUS");
                }
            });

    /// <summary>
    /// Creates a new <see cref="PowerStatusNotification"/> that will handle when the current monitor's display state
    /// has changed.
    /// </summary>
    public static PowerStatusNotification CurrentMonitorDisplayState(Action<DisplayState> handle) =>
        new(
            PInvoke.GUID_CONSOLE_DISPLAY_STATE,
            data =>
            {
                if (data.Length > 0)
                {
                    var displayState = (DisplayState)data[0];
                    handle(displayState);
                }
                else
                {
                    Complain("Bad data for GUID_CONSOLE_DISPLAY_STATE");
                }
            });

    /// <summary>
    /// Creates a new <see cref="PowerStatusNotification"/> that handles when the state of the lid has changed (open or
    /// closed). The callback won't be called until a lid device is found and its current state is known.
    /// </summary>
    public static PowerStatusNotification LidIsOpen(Action<bool> handle) =>
        new(
            PInvoke.GUID_LIDSWITCH_STATE_CHANGE,
            data =>
            {
                if (data.Length > 0)
                {
                    var lidIsOpen = data[0] != 0;
                    handle(lidIsOpen);
                }
                else
                {
                    Complain("Bad data for GUID_LIDSWITCH_STATE_CHANGE");
                }
            });

    /// <summary>
    /// Creates a new <see cref="PowerStatusNotification"/> that handles when the active power scheme personality has
    /// changed.
    /// </summary>
    public static PowerStatusNotification PowerSchemePersonalityChanged(Action<PowerSchemePersonality> handle) =>
        new(
            PInvoke.GUID_POWERSCHEME_PERSONALITY,
            data =>
            {
                if (data.Length >= 16)
                {
                    var guid = new Guid(data);
                    if (guid == PInvoke.GUID_MAX_POWER_SAVINGS)
                        handle(PowerSchemePersonality.BatterySaver);
                    else if (guid == PInvoke.GUID_TYPICAL_POWER_SAVINGS)
                        handle(PowerSchemePersonality.Automatic);
                    else if (guid == PInvoke.GUID_MIN_POWER_SAVINGS)
                        handle(PowerSchemePersonality.HighPerformance);
                    else
                        Complain($"Unrecognized power scheme personality {guid}");
                }
                else
                {
                    Complain("Bad data for GUID_POWERSCHEME_PERSONALITY");
                }
            });

    /// <summary>
    /// Creates a new <see cref="PowerStatusNotification"/> that will handle when the system power source has changed.
    /// </summary>
    public static PowerStatusNotification PowerSource(Action<PowerSource> handle) =>
        new(
            PInvoke.GUID_ACDC_POWER_SOURCE,
            data =>
            {
                if (data.Length > 0)
                {
                    var powerSource = (PowerSource)data[0];
                    handle(powerSource);
                }
                else
                {
                    Complain("Bad data for GUID_ACDC_POWER_SOURCE");
                }
            });

    /// <summary>
    /// Creates a new <see cref="PowerStatusNotification"/> that handles when the primary system monitor has been
    /// powered on or off. This notification is useful for components that actively render content to the display
    /// device, such as media visualization. These applications should register for this notification and stop rendering
    /// graphics content when the monitor is off to reduce system power consumption.
    /// </summary>
    public static PowerStatusNotification PrimaryMonitorIsOn(Action<bool> handle) =>
        new(
            PInvoke.GUID_MONITOR_POWER_ON,
            data =>
            {
                if (data.Length > 0)
                {
                    var isOn = data[0] != 0;
                    handle(isOn);
                }
                else
                {
                    Complain("Bad data for GUID_MONITOR_POWER_ON");
                }
            });

    /// <summary>
    /// Creates a new <see cref="PowerStatusNotification"/> that handles when the display associated with the
    /// application's session has been powered on or off.
    /// </summary>
    public static PowerStatusNotification SessionDisplayState(Action<DisplayState> handle) =>
        new(
            PInvoke.GUID_SESSION_DISPLAY_STATUS,
            data =>
            {
                if (data.Length > 0)
                {
                    var displayState = (DisplayState)data[0];
                    handle(displayState);
                }
                else
                {
                    Complain("Bad data for GUID_SESSION_DISPLAY_STATUS");
                }
            });

    /// <summary>
    /// Creates a new <see cref="PowerStatusNotification"/> that handles when the system is entering or exiting away
    /// mode.
    /// </summary>
    public static PowerStatusNotification SystemAwayModeChanged(Action<AwayMode> handle) =>
        new(
            PInvoke.GUID_SYSTEM_AWAYMODE,
            data =>
            {
                if (data.Length > 0)
                {
                    var awayMode = (AwayMode)data[0];
                    handle(awayMode);
                }
                else
                {
                    Complain("Bad data for GUID_SYSTEM_AWAYMODE");
                }
            });

    /// <summary>
    /// Creates a new <see cref="PowerStatusNotification"/> that handles when the system is busy. This indicates that
    /// the system will not be moving into an idle state in the near future and that the current time is a good time for
    /// components to perform background or idle tasks that would otherwise prevent the computer from entering an idle
    /// state.
    /// </summary>
    public static PowerStatusNotification SystemIsBusy(Action handle) =>
        new(
            PInvoke.GUID_IDLE_BACKGROUND_TASK,
            _ =>
            {
                // This event sends no data
                handle();
            });

    /// <summary>
    /// Creates a new <see cref="PowerStatusNotification"/> that handles when the user status associated with the
    /// application's session has changed. <c>true</c> means the user is providing input to the session. <c>false</c>
    /// means the user activity timeout has elapsed with no interaction from the user.
    /// </summary>
    /// <param name="handle"></param>
    /// <returns></returns>
    public static PowerStatusNotification UserIsActive(Action<bool> handle) =>
        new(
            PInvoke.GUID_SESSION_USER_PRESENCE,
            data =>
            {
                if (data.Length > 0)
                {
                    var isActive = data[0] == 0;
                    handle(isActive);
                }
                else
                {
                    Complain("Bad data for GUID_SESSION_USER_PRESENCE");
                }
            });
}
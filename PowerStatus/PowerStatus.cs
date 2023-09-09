using System;
// ReSharper disable NotAccessedPositionalProperty.Global

namespace PowerStatus;

/// <summary>
/// A snapshot of the system's current power and battery status.
/// </summary>
/// <param name="AcLineStatus">The AC power status. <c>null</c> if unknown.</param>
/// <param name="BatteryFlags">The battery charge status. <c>null</c> if unknown.</param>
/// <param name="BatteryLifeProportion">
/// A value in the inclusive range [0, 1] representing the proportion of full battery charge remaining. <c>null</c> if
/// status is unknown.
/// </param>
/// <param name="BatterySaver">
/// The status of battery saver. To participate in energy conservation, avoid resource intensive tasks when battery
/// saver is on.
/// </param>
/// <param name="BatteryTime">
/// The estimated time of remaining battery life. <c>null</c> if unknown or connected to AC power.
/// </param>
/// <param name="FullTime">
/// The estimated time of a full battery. <c>null</c> if battery lifetime is unknown or the device is connected to AC
/// power.
/// </param>
public sealed record PowerStatus(
    AcLineStatus? AcLineStatus,
    BatteryFlags? BatteryFlags,
    double? BatteryLifeProportion,
    bool BatterySaver,
    TimeSpan? BatteryTime,
    TimeSpan? FullTime);
using System;
// ReSharper disable UnusedMember.Global

namespace PowerStatus;

/// <summary>
/// Flags for battery charge status.
/// </summary>
[Flags]
public enum BatteryFlags : byte
{
    /// <summary>
    /// The battery capacity is at more than 66 percent.
    /// </summary>
    High = 1,
    /// <summary>
    /// The battery capacity is at less than 33 percent.
    /// </summary>
    Low = 2,
    /// <summary>
    /// The battery capacity is at less than five percent.
    /// </summary>
    Critical = 4,
    /// <summary>
    /// Charging.
    /// </summary>
    Charging = 8,
    /// <summary>
    /// No system battery.
    /// </summary>
    NoSystemBattery = 128,
}
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
namespace PowerStatus;

/// <summary>
/// The current power source.
/// </summary>
public enum PowerSource
{
    /// <summary>
    /// The computer is powered by an AC power source (or similar, such as a laptop powered by a 12V automotive
    /// adapter).
    /// </summary>
    AC = 0,
    /// <summary>
    /// The computer is powered by an onboard battery power source.
    /// </summary>
    DC = 1,
    /// <summary>
    /// The computer is powered by a short-term power source such as a UPS device.
    /// </summary>
    UPS = 2
}
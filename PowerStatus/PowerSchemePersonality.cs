namespace PowerStatus;

/// <summary>
/// The power scheme's personality.
/// </summary>
public enum PowerSchemePersonality
{
    /// <summary>
    /// The scheme is designed to deliver maximum performance at the expense of power consumption savings.
    /// </summary>
    HighPerformance,
    /// <summary>
    /// The scheme is designed to deliver maximum power consumption savings at the expense of system performance and
    /// responsiveness.
    /// </summary>
    BatterySaver,
    /// <summary>
    /// The scheme is designed to automatically balance performance and power consumption savings.
    /// </summary>
    Automatic
}
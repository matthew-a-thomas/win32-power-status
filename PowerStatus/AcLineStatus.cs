namespace PowerStatus;

/// <summary>
/// The AC power status.
/// </summary>
public enum AcLineStatus : byte
{
    /// <summary>
    /// AC power is offline.
    /// </summary>
    Offline = 0,
    /// <summary>
    /// AC power is online.
    /// </summary>
    Online = 1
}
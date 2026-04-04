namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents the zone settings used when the home is in AWAY mode.
/// </summary>
public class AwayConfiguration
{
    /// <summary>
    /// The zone type, for example HEATING or HOT_WATER.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Indicates whether the away configuration should auto-adjust.
    /// </summary>
    public bool? AutoAdjust { get; set; }

    /// <summary>
    /// The configured comfort level when applicable.
    /// </summary>
    public string? ComfortLevel { get; set; }

    /// <summary>
    /// The setting applied while the home is away.
    /// </summary>
    public Setting? Setting { get; set; }
}
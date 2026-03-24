namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents configurable settings for a zone.
/// </summary>
public class ZoneSettings
{
    /// <summary>Target temperature for the zone.</summary>
    public double Temperature { get; set; }

    /// <summary>Mode of the zone (AUTO, MANUAL, OFF).</summary>
    public string Mode { get; set; } = string.Empty;

    /// <summary>Fan setting (if applicable).</summary>
    public bool? Fan { get; set; }
}
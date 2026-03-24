namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents a temperature overlay in a zone.
/// </summary>
public class Overlay
{
    /// <summary>Target temperature for this overlay (°C).</summary>
    public double TargetTemperature { get; set; }

    /// <summary>Overlay mode (e.g., MANUAL, TIMER).</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Overlay end time, if applicable.</summary>
    public DateTime? EndTime { get; set; }
}
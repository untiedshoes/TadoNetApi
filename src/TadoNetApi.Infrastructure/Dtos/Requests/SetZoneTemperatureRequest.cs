namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Request DTO to set the temperature overlay in a zone.
/// </summary>
public class SetZoneTemperatureRequest
{
    /// <summary>Target temperature (°C).</summary>
    public double Temperature { get; set; }

    /// <summary>Overlay type (e.g., MANUAL).</summary>
    public string Type { get; set; } = "MANUAL";

    /// <summary>Optional end time for overlay.</summary>
    public DateTime? EndTime { get; set; }
}
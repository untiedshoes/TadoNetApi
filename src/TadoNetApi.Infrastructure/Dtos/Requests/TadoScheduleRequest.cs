namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Represents a request to set a schedule entry via the Tado API.
/// </summary>
public class TadoScheduleRequest
{
    public string Name { get; set; } = string.Empty;
    public double TargetTemperature { get; set; }
    public string StartTime { get; set; } = string.Empty; // ISO 8601 or HH:mm format
    public string EndTime { get; set; } = string.Empty;   // ISO 8601 or HH:mm format
}
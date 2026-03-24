namespace TadoNetApi.Infrastructure.Dtos.Responses;

/// <summary>
/// Represents a schedule entry returned by the Tado API.
/// </summary>
public class TadoScheduleResponse
{
    public string Name { get; set; } = string.Empty;
    public double TargetTemperature { get; set; }
    public string StartTime { get; set; } = string.Empty; // ISO 8601 or HH:mm format
    public string EndTime { get; set; } = string.Empty;   // ISO 8601 or HH:mm format
    public bool IsActive { get; set; } = false;
}
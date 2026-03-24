namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents a schedule for a zone (Tado heating schedule / program).
/// </summary>
public class Schedule
{
    /// <summary>Name of the schedule program.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Target temperature in the schedule.</summary>
    public double TargetTemperature { get; set; }

    /// <summary>Start time of the schedule.</summary>
    public DateTime StartTime { get; set; }

    /// <summary>End time of the schedule.</summary>
    public DateTime EndTime { get; set; }

    /// <summary>Indicates whether the schedule is active.</summary>
    public bool IsActive { get; set; }

    /// <summary>ID of the zone this schedule belongs to.</summary>
    public int ZoneId { get; set; } 
}
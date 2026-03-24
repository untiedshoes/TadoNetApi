namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents a Zone in the domain.
/// </summary>
public class Zone
{
    /// <summary>The unique identifier of the zone.</summary>
    public int Id { get; set; }

    /// <summary>The name of the zone.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>The type of zone (e.g., HEATING, HOT_WATER).</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>The target temperature for the zone.</summary>
    public double TargetTemperature { get; set; }

    /// <summary>The current measured temperature in the zone.</summary>
    public double CurrentTemperature { get; set; }

    /// <summary>The current humidity in the zone.</summary>
    public double Humidity { get; set; }

    /// <summary>Whether the heating is currently on in the zone.</summary>
    public bool IsHeating { get; set; }
}
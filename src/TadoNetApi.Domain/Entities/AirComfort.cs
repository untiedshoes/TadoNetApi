namespace TadoNetApi.Domain.Entities;

/// <summary>
/// Represents the climate comfort indicators for a home.
/// </summary>
public class AirComfort
{
    /// <summary>
    /// The overall freshness indicator for the home.
    /// </summary>
    public AirComfortFreshness? Freshness { get; set; }

    /// <summary>
    /// Indicates whether air conditioning is currently powered on.
    /// </summary>
    public bool? AcPoweredOn { get; set; }

    /// <summary>
    /// The last time the air conditioning was powered off.
    /// </summary>
    public DateTime? LastAcPowerOff { get; set; }

    /// <summary>
    /// Per-room comfort indicators.
    /// </summary>
    public List<AirComfortComfort>? Comfort { get; set; }
}

/// <summary>
/// Represents the freshness indicator for a home.
/// </summary>
public class AirComfortFreshness
{
    /// <summary>
    /// The freshness value, for example FRESH or FAIR.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// The last time a window was opened, when available.
    /// </summary>
    public DateTime? LastOpenWindow { get; set; }
}

/// <summary>
/// Represents comfort indicators for a specific room.
/// </summary>
public class AirComfortComfort
{
    /// <summary>
    /// The room identifier.
    /// </summary>
    public int? RoomId { get; set; }

    /// <summary>
    /// The temperature comfort level.
    /// </summary>
    public string? TemperatureLevel { get; set; }

    /// <summary>
    /// The humidity comfort level.
    /// </summary>
    public string? HumidityLevel { get; set; }

    /// <summary>
    /// The room's position within the comfort chart.
    /// </summary>
    public AirComfortCoordinate? Coordinate { get; set; }
}

/// <summary>
/// Represents a point in the comfort chart.
/// </summary>
public class AirComfortCoordinate
{
    /// <summary>
    /// The radial coordinate.
    /// </summary>
    public double? Radial { get; set; }

    /// <summary>
    /// The angular coordinate.
    /// </summary>
    public int? Angular { get; set; }
}
using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

/// <summary>
/// Represents the API response for a home's air comfort indicators.
/// </summary>
public class TadoAirComfortResponse
{
    /// <summary>
    /// The overall freshness indicator for the home.
    /// </summary>
    [JsonPropertyName("freshness")]
    public TadoAirComfortFreshnessResponse? Freshness { get; set; }

    /// <summary>
    /// Indicates whether air conditioning is currently powered on.
    /// </summary>
    [JsonPropertyName("acPoweredOn")]
    public bool? AcPoweredOn { get; set; }

    /// <summary>
    /// The last time the air conditioning was powered off.
    /// </summary>
    [JsonPropertyName("lastAcPowerOff")]
    public DateTime? LastAcPowerOff { get; set; }

    /// <summary>
    /// Per-room comfort indicators.
    /// </summary>
    [JsonPropertyName("comfort")]
    public List<TadoAirComfortComfortResponse>? Comfort { get; set; }
}

/// <summary>
/// Represents the freshness indicator in the API response.
/// </summary>
public class TadoAirComfortFreshnessResponse
{
    /// <summary>
    /// The freshness value.
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <summary>
    /// The last time a window was opened.
    /// </summary>
    [JsonPropertyName("lastOpenWindow")]
    public DateTime? LastOpenWindow { get; set; }
}

/// <summary>
/// Represents room-level comfort indicators in the API response.
/// </summary>
public class TadoAirComfortComfortResponse
{
    /// <summary>
    /// The room identifier.
    /// </summary>
    [JsonPropertyName("roomId")]
    public int? RoomId { get; set; }

    /// <summary>
    /// The temperature comfort level.
    /// </summary>
    [JsonPropertyName("temperatureLevel")]
    public string? TemperatureLevel { get; set; }

    /// <summary>
    /// The humidity comfort level.
    /// </summary>
    [JsonPropertyName("humidityLevel")]
    public string? HumidityLevel { get; set; }

    /// <summary>
    /// The room's position within the comfort chart.
    /// </summary>
    [JsonPropertyName("coordinate")]
    public TadoAirComfortCoordinateResponse? Coordinate { get; set; }
}

/// <summary>
/// Represents a room coordinate in the air comfort chart.
/// </summary>
public class TadoAirComfortCoordinateResponse
{
    /// <summary>
    /// The radial coordinate.
    /// </summary>
    [JsonPropertyName("radial")]
    public double? Radial { get; set; }

    /// <summary>
    /// The angular coordinate.
    /// </summary>
    [JsonPropertyName("angular")]
    public int? Angular { get; set; }
}
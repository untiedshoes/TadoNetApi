using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

/// <summary>
/// DTO representing the weather response from the Tado API.
/// </summary>
public class TadoWeatherResponse
{
    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    [JsonPropertyName("humidity")]
    public double? Humidity { get; set; }

    [JsonPropertyName("windSpeed")]
    public double? WindSpeed { get; set; }

    [JsonPropertyName("rain")]
    public double? Rain { get; set; }

    [JsonPropertyName("condition")]
    public string? Condition { get; set; }
}
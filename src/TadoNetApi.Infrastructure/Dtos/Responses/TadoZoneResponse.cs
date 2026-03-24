using System.Text.Json.Serialization;

using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

/// <summary>
/// Represents the raw Zone response from the Tado API.
/// </summary>
public class TadoZoneResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("setting")]
    public TadoZoneSetting? Setting { get; set; }

    [JsonPropertyName("state")]
    public TadoZoneState? State { get; set; }
}

/// <summary>
/// Represents the Zone settings from the Tado API.
/// </summary>
public class TadoZoneSetting
{
    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    [JsonPropertyName("mode")]
    public string? Mode { get; set; }
}

/// <summary>
/// Represents the Zone state from the Tado API.
/// </summary>
public class TadoZoneState
{
    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    [JsonPropertyName("humidity")]
    public double? Humidity { get; set; }

    [JsonPropertyName("isHeating")]
    public bool? IsHeating { get; set; }
}


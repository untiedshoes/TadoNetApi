using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

/// <summary>
/// Represents the API response for zone away-configuration settings.
/// </summary>
public class TadoAwayConfigurationResponse
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("autoAdjust")]
    public bool? AutoAdjust { get; set; }

    [JsonPropertyName("comfortLevel")]
    public string? ComfortLevel { get; set; }

    [JsonPropertyName("setting")]
    public TadoSettingResponse? Setting { get; set; }
}
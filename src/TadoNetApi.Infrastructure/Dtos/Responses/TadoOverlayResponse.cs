using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

/// <summary>
/// Response from Tado API for a zone overlay.
/// </summary>
public class TadoOverlayResponse
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    [JsonPropertyName("endTime")]
    public DateTime? EndTime { get; set; }
}
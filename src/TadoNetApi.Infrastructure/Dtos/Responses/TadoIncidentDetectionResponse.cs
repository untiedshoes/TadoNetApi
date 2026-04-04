using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

/// <summary>
/// Represents the API response for incident detection settings.
/// </summary>
public class TadoIncidentDetectionResponse
{
    /// <summary>
    /// Indicates whether incident detection is enabled.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Indicates whether incident detection is supported.
    /// </summary>
    [JsonPropertyName("supported")]
    public bool? Supported { get; set; }
}
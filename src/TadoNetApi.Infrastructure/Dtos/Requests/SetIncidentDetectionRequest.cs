using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Request payload for updating incident detection.
/// </summary>
public class SetIncidentDetectionRequest
{
    /// <summary>
    /// Indicates whether incident detection should be enabled.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }
}
using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

public class TadoCapabilityResponse
{
    /// <summary>
    /// The type of capability (e.g., HEATING, COOLING)
    /// </summary>
    [JsonPropertyName("type")]
    public string? PurpleType { get; set; }

    /// <summary>
    /// The temperature-related capabilities
    /// </summary>
    [JsonPropertyName("temperatures")]
    public TadoTemperaturesResponse? Temperatures { get; set; }
}
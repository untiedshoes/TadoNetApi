using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

/// <summary>
/// Represents the raw Home response from the Tado API.
/// Mirrors the JSON structure returned by the API.
/// </summary>
public class TadoHomeResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }
}
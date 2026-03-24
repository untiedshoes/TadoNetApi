using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

public class TadoUserResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("homeId")]
    public int HomeId { get; set; }

    [JsonPropertyName("locale")]
    public string? Locale { get; set; }
}
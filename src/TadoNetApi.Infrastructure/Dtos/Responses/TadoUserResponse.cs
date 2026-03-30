using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

public class TadoUserResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("email")]
    public string? Email { get; set; } = null!;

    [JsonPropertyName("homes")]
    public List<TadoHomeResponse> Homes { get; set; } = new();

    [JsonPropertyName("locale")]
    public string? Locale { get; set; } 
}
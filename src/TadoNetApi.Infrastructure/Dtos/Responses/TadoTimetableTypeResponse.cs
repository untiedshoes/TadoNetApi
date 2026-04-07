using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

/// <summary>
/// Represents a timetable type returned by the Tado API.
/// </summary>
public class TadoTimetableTypeResponse
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}
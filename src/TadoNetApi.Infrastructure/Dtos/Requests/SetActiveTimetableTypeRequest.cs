using System.Text.Json.Serialization;
using TadoNetApi.Domain.Entities;

namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Request payload for updating the active timetable type of a zone.
/// </summary>
public sealed class SetActiveTimetableTypeRequest
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Type { get; set; }

    /// <summary>
    /// Maps a domain <see cref="TimetableType"/> into the API request shape.
    /// </summary>
    public static SetActiveTimetableTypeRequest FromDomain(TimetableType timetableType)
    {
        ArgumentNullException.ThrowIfNull(timetableType);

        return new SetActiveTimetableTypeRequest
        {
            Id = timetableType.Id,
            Type = timetableType.Type
        };
    }
}
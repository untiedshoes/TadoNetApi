using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers;

/// <summary>
/// Provides mapping between Tado timetable DTOs and domain timetable entities.
/// </summary>
public static class TimetableMapper
{
    /// <summary>
    /// Maps a timetable type DTO to the domain model.
    /// </summary>
    public static TimetableType ToDomain(this TadoTimetableTypeResponse dto)
        => new()
        {
            Id = dto.Id,
            Type = dto.Type
        };

    /// <summary>
    /// Maps a timetable block DTO to the domain model.
    /// </summary>
    public static TimetableBlock ToDomain(this TadoTimetableBlockResponse dto)
        => new()
        {
            DayType = dto.DayType,
            Start = dto.Start,
            End = dto.End,
            GeolocationOverride = dto.GeolocationOverride,
            Setting = dto.Setting?.ToDomain()
        };
}
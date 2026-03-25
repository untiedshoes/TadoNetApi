using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers;

public static class HomeMapper
{
    /// <summary>
    /// Maps a TadoHomeResponse to a Home domain entity.
    /// </summary>
    /// <param name="response">The TadoHomeResponse to map.</param>
    /// <returns>A Home domain entity.</returns>
    public static Home ToDomain(TadoHomeResponse dto)
    {
        return new Home
        {
            Id = dto.Id,
            Name = dto.Name,
            ShortName = dto.ShortName ?? string.Empty,
            Presence = string.Empty, // Not returned in this endpoint
            Address = dto.TimeZone ?? string.Empty, // temporary mapping
            CountryCode = dto.CountryCode ?? string.Empty,
            IsActive = dto.IsActive,
            OpenWindowDetectionEnabled = dto.OpenWindowDetectionEnabled
        };
    }

    /// <summary>
    /// Maps a collection of TadoHomeResponse to Domain entities.
    /// </summary>
    public static List<Home> ToDomainList(IEnumerable<TadoHomeResponse> dtos)
    {
        return dtos.Select(ToDomain).ToList();
    }
}
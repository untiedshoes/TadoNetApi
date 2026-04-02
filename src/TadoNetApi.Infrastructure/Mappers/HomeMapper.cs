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
            Name = dto.Name
        };
    }

    public static Home ToDomain(TadoHouseResponse dto)
    {
        return new Home
        {
            Id = dto.Id,
            Name = dto.Name
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
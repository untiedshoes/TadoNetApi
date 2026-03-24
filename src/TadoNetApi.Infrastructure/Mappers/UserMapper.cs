using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers;

public static class UserMapper
{
    public static User ToDomain(this TadoUserResponse response)
    {
        return new User
        {
            Id = response.Id,
            Name = response.Name,
            Email = response.Email,
            HomeId = response.HomeId,
            Locale = response.Locale
        };
    }

    /// <summary>
    /// Maps a collection of TadoUserResponse to Domain User entities.
    /// </summary>
    public static List<User> ToDomainList(IEnumerable<TadoUserResponse> dtos)
    {
        return dtos.Select(ToDomain).ToList();
    }
}
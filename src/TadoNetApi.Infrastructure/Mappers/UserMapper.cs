using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Entities.MobileDevice;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Dtos.Responses.MobileDevice;

namespace TadoNetApi.Infrastructure.Mappers;

public static class UserMapper
{
    /// <summary>
    /// Maps a <see cref="TadoUserResponse"/> to a <see cref="User"/> domain entity.
    /// </summary>
    /// <param name="response">The API DTO to map.</param>
    /// <returns>A fully mapped domain <see cref="User"/>.</returns>
    public static User ToDomain(this TadoUserResponse response)
    {
        return new User
        {
            Id = response.Id,
            Name = response.Name,
            Email = response.Email,
            Username = response.Username,
            Homes = response.Homes?.Select(HomeMapper.ToDomain).ToArray(),
            Locale = response.Locale,
            MobileDevices = response.MobileDevices?.Select(md => md.ToDomain()).ToArray()
        };
    }

    /// <summary>
    /// Maps a collection of <see cref="TadoUserResponse"/> to a list of domain <see cref="User"/> entities.
    /// </summary>
    /// <param name="dtos">The list of API DTOs.</param>
    /// <returns>A list of domain <see cref="User"/> entities.</returns>
    public static List<User> ToDomainList(IEnumerable<TadoUserResponse> dtos)
    {
        return dtos.Select(ToDomain).ToList();
    }

    /// <summary>
    /// Maps a <see cref="TadoMobileItemResponse"/> to an <see cref="Item"/> domain entity.
    /// </summary>
    /// <param name="dto">The API DTO to map.</param>
    /// <returns>A mapped <see cref="Item"/>.</returns>
    public static Item ToDomain(this TadoMobileItemResponse dto)
        => new()
        {
            Name = dto.Name,
            Id = dto.Id,
            Settings = dto.Settings?.ToDomain(),
            Location = dto.Location?.ToDomain(),
            MobileDeviceDetails = dto.MobileDeviceDetails?.ToDomain()
        };

    /// <summary>
    /// Maps a <see cref="TadoMobileSettingsResponse"/> to a <see cref="Settings"/> domain entity.
    /// </summary>
    /// <param name="dto">The API DTO to map.</param>
    /// <returns>A mapped <see cref="Settings"/>.</returns>
    public static Settings ToDomain(this TadoMobileSettingsResponse dto)
        => new() { GeoTrackingEnabled = dto.GeoTrackingEnabled };

    /// <summary>
    /// Maps a <see cref="TadoMobileLocationResponse"/> to a <see cref="Location"/> domain entity.
    /// </summary>
    /// <param name="dto">The API DTO to map.</param>
    /// <returns>A mapped <see cref="Location"/>.</returns>
    public static Location ToDomain(this TadoMobileLocationResponse dto)
        => new()
        {
            Stale = dto.Stale,
            AtHome = dto.AtHome,
            BearingFromHome = dto.BearingFromHome?.ToDomain(),
            RelativeDistanceFromHomeFence = dto.RelativeDistanceFromHomeFence
        };

    /// <summary>
    /// Maps a <see cref="TadoMobileBearingFromHomeResponse"/> to a <see cref="BearingFromHome"/> domain entity.
    /// </summary>
    /// <param name="dto">The API DTO to map.</param>
    /// <returns>A mapped <see cref="BearingFromHome"/>.</returns>
    public static BearingFromHome ToDomain(this TadoMobileBearingFromHomeResponse dto)
        => new() { Degrees = dto.Degrees, Radians = dto.Radians };

    /// <summary>
    /// Maps a <see cref="TadoMobileDetailsResponse"/> to a <see cref="Details"/> domain entity.
    /// </summary>
    /// <param name="dto">The API DTO to map.</param>
    /// <returns>A mapped <see cref="Details"/>.</returns>
    public static Details ToDomain(this TadoMobileDetailsResponse dto)
        => new()
        {
            Platform = dto.Platform,
            OsVersion = dto.OsVersion,
            Model = dto.Model,
            Locale = dto.Locale
        };

    /// <summary>
    /// Maps a list of <see cref="TadoMobileItemResponse"/> to a list of <see cref="Item"/> domain entities.
    /// </summary>
    /// <param name="dtos">The list of API DTOs.</param>
    /// <returns>A list of domain <see cref="Item"/> entities.</returns>
    public static List<Item> ToDomainList(this IEnumerable<TadoMobileItemResponse> dtos) =>
        dtos.Select(d => d.ToDomain()).ToList();
}
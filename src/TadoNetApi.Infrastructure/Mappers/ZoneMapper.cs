using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers;

public static partial class ZoneMapper
{
    /// <summary>
    /// Maps a single TadoZoneResponse to a Domain Zone entity.
    /// </summary>
    public static Zone ToDomain(TadoZoneResponse dto)
    {
        return new Zone
        {
            Id = dto.Id,
            Name = dto.Name,
            Type = dto.Type,
            TargetTemperature = dto.Setting?.Temperature ?? 0,
            CurrentTemperature = dto.State?.Temperature ?? 0,
            Humidity = dto.State?.Humidity ?? 0,
            IsHeating = dto.State?.IsHeating ?? false
        };
    }

    /// <summary>
    /// Maps a list of TadoZoneResponse objects to a list of Domain Zone entities.
    /// </summary>
    public static List<Zone> ToDomainList(List<TadoZoneResponse> dtos)
    {
        return dtos.Select(ToDomain).ToList();
    }

    /// <summary>
    /// Maps the ZoneState part of TadoZoneResponse to Domain ZoneState entity.
    /// </summary>
    public static ZoneState ToDomainState(TadoZoneResponse dto)
    {
        if (dto.State == null || dto.Setting == null)
            return new ZoneState();

        return new ZoneState
        {
            Temperature = dto.State.Temperature ?? 0,
            Humidity = dto.State.Humidity ?? 0,
            Power = dto.State.IsHeating.HasValue && dto.State.IsHeating.Value ? "ON" : "OFF",
            OverlayType = dto.Setting.Mode ?? string.Empty,
            OverlayTargetTemperatureType = "MANUAL" // adjust as needed from API
        };
    }
}
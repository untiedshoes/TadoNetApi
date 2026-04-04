using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers;

/// <summary>
/// Provides mapping from air comfort DTOs to domain entities.
/// </summary>
public static class AirComfortMapper
{
    public static AirComfort ToDomain(this TadoAirComfortResponse dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new AirComfort
        {
            Freshness = dto.Freshness?.ToDomain(),
            AcPoweredOn = dto.AcPoweredOn,
            LastAcPowerOff = dto.LastAcPowerOff,
            Comfort = dto.Comfort?.Select(ToDomain).ToList()
        };
    }

    public static AirComfortFreshness ToDomain(this TadoAirComfortFreshnessResponse dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new AirComfortFreshness
        {
            Value = dto.Value,
            LastOpenWindow = dto.LastOpenWindow
        };
    }

    public static AirComfortComfort ToDomain(this TadoAirComfortComfortResponse dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new AirComfortComfort
        {
            RoomId = dto.RoomId,
            TemperatureLevel = dto.TemperatureLevel,
            HumidityLevel = dto.HumidityLevel,
            Coordinate = dto.Coordinate?.ToDomain()
        };
    }

    public static AirComfortCoordinate ToDomain(this TadoAirComfortCoordinateResponse dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new AirComfortCoordinate
        {
            Radial = dto.Radial,
            Angular = dto.Angular
        };
    }
}
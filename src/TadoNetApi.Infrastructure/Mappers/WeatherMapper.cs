using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers;

/// <summary>
/// Maps Tado API weather DTOs to domain entities.
/// </summary>
public static class WeatherMapper
{
    /// <summary>
    /// Maps TadoWeatherResponse to Weather domain entity.
    /// </summary>
    public static Weather ToDomain(TadoWeatherResponse dto)
    {
        return new Weather
        {
            Temperature = dto.Temperature ?? 0,
            Humidity = dto.Humidity ?? 0,
            WindSpeed = dto.WindSpeed ?? 0,
            Rain = dto.Rain ?? 0,
            Condition = dto.Condition ?? string.Empty
        };
    }
}
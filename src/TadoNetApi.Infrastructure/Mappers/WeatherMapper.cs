using System;
using System.Collections.Generic;
using System.Linq;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers
{
    /// <summary>
    /// Provides mapping from TadoWeatherResponse DTO to Weather domain entity.
    /// </summary>
    public static class WeatherMapper
    {
        public static Weather ToDomain(this TadoWeatherResponse dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new Weather
            {
                SolarIntensity = dto.SolarIntensity?.ToDomain(),
                OutsideTemperature = dto.OutsideTemperature?.ToDomain(),
                WeatherState = dto.WeatherState?.ToDomain()
            };
        }

        public static WeatherState ToDomain(this TadoWeatherStateResponse dto)
            => new() { CurrentType = dto.CurrentType, Value = dto.Value, Timestamp = dto.Timestamp };

        public static List<Weather> ToDomainList(this IEnumerable<TadoWeatherResponse> dtos)
            => dtos.Select(ToDomain).ToList();
    }
}
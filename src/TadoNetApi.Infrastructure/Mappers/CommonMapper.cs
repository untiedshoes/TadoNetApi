using System;
using System.Collections.Generic;
using System.Linq;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers
{
    /// <summary>
    /// Provides mapping extensions between remaining Tado API DTOs and domain entities.
    /// </summary>
    public static class CommonMapper
    {
        public static Address ToDomain(this TadoAddressResponse dto)
            => new()
            {
                AddressLine1 = dto.AddressLine1,
                AddressLine2 = dto.AddressLine2,
                ZipCode = dto.ZipCode,
                City = dto.City,
                State = dto.State,
                Country = dto.Country
            };

        public static ContactDetails ToDomain(this TadoContactDetailsResponse dto)
            => new() { Name = dto.Name, Email = dto.Email, Phone = dto.Phone };

        public static Geolocation ToDomain(this TadoGeolocationResponse dto)
            => new() { Latitude = dto.Latitude, Longitude = dto.Longitude };

        public static OutsideTemperature ToDomain(this TadoOutsideTemperatureResponse dto)
            => new()
            {
                Celsius = dto.Celsius,
                Fahrenheit = dto.Fahrenheit,
                Timestamp = dto.Timestamp,
                PurpleType = dto.PurpleType,
                Precision = dto.Precision?.ToDomain()
            };

        public static TemperatureSteps ToDomain(this TadoTemperatureStepsResponse dto)
            => new() { Min = dto.Min, Max = dto.Max, Step = dto.Step };

        public static Temperatures ToDomain(this TadoTemperaturesResponse dto)
            => new() { Celsius = dto.Celsius?.ToDomain(), Fahrenheit = dto.Fahrenheit?.ToDomain() };

        public static List<T> ToDomainList<T, TDto>(this IEnumerable<TDto> dtos, Func<TDto, T> mapper)
            => dtos.Select(mapper).ToList();
    }
}

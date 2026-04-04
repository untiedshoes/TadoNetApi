using System;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers
{
    /// <summary>
    /// Provides mapping from TadoHouseResponse DTO to House domain entity.
    /// </summary>
    public static class HouseMapper
    {
        public static House ToDomain(this TadoHouseResponse dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new House
            {
                Id = dto.Id,
                Name = dto.Name,
                DateTimeZone = dto.DateTimeZone,
                DateCreated = dto.DateCreated,
                TemperatureUnit = dto.TemperatureUnit,
                InstallationCompleted = dto.InstallationCompleted,
                Partner = dto.Partner,
                SimpleSmartScheduleEnabled = dto.SimpleSmartScheduleEnabled,
                AwayRadiusInMeters = dto.AwayRadiusInMeters,
                License = dto.License,
                ChristmasModeEnabled = dto.ChristmasModeEnabled,
                IncidentDetection = dto.IncidentDetection?.ToDomain(),
                ContactDetails = dto.ContactDetails?.ToDomain() ?? new ContactDetails(),
                Address = dto.Address?.ToDomain() ?? new Address(),
                Geolocation = dto.Geolocation?.ToDomain() ?? new Geolocation()
            };
        }

        public static List<House> ToDomainList(this IEnumerable<TadoHouseResponse> dtos)
            => dtos.Select(ToDomain).ToList();
    }
}
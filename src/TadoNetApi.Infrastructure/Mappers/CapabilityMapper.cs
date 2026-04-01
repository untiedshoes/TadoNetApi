using System;
using System.Collections.Generic;
using System.Linq;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers
{
    /// <summary>
    /// Provides mapping between Tado API Capability DTOs and domain entities.
    /// </summary>
    public static class CapabilityMapper
    {
        /// <summary>
        /// Maps a <see cref="TadoCapabilityResponse"/> to a <see cref="Capability"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="Capability"/>.</returns>
        public static Capability ToDomain(this TadoCapabilityResponse dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new Capability
            {
                PurpleType = dto.PurpleType,
                Temperatures = dto.Temperatures?.ToDomain()
            };
        }

        /// <summary>
        /// Maps a collection of <see cref="TadoCapabilityResponse"/> to domain entities.
        /// </summary>
        /// <param name="dtos">The list of API DTOs.</param>
        /// <returns>A list of mapped <see cref="Capability"/> entities.</returns>
        public static List<Capability> ToDomainList(this IEnumerable<TadoCapabilityResponse> dtos)
            => dtos.Select(ToDomain).ToList();
    }
}
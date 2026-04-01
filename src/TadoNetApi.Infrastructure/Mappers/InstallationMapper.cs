using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers
{
    /// <summary>
    /// Provides mapping between Tado API DTOs and domain Installation entities.
    /// </summary>
    public static class InstallationMapper
    {
        /// <summary>
        /// Maps a <see cref="TadoInstallationResponse"/> to an <see cref="Installation"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A fully mapped domain <see cref="Installation"/>.</returns>
        public static Installation ToDomain(this TadoInstallationResponse dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new Installation
            {
                Id = dto.Id,
                CurrentType = dto.CurrentType,
                Revision = dto.Revision,
                State = dto.State,
                Devices = dto.Devices?.Select(d => d.ToDomain()).ToArray() ?? Array.Empty<Device>()
            };
        }

        /// <summary>
        /// Maps a list of <see cref="TadoInstallationResponse"/> to a list of <see cref="Installation"/> domain entities.
        /// </summary>
        /// <param name="dtos">The list of API DTOs.</param>
        /// <returns>A list of domain <see cref="Installation"/> entities.</returns>
        public static List<Installation> ToDomainList(this IEnumerable<TadoInstallationResponse> dtos)
        {
            return dtos.Select(d => d.ToDomain()).ToList();
        }
    }
}
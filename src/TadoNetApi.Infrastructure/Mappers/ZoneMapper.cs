using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers
{
    /// <summary>
    /// Provides mapping between Tado DTOs and domain Zone entities.
    /// </summary>
    public static class ZoneMapper
    {
    /// <summary>
    /// Maps a <see cref="TadoZoneResponse"/> to a <see cref="Zone"/> domain entity.
    /// </summary>
    /// <param name="dto">The API DTO to map.</param>
    /// <returns>A fully mapped domain <see cref="Zone"/>.</returns>
    public static Zone ToDomain(this TadoZoneResponse dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            return new Zone
            {
                Id = dto.Id,
                Name = dto.Name,
                CurrentType = dto.CurrentType,
                DateCreated = dto.DateCreated,
                DeviceTypes = dto.DeviceTypes,
                Devices = dto.Devices?.Select(d => d.ToDomain()).ToArray(),
                ReportAvailable = dto.ReportAvailable,
                SupportsDazzle = dto.SupportsDazzle,
                DazzleEnabled = dto.DazzleEnabled,
                DazzleMode = dto.DazzleMode?.ToDomain(),
                OpenWindowDetection = dto.OpenWindowDetection?.ToDomain()
            };
        }

        /// <summary>
        /// Maps a <see cref="TadoCharacteristicsResponse"/> to a <see cref="Characteristics"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="Characteristics"/>.</returns>
        public static Characteristics ToDomain(this TadoCharacteristicsResponse dto)
            => new() { Capabilities = dto.Capabilities };

        /// <summary>
        /// Maps a <see cref="TadoMountingStateResponse"/> to a <see cref="MountingState"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="MountingState"/>.</returns>
        public static MountingState ToDomain(this TadoMountingStateResponse dto)
            => new() { Value = dto.Value, Timestamp = dto.Timestamp };

        /// <summary>
        /// Maps a <see cref="TadoConnectionStateResponse"/> to a <see cref="ConnectionState"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="ConnectionState"/>.</returns>
        public static ConnectionState ToDomain(this TadoConnectionStateResponse dto)
            => new() { Value = dto.Value, Timestamp = dto.Timestamp };

        /// <summary>
        /// Maps a <see cref="TadoDazzleModeResponse"/> to a <see cref="DazzleMode"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="DazzleMode"/>.</returns>
        public static DazzleMode ToDomain(this TadoDazzleModeResponse dto)
            => new() { Supported = dto.Supported, Enabled = dto.Enabled };

        /// <summary>
        /// Maps a <see cref="TadoOpenWindowDetectionResponse"/> to an <see cref="OpenWindowDetection"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="OpenWindowDetection"/>.</returns>
        public static OpenWindowDetection ToDomain(this TadoOpenWindowDetectionResponse dto)
            => new()
            {
                Supported = dto.Supported ?? false,
                Enabled = dto.Enabled ?? false,
                TimeoutInSeconds = dto.TimeoutInSeconds
            };
    }
}
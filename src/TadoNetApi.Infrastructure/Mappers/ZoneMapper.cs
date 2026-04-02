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

        /// <summary>
        /// Maps a <see cref="TadoEarlyStartResponse"/> to an <see cref="EarlyStart"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A mapped <see cref="EarlyStart"/>.</returns>
        public static EarlyStart ToDomain(this TadoEarlyStartResponse dto)
            => new() { Enabled = dto.Enabled };
    }
}
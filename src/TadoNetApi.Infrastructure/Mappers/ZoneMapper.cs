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

        /// <summary>
        /// Maps a <see cref="TadoZoneControlResponse"/> to a <see cref="ZoneControl"/> domain entity.
        /// </summary>
        public static ZoneControl ToDomain(this TadoZoneControlResponse dto)
            => new()
            {
                Type = dto.Type,
                EarlyStartEnabled = dto.EarlyStartEnabled,
                HeatingCircuit = dto.HeatingCircuit,
                Duties = dto.Duties?.ToDomain()
            };

        /// <summary>
        /// Maps a <see cref="TadoZoneControlDutiesResponse"/> to a <see cref="ZoneControlDuties"/> domain entity.
        /// </summary>
        public static ZoneControlDuties ToDomain(this TadoZoneControlDutiesResponse dto)
            => new()
            {
                Type = dto.Type,
                Driver = dto.Driver?.ToDomain(),
                Drivers = dto.Drivers?.Select(DeviceMapper.ToDomain).ToArray(),
                Leader = dto.Leader?.ToDomain(),
                Leaders = dto.Leaders?.Select(DeviceMapper.ToDomain).ToArray(),
                Ui = dto.Ui?.ToDomain(),
                Uis = dto.Uis?.Select(DeviceMapper.ToDomain).ToArray()
            };

        /// <summary>
        /// Maps a <see cref="TadoDefaultZoneOverlayResponse"/> to a <see cref="DefaultZoneOverlay"/> domain entity.
        /// </summary>
        public static DefaultZoneOverlay ToDomain(this TadoDefaultZoneOverlayResponse dto)
            => new()
            {
                TerminationCondition = dto.TerminationCondition?.ToDomain()
            };
    }
}
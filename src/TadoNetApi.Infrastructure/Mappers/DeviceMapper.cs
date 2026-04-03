using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Mappers
{
    /// <summary>
    /// Provides mapping extensions from Tado API DTOs to domain entities.
    /// </summary>
    public static class DeviceMapper
    {
        /// <summary>
        /// Maps a <see cref="TadoDeviceResponse"/> to a <see cref="Device"/> domain entity.
        /// </summary>
        /// <param name="dto">The API DTO to map.</param>
        /// <returns>A fully mapped domain <see cref="Device"/>.</returns>
        public static Device ToDomain(this TadoDeviceResponse dto) =>
            new Device
            {
                DeviceType = dto.DeviceType,
                SerialNo = dto.SerialNo,
                ShortSerialNo = dto.ShortSerialNo,
                CurrentFwVersion = dto.CurrentFwVersion,
                ConnectionState = dto.ConnectionState?.ToDomain(),
                Characteristics = dto.Characteristics?.ToDomain(),
                Duties = dto.Duties,
                MountingState = dto.MountingState?.ToDomain(),
                BatteryState = dto.BatteryState,
                ChildLockEnabled = dto.ChildLockEnabled
            };

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
        /// Maps a list of <see cref="TadoDeviceResponse"/> to a list of <see cref="Device"/> domain entities.
        /// </summary>
        /// <param name="dtos">The list of API DTOs.</param>
        /// <returns>A list of domain <see cref="Device"/> entities.</returns>
        public static List<Device> ToDomainList(this IEnumerable<TadoDeviceResponse> dtos) =>
            dtos.Select(d => d.ToDomain()).ToList();

        /// <summary>
        /// Maps a <see cref="TadoDeviceListItemResponse"/> to a <see cref="DeviceListEntry"/> domain entity.
        /// </summary>
        public static DeviceListEntry ToDomain(this TadoDeviceListItemResponse dto)
            => new()
            {
                Type = dto.Type,
                Device = dto.Device?.ToDomain(),
                ZoneId = dto.Zone?.Discriminator,
                ZoneDuties = dto.Zone?.Duties
            };

        /// <summary>
        /// Maps a list of <see cref="TadoDeviceListItemResponse"/> to a list of <see cref="DeviceListEntry"/> domain entities.
        /// </summary>
        public static List<DeviceListEntry> ToDomainList(this IEnumerable<TadoDeviceListItemResponse> dtos)
            => dtos.Select(d => d.ToDomain()).ToList();
    }
}
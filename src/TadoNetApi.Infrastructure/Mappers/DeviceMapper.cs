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
        /// Maps a list of <see cref="TadoDeviceResponse"/> to a list of <see cref="Device"/> domain entities.
        /// </summary>
        /// <param name="dtos">The list of API DTOs.</param>
        /// <returns>A list of domain <see cref="Device"/> entities.</returns>
        public static List<Device> ToDomainList(this IEnumerable<TadoDeviceResponse> dtos) =>
            dtos.Select(d => d.ToDomain()).ToList();
    }
}
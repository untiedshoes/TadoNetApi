using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;

namespace TadoNetApi.Application.Services
{
    /// <summary>
    /// Application-level service for devices. Wraps <see cref="IDeviceService"/> for easier consumption.
    /// </summary>
    public class DeviceAppService
    {
        private readonly IDeviceService _deviceService;

        public DeviceAppService(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        /// <summary>
        /// Retrieves all devices for a given home and zone.
        /// </summary>
        public Task<IReadOnlyList<Device>> GetDevicesAsync(int homeId, CancellationToken cancellationToken = default)
            => _deviceService.GetDevicesAsync(homeId, cancellationToken);

        /// <summary>
        /// Retrieves a single device by ID.
        /// </summary>
        public Task<Device> GetDeviceAsync(int homeId, int deviceId, CancellationToken cancellationToken = default)
            => _deviceService.GetDeviceAsync(homeId, deviceId, cancellationToken);

        /// <summary>
        /// Retrieves the temperature offset for a device.
        /// </summary>
        public Task<Temperature> GetZoneTemperatureOffsetAsync(int deviceId, CancellationToken cancellationToken = default)
            => _deviceService.GetZoneTemperatureOffsetAsync(deviceId, cancellationToken);
    }
}
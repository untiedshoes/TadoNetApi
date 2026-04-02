using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Entities.MobileDevice;
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

        /// <summary>
        /// Retrieves all mobile devices registered to the specified home.
        /// </summary>
        public Task<IReadOnlyList<Item>> GetMobileDevicesAsync(int homeId, CancellationToken cancellationToken = default)
            => _deviceService.GetMobileDevicesAsync(homeId, cancellationToken);

        /// <summary>
        /// Retrieves the settings for a specific mobile device.
        /// </summary>
        public Task<Settings> GetMobileDeviceSettingsAsync(int homeId, int mobileDeviceId, CancellationToken cancellationToken = default)
            => _deviceService.GetMobileDeviceSettingsAsync(homeId, mobileDeviceId, cancellationToken);
    }
}
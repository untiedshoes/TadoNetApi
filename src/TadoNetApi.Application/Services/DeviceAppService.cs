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
        /// Retrieves all devices in a home with their zone association when available.
        /// </summary>
        public Task<IReadOnlyList<DeviceListEntry>> GetDeviceListAsync(int homeId, CancellationToken cancellationToken = default)
            => _deviceService.GetDeviceListAsync(homeId, cancellationToken);

        /// <summary>
        /// Retrieves a single device by ID.
        /// </summary>
        public Task<Device> GetDeviceAsync(string deviceId, CancellationToken cancellationToken = default)
            => _deviceService.GetDeviceAsync(deviceId, cancellationToken);

        /// <summary>
        /// Retrieves a single device by legacy numeric ID.
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
        /// Retrieves the measuring device for a zone.
        /// </summary>
        public Task<Device> GetZoneMeasuringDeviceAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _deviceService.GetZoneMeasuringDeviceAsync(homeId, zoneId, cancellationToken);

        /// <summary>
        /// Retrieves a specific mobile device.
        /// </summary>
        public Task<Item> GetMobileDeviceAsync(int homeId, int mobileDeviceId, CancellationToken cancellationToken = default)
            => _deviceService.GetMobileDeviceAsync(homeId, mobileDeviceId, cancellationToken);

        /// <summary>
        /// Retrieves the settings for a specific mobile device.
        /// </summary>
        public Task<Settings> GetMobileDeviceSettingsAsync(int homeId, int mobileDeviceId, CancellationToken cancellationToken = default)
            => _deviceService.GetMobileDeviceSettingsAsync(homeId, mobileDeviceId, cancellationToken);

        /// <summary>
        /// Turns child lock on or off for a device.
        /// </summary>
        public Task<bool> SetDeviceChildLockAsync(Device device, bool enableChildLock, CancellationToken cancellationToken = default)
            => _deviceService.SetDeviceChildLockAsync(device, enableChildLock, cancellationToken);

        /// <summary>
        /// Turns child lock on or off for a device by device ID.
        /// </summary>
        public Task<bool> SetDeviceChildLockAsync(string deviceId, bool enableChildLock, CancellationToken cancellationToken = default)
            => _deviceService.SetDeviceChildLockAsync(deviceId, enableChildLock, cancellationToken);

        /// <summary>
        /// Triggers identify mode ("Say Hi") on a device.
        /// </summary>
        public Task<bool> SayHiAsync(string deviceId, CancellationToken cancellationToken = default)
            => _deviceService.SayHiAsync(deviceId, cancellationToken);

        /// <summary>
        /// Sets the temperature offset in Celsius for a Tado device.
        /// </summary>
        public Task<bool> SetZoneTemperatureOffsetCelsiusAsync(string deviceId, double temperature, CancellationToken cancellationToken = default)
            => _deviceService.SetZoneTemperatureOffsetCelsiusAsync(deviceId, temperature, cancellationToken);
    }
}
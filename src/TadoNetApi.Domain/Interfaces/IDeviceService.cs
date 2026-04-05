using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Entities.MobileDevice;

namespace TadoNetApi.Domain.Interfaces;

/// <summary>
/// Provides methods to interact with Tado Devices at the domain level.
/// </summary>
public interface IDeviceService
{
    /// <summary>
    /// Retrieves all devices for a given zone in a home.
    /// </summary>
    Task<IReadOnlyList<Device>> GetDevicesAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all devices in a home with their zone association when available.
    /// </summary>
    Task<IReadOnlyList<DeviceListEntry>> GetDeviceListAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single device by its Tado device identifier.
    /// </summary>
    Task<Device> GetDeviceAsync(string deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single device by a legacy integer identifier.
    /// The home ID is retained for compatibility but is not required by the upstream API route.
    /// </summary>
    Task<Device> GetDeviceAsync(int homeId, int deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the temperature offset for a device.
    /// </summary>
    Task<Temperature> GetZoneTemperatureOffsetAsync(int deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all mobile devices registered to the specified home.
    /// </summary>
    Task<IReadOnlyList<Item>> GetMobileDevicesAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the measuring device for a zone.
    /// </summary>
    Task<Device> GetZoneMeasuringDeviceAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific mobile device for the specified home.
    /// </summary>
    Task<Item> GetMobileDeviceAsync(int homeId, int mobileDeviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the settings for a specific mobile device.
    /// </summary>
    Task<Settings> GetMobileDeviceSettingsAsync(int homeId, int mobileDeviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Turns child lock on or off for a device.
    /// </summary>
    Task<bool> SetDeviceChildLockAsync(Device device, bool enableChildLock, CancellationToken cancellationToken = default);

    /// <summary>
    /// Turns child lock on or off for a device by device ID.
    /// </summary>
    Task<bool> SetDeviceChildLockAsync(string deviceId, bool enableChildLock, CancellationToken cancellationToken = default);

    /// <summary>
    /// Triggers identify mode ("Say Hi") on a device.
    /// </summary>
    Task<bool> SayHiAsync(string deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the temperature offset in Celsius for a Tado device.
    /// </summary>
    Task<bool> SetZoneTemperatureOffsetCelsiusAsync(string deviceId, double temperature, CancellationToken cancellationToken = default);
}
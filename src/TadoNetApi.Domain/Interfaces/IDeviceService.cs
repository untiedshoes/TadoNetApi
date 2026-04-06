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
    /// <param name="homeId">The ID of the home whose devices should be retrieved.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A read-only list of devices associated with the home.</returns>
    Task<IReadOnlyList<Device>> GetDevicesAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all devices in a home with their zone association when available.
    /// </summary>
    /// <param name="homeId">The ID of the home whose device list should be retrieved.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A read-only list of device entries including zone association when available.</returns>
    Task<IReadOnlyList<DeviceListEntry>> GetDeviceListAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single device by its Tado device identifier.
    /// </summary>
    /// <param name="deviceId">The Tado device identifier.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The requested device.</returns>
    Task<Device> GetDeviceAsync(string deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single device by a legacy integer identifier.
    /// The home ID is retained for compatibility but is not required by the upstream API route.
    /// </summary>
    /// <param name="homeId">The home ID retained for compatibility.</param>
    /// <param name="deviceId">The legacy numeric device identifier.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The requested device.</returns>
    Task<Device> GetDeviceAsync(int homeId, int deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the temperature offset for a device.
    /// </summary>
    /// <param name="deviceId">The numeric device identifier.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The configured temperature offset.</returns>
    Task<Temperature> GetZoneTemperatureOffsetAsync(int deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all mobile devices registered to the specified home.
    /// </summary>
    /// <param name="homeId">The ID of the home whose mobile devices should be retrieved.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A read-only list of mobile devices registered to the home.</returns>
    Task<IReadOnlyList<Item>> GetMobileDevicesAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the measuring device for a zone.
    /// </summary>
    /// <param name="homeId">The ID of the home containing the zone.</param>
    /// <param name="zoneId">The ID of the zone whose measuring device should be retrieved.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The measuring device assigned to the zone.</returns>
    Task<Device> GetZoneMeasuringDeviceAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves a device into an existing zone.
    /// </summary>
    /// <param name="homeId">The ID of the home containing the zone.</param>
    /// <param name="zoneId">The ID of the destination zone.</param>
    /// <param name="deviceSerialNo">The serial number of the device to move.</param>
    /// <param name="force">Optional flag indicating whether the previous zone should be forcibly removed when supported.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns><see langword="true"/> when the command succeeds.</returns>
    Task<bool> MoveDeviceToZoneAsync(int homeId, int zoneId, string deviceSerialNo, bool? force = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets which device should measure temperature and humidity for a zone.
    /// </summary>
    /// <param name="homeId">The ID of the home containing the zone.</param>
    /// <param name="zoneId">The ID of the zone to update.</param>
    /// <param name="deviceSerialNo">The serial number of the device that should become the measuring device.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The measuring device assigned to the zone after the update.</returns>
    Task<Device> SetZoneMeasuringDeviceAsync(int homeId, int zoneId, string deviceSerialNo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific mobile device for the specified home.
    /// </summary>
    /// <param name="homeId">The ID of the home containing the mobile device.</param>
    /// <param name="mobileDeviceId">The ID of the mobile device.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The requested mobile device.</returns>
    Task<Item> GetMobileDeviceAsync(int homeId, int mobileDeviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the settings for a specific mobile device.
    /// </summary>
    /// <param name="homeId">The ID of the home containing the mobile device.</param>
    /// <param name="mobileDeviceId">The ID of the mobile device.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The settings for the requested mobile device.</returns>
    Task<Settings> GetMobileDeviceSettingsAsync(int homeId, int mobileDeviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a specific mobile device from the specified home.
    /// </summary>
    /// <param name="homeId">The ID of the home containing the mobile device.</param>
    /// <param name="mobileDeviceId">The ID of the mobile device to delete.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns><see langword="true"/> when the command succeeds.</returns>
    Task<bool> DeleteMobileDeviceAsync(int homeId, int mobileDeviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the settings for a specific mobile device.
    /// </summary>
    /// <param name="homeId">The ID of the home containing the mobile device.</param>
    /// <param name="mobileDeviceId">The ID of the mobile device to update.</param>
    /// <param name="settings">The settings to apply to the mobile device.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns><see langword="true"/> when the command succeeds.</returns>
    Task<bool> SetMobileDeviceSettingsAsync(int homeId, int mobileDeviceId, Settings settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Turns child lock on or off for a device.
    /// </summary>
    /// <param name="device">The device whose child lock should be updated.</param>
    /// <param name="enableChildLock">True to enable child lock; otherwise false.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns><see langword="true"/> when the command succeeds.</returns>
    Task<bool> SetDeviceChildLockAsync(Device device, bool enableChildLock, CancellationToken cancellationToken = default);

    /// <summary>
    /// Turns child lock on or off for a device by device ID.
    /// </summary>
    /// <param name="deviceId">The Tado device identifier.</param>
    /// <param name="enableChildLock">True to enable child lock; otherwise false.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns><see langword="true"/> when the command succeeds.</returns>
    Task<bool> SetDeviceChildLockAsync(string deviceId, bool enableChildLock, CancellationToken cancellationToken = default);

    /// <summary>
    /// Triggers identify mode ("Say Hi") on a device.
    /// </summary>
    /// <param name="deviceId">The Tado device identifier.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns><see langword="true"/> when the command succeeds.</returns>
    Task<bool> SayHiAsync(string deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the temperature offset in Celsius for a Tado device.
    /// </summary>
    /// <param name="deviceId">The Tado device identifier.</param>
    /// <param name="temperature">The temperature offset in Celsius.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns><see langword="true"/> when the command succeeds.</returns>
    Task<bool> SetZoneTemperatureOffsetCelsiusAsync(string deviceId, double temperature, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the temperature offset in Fahrenheit for a Tado device.
    /// </summary>
    /// <param name="deviceId">The Tado device identifier.</param>
    /// <param name="temperature">The temperature offset in Fahrenheit.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns><see langword="true"/> when the command succeeds.</returns>
    Task<bool> SetZoneTemperatureOffsetFahrenheitAsync(string deviceId, double temperature, CancellationToken cancellationToken = default);
}
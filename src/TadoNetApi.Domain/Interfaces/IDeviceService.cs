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
    /// Retrieves a single device by ID within a zone.
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
    /// Retrieves the settings for a specific mobile device.
    /// </summary>
    Task<Settings> GetMobileDeviceSettingsAsync(int homeId, int mobileDeviceId, CancellationToken cancellationToken = default);
}
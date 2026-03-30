namespace TadoNetApi.Domain.Interfaces;

using TadoNetApi.Domain.Entities;

/// <summary>
/// Service interface for managing devices within zones.
/// </summary>
public interface IDeviceService
{
    /// <summary>
    /// Retrieves all devices in a zone.
    /// </summary>
    Task<List<Device>> GetDevicesAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific device by ID.
    /// </summary>
    Task<Device?> GetDeviceAsync(int homeId, int zoneId, int deviceId, CancellationToken cancellationToken = default);

}
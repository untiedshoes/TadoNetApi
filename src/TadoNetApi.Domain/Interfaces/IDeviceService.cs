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

    /// <summary>
    /// Sets the child lock state for a device.
    /// </summary>
    /// <param name="homeId">Home ID.</param>
    /// <param name="zoneId">Zone ID.</param>
    /// <param name="deviceId">Device ID.</param>
    /// <param name="lockEnabled">True to enable child lock, false to disable.</param>
    Task SetDeviceChildLockAsync(int homeId, int zoneId, int deviceId, bool lockEnabled);
}
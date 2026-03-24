using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;

namespace TadoNetApi.Application.Services;

/// <summary>
/// Application service for managing devices within zones.
/// </summary>
public class DeviceAppService
{
    private readonly IDeviceService _deviceService;

    public DeviceAppService(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    /// <summary>
    /// Retrieves all devices in a specific zone.
    /// </summary>
    public Task<List<Device>> GetDevicesAsync(int homeId, int zoneId) =>
        _deviceService.GetDevicesAsync(homeId, zoneId);

    /// <summary>
    /// Retrieves a specific device by ID.
    /// </summary>
    public Task<Device?> GetDeviceAsync(int homeId, int zoneId, int deviceId) =>
        _deviceService.GetDeviceAsync(homeId, zoneId, deviceId);

    /// <summary>
    /// Sets the child lock state for a device.
    /// </summary>
    public Task SetDeviceChildLockAsync(int homeId, int zoneId, int deviceId, bool lockEnabled) =>
        _deviceService.SetDeviceChildLockAsync(homeId, zoneId, deviceId, lockEnabled);
}
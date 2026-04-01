using TadoNetApi.Domain.Entities;

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
}
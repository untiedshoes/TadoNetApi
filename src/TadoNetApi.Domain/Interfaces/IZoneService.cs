using TadoNetApi.Domain.Entities;

namespace TadoNetApi.Domain.Interfaces;

/// <summary>
/// Service interface for managing zones within homes.
/// </summary>
public interface IZoneService
{
    /// <summary>
    /// Retrieves all zones within a specific home.
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    Task<List<Zone>> GetZonesAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific zone by its ID within a home.
    /// homeId: The ID of the home.
    /// zoneId: The ID of the zone to retrieve.
    /// </summary>
    Task<Zone?> GetZoneAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the current state of a zone.
    /// homeId: The ID of the home.
    /// zoneId: The ID of the zone
    /// </summary>
    Task<ZoneState> GetZoneStateAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the target temperature for a zone.
    /// homeId: The ID of the home.
    /// zoneId: The ID of the zone.
    /// temperature: The target temperature to set for the zone.
    /// </summary>
    Task SetZoneTemperatureAsync(int homeId, int zoneId, double temperature, CancellationToken cancellationToken = default);
}
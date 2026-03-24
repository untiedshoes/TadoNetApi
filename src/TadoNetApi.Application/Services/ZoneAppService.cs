using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;

namespace TadoNetApi.Application.Services;

/// <summary>
/// Application service for managing zones within homes.
/// </summary>
public class ZoneAppService
{
    private readonly IZoneService _zoneService;

    public ZoneAppService(IZoneService zoneService)
    {
        _zoneService = zoneService;
    }

    /// <summary>
    /// Retrieves all zones within a specific home.
    /// </summary>
    public Task<List<Zone>> GetZonesAsync(int homeId) => _zoneService.GetZonesAsync(homeId);

    /// <summary>
    /// Retrieves a specific zone by ID within a home.
    /// </summary>
    public Task<Zone?> GetZoneAsync(int homeId, int zoneId) => _zoneService.GetZoneAsync(homeId, zoneId);

    /// <summary>
    /// Retrieves the current state of a zone.
    /// </summary>
    public Task<ZoneState> GetZoneStateAsync(int homeId, int zoneId) =>
        _zoneService.GetZoneStateAsync(homeId, zoneId);

    /// <summary>
    /// Sets the target temperature for a zone.
    /// </summary>
    public Task SetZoneTemperatureAsync(int homeId, int zoneId, double temperature) =>
        _zoneService.SetZoneTemperatureAsync(homeId, zoneId, temperature);
}
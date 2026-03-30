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
    public Task<List<Zone>> GetZonesAsync(int homeId, CancellationToken cancellationToken) => _zoneService.GetZonesAsync(homeId, cancellationToken);

    /// <summary>
    /// Retrieves a specific zone by ID within a home.
    /// </summary>
    public Task<Zone?> GetZoneAsync(int homeId, int zoneId, CancellationToken cancellationToken) => _zoneService.GetZoneAsync(homeId, zoneId, cancellationToken);

    /// <summary>
    /// Retrieves the current state of a zone.
    /// </summary>
    public Task<ZoneState> GetZoneStateAsync(int homeId, int zoneId, CancellationToken cancellationToken) =>
        _zoneService.GetZoneStateAsync(homeId, zoneId, cancellationToken);

    /// <summary>
    /// Retrieves all zones within a home along with their current state (temperature, humidity, heating status).
    /// </summary>
    public async Task<List<Zone>> GetZonesWithStateAsync(int homeId)
    {
        var zones = await _zoneService.GetZonesAsync(homeId);

        foreach (var zone in zones)
        {
            var state = await _zoneService.GetZoneStateAsync(homeId, zone.Id);

            zone.CurrentTemperature = state.Temperature;
            zone.Humidity = state.Humidity;
            zone.IsHeating = state.Power == "ON";
        }

        return zones;
    }

    /// <summary>
    /// Sets the target temperature for a zone.
    /// </summary>
    public Task SetZoneTemperatureAsync(int homeId, int zoneId, double temperature) =>
        _zoneService.SetZoneTemperatureAsync(homeId, zoneId, temperature);
}
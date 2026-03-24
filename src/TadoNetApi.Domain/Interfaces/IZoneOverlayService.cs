using TadoNetApi.Domain.Entities;

namespace TadoNetApi.Domain.Interfaces;

/// <summary>
/// Service interface for managing zone overlays and schedules.
/// </summary>
public interface IZoneOverlayService
{
    /// <summary>
    /// Sets a manual overlay temperature for a zone.
    /// </summary>
    Task SetZoneTemperatureAsync(int homeId, int zoneId, double temperature, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the current overlay for a zone.
    /// </summary>
    Task<Overlay> GetZoneOverlayAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);
}
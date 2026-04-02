using TadoNetApi.Domain.Entities;

namespace TadoNetApi.Domain.Interfaces
{
    /// <summary>
    /// Defines operations for interacting with zones.
    /// </summary>
    public interface IZoneService
    {
        Task<IReadOnlyList<Zone>> GetZonesAsync(int homeId, CancellationToken cancellationToken = default);
        Task<Zone> GetZoneAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the current state of a zone.
        /// </summary>
        Task<State> GetZoneStateAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches a summary of the zone's current settings and termination conditions.
        /// Returns null if no overlay is active.
        /// </summary> <param name="homeId">The ID of the home.</param>
        /// <param name="zoneId">The ID of the zone.</param>    /// <param name="cancellationToken">Cancellation token.</param> 
        /// <returns>A summary of the zone's current settings and termination conditions, or null if no overlay.</returns>
        Task<ZoneSummary?> GetZoneSummaryAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the capabilities of a zone.
        /// </summary>
        Task<IReadOnlyList<Capability>> GetZoneCapabilitiesAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);
    }
}
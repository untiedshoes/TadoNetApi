using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Enums;

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

        /// <summary>
        /// Fetches the control configuration for a zone, including heating circuit and device duties.
        /// </summary>
        Task<ZoneControl> GetZoneControlAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the default overlay configuration for a zone.
        /// </summary>
        Task<DefaultZoneOverlay> GetDefaultZoneOverlayAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the early start settings for a zone.
        /// </summary>
        Task<EarlyStart> GetEarlyStartAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the settings used for the zone while the home is in AWAY mode.
        /// </summary>
        Task<AwayConfiguration> GetAwayConfigurationAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the day report payload for a zone.
        /// </summary>
        Task<ZoneDayReport> GetZoneDayReportAsync(int homeId, int zoneId, DateOnly? date = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the temperature offset for a zone by using the first associated device serial.
        /// </summary>
        Task<Temperature> GetZoneTemperatureOffsetAsync(Zone zone, CancellationToken cancellationToken = default);

        /// <summary>
        /// Enables or disables the early start mode for a zone.
        /// </summary>
        Task<bool> SetEarlyStartAsync(int homeId, int zoneId, bool enabled, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the heating temperature in Celsius for a zone, keeping it until the next manual change.
        /// </summary>
        Task<ZoneSummary?> SetHeatingTemperatureCelsiusAsync(int homeId, int zoneId, double temperature, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the heating temperature in Celsius for a zone for the specified duration.
        /// </summary>
        Task<ZoneSummary?> SetHeatingTemperatureCelsiusAsync(int homeId, int zoneId, double temperature, DurationModes durationMode, TimeSpan? timer = null, CancellationToken cancellationToken = default);
    }
}
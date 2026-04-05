using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Enums;
using TadoNetApi.Domain.Interfaces;

namespace TadoNetApi.Application.Services
{
    /// <summary>
    /// Provides application-level orchestration for zone operations.
    /// </summary>
    public class ZoneAppService
    {
        private readonly IZoneService _zoneService;

        public ZoneAppService(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        /// <summary>
        /// Retrieves all zones for a home.
        /// </summary>
        /// <param name="homeId">The ID of the home whose zones should be retrieved.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>A read-only list of zones.</returns>
        public Task<IReadOnlyList<Zone>> GetZonesAsync(int homeId, CancellationToken cancellationToken = default)
            => _zoneService.GetZonesAsync(homeId, cancellationToken);

        /// <summary>
        /// Retrieves a single zone definition.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The requested zone.</returns>
        public Task<Zone> GetZoneAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetZoneAsync(homeId, zoneId, cancellationToken);

        /// <summary>
        /// Retrieves the live operating state for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The current zone state.</returns>
        public Task<State> GetZoneStateAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetZoneStateAsync(homeId, zoneId, cancellationToken);

        /// <summary>
        /// Retrieves the current overlay summary for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The current zone summary, or <see langword="null"/> when no overlay is active.</returns>
        public Task<ZoneSummary?> GetZoneSummaryAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetZoneSummaryAsync(homeId, zoneId, cancellationToken);

        /// <summary>
        /// Retrieves the capability descriptors supported by a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>A read-only list of zone capabilities.</returns>
        public Task<IReadOnlyList<Capability>> GetZoneCapabilitiesAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetZoneCapabilitiesAsync(homeId, zoneId, cancellationToken);

        /// <summary>
        /// Retrieves zone control metadata including heating-circuit assignment and duties.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The zone control details.</returns>
        public Task<ZoneControl> GetZoneControlAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetZoneControlAsync(homeId, zoneId, cancellationToken);

        /// <summary>
        /// Retrieves the default termination condition used for device-created overlays.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The default overlay configuration.</returns>
        public Task<DefaultZoneOverlay> GetDefaultZoneOverlayAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetDefaultZoneOverlayAsync(homeId, zoneId, cancellationToken);

        /// <summary>
        /// Retrieves the early start setting for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The early start configuration.</returns>
        public Task<EarlyStart> GetEarlyStartAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetEarlyStartAsync(homeId, zoneId, cancellationToken);

        /// <summary>
        /// Retrieves the settings used when the home is in AWAY mode for the specified zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The away configuration for the zone.</returns>
        public Task<AwayConfiguration> GetAwayConfigurationAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetAwayConfigurationAsync(homeId, zoneId, cancellationToken);

        /// <summary>
        /// Retrieves a typed day report for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="date">An optional report date. When omitted, the current date is used.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The requested day report.</returns>
        public Task<ZoneDayReport> GetZoneDayReportAsync(int homeId, int zoneId, DateOnly? date = null, CancellationToken cancellationToken = default)
            => _zoneService.GetZoneDayReportAsync(homeId, zoneId, date, cancellationToken);

        /// <summary>
        /// Retrieves the temperature offset using the first associated device in a zone.
        /// </summary>
        /// <param name="zone">The zone whose measuring device offset should be resolved.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The resolved temperature offset.</returns>
        public Task<Temperature> GetZoneTemperatureOffsetAsync(Zone zone, CancellationToken cancellationToken = default)
            => _zoneService.GetZoneTemperatureOffsetAsync(zone, cancellationToken);

        /// <summary>
        /// Enables or disables early start for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="enabled">True to enable early start; otherwise false.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns><see langword="true"/> when the command succeeds.</returns>
        public Task<bool> SetEarlyStartAsync(int homeId, int zoneId, bool enabled, CancellationToken cancellationToken = default)
            => _zoneService.SetEarlyStartAsync(homeId, zoneId, enabled, cancellationToken);

        /// <summary>
        /// Creates a new zone and moves the specified devices into it.
        /// </summary>
        /// <param name="homeId">The ID of the home in which the zone should be created.</param>
        /// <param name="zoneType">The zone type, such as HEATING or HOT_WATER.</param>
        /// <param name="deviceSerialNumbers">The device serial numbers to move into the new zone.</param>
        /// <param name="force">Optional flag indicating whether the previous zone should be forcibly removed when supported.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        public Task CreateZoneAsync(int homeId, string zoneType, IReadOnlyList<string> deviceSerialNumbers, bool? force = null, CancellationToken cancellationToken = default)
            => _zoneService.CreateZoneAsync(homeId, zoneType, deviceSerialNumbers, force, cancellationToken);

        /// <summary>
        /// Sets a manual heating overlay that lasts until the next manual change.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="temperature">The target temperature in Celsius.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The resulting zone summary, or <see langword="null"/> when no payload is returned.</returns>
        public Task<ZoneSummary?> SetHeatingTemperatureCelsiusAsync(int homeId, int zoneId, double temperature, CancellationToken cancellationToken = default)
            => _zoneService.SetHeatingTemperatureCelsiusAsync(homeId, zoneId, temperature, cancellationToken);

        /// <summary>
        /// Sets a heating overlay with the specified duration mode.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="temperature">The target temperature in Celsius.</param>
        /// <param name="durationMode">How long the overlay should remain active.</param>
        /// <param name="timer">The timer duration when <paramref name="durationMode"/> is timer-based.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The resulting zone summary, or <see langword="null"/> when no payload is returned.</returns>
        public Task<ZoneSummary?> SetHeatingTemperatureCelsiusAsync(int homeId, int zoneId, double temperature, DurationModes durationMode, TimeSpan? timer = null, CancellationToken cancellationToken = default)
            => _zoneService.SetHeatingTemperatureCelsiusAsync(homeId, zoneId, temperature, durationMode, timer, cancellationToken);
    }
}
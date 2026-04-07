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
        /// Retrieves the active timetable type for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The active timetable type.</returns>
        public Task<TimetableType> GetActiveTimetableTypeAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetActiveTimetableTypeAsync(homeId, zoneId, cancellationToken);

        /// <summary>
        /// Retrieves the timetable types supported by a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The available timetable types.</returns>
        public Task<IReadOnlyList<TimetableType>> GetZoneTimetablesAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.GetZoneTimetablesAsync(homeId, zoneId, cancellationToken);

        /// <summary>
        /// Retrieves a specific timetable type definition for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="timetableTypeId">The timetable type ID to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The requested timetable type.</returns>
        public Task<TimetableType> GetZoneTimetableAsync(int homeId, int zoneId, int timetableTypeId, CancellationToken cancellationToken = default)
            => _zoneService.GetZoneTimetableAsync(homeId, zoneId, timetableTypeId, cancellationToken);

        /// <summary>
        /// Retrieves all timetable blocks for a specific timetable type.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="timetableTypeId">The timetable type ID to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The timetable blocks for the specified timetable type.</returns>
        public Task<IReadOnlyList<TimetableBlock>> GetZoneTimetableBlocksAsync(int homeId, int zoneId, int timetableTypeId, CancellationToken cancellationToken = default)
            => _zoneService.GetZoneTimetableBlocksAsync(homeId, zoneId, timetableTypeId, cancellationToken);

        /// <summary>
        /// Retrieves the timetable blocks for a specific day type.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="timetableTypeId">The timetable type ID to inspect.</param>
        /// <param name="dayType">The day type to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The timetable blocks for the specified day type.</returns>
        public Task<IReadOnlyList<TimetableBlock>> GetTimetableBlocksByDayTypeAsync(int homeId, int zoneId, int timetableTypeId, string dayType, CancellationToken cancellationToken = default)
            => _zoneService.GetTimetableBlocksByDayTypeAsync(homeId, zoneId, timetableTypeId, dayType, cancellationToken);

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
        /// Updates the open window detection settings for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="settings">The open window detection settings to apply.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        public Task SetOpenWindowDetectionAsync(int homeId, int zoneId, OpenWindowDetection settings, CancellationToken cancellationToken = default)
            => _zoneService.SetOpenWindowDetectionAsync(homeId, zoneId, settings, cancellationToken);

        /// <summary>
        /// Activates the open window state for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        public Task ActivateOpenWindowAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.ActivateOpenWindowAsync(homeId, zoneId, cancellationToken);

        /// <summary>
        /// Resets the open window state for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        public Task ResetOpenWindowAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.ResetOpenWindowAsync(homeId, zoneId, cancellationToken);

        /// <summary>
        /// Updates the writable details of a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="zoneDetails">The writable zone details payload.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The updated zone definition.</returns>
        public Task<Zone> SetZoneDetailsAsync(int homeId, int zoneId, Zone zoneDetails, CancellationToken cancellationToken = default)
            => _zoneService.SetZoneDetailsAsync(homeId, zoneId, zoneDetails, cancellationToken);

        /// <summary>
        /// Updates the default overlay configuration for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="defaultOverlay">The default overlay configuration to apply.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The updated default overlay configuration.</returns>
        public Task<DefaultZoneOverlay> SetDefaultZoneOverlayAsync(int homeId, int zoneId, DefaultZoneOverlay defaultOverlay, CancellationToken cancellationToken = default)
            => _zoneService.SetDefaultZoneOverlayAsync(homeId, zoneId, defaultOverlay, cancellationToken);

        /// <summary>
        /// Applies manual overlays to multiple zones in a single home-level command.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zones.</param>
        /// <param name="zoneOverlays">The set of zone IDs and overlay payloads to apply.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        public Task SetZoneOverlaysAsync(int homeId, IReadOnlyDictionary<int, Overlay> zoneOverlays, CancellationToken cancellationToken = default)
            => _zoneService.SetZoneOverlaysAsync(homeId, zoneOverlays, cancellationToken);

        /// <summary>
        /// Deletes the currently active manual overlays for multiple zones in a single home-level command.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zones.</param>
        /// <param name="zoneIds">The IDs of the zones whose overlays should be deleted.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        public Task DeleteZoneOverlaysAsync(int homeId, IReadOnlyList<int> zoneIds, CancellationToken cancellationToken = default)
            => _zoneService.DeleteZoneOverlaysAsync(homeId, zoneIds, cancellationToken);

        /// <summary>
        /// Updates the settings used for a zone while the home is in AWAY mode.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="awayConfiguration">The away configuration to apply.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        public Task SetAwayConfigurationAsync(int homeId, int zoneId, AwayConfiguration awayConfiguration, CancellationToken cancellationToken = default)
            => _zoneService.SetAwayConfigurationAsync(homeId, zoneId, awayConfiguration, cancellationToken);

        /// <summary>
        /// Updates the active timetable type for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="timetableType">The timetable type to activate.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The active timetable type returned by the API.</returns>
        public Task<TimetableType> SetActiveTimetableTypeAsync(int homeId, int zoneId, TimetableType timetableType, CancellationToken cancellationToken = default)
            => _zoneService.SetActiveTimetableTypeAsync(homeId, zoneId, timetableType, cancellationToken);

        /// <summary>
        /// Updates the timetable blocks for a specific day type.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="timetableTypeId">The timetable type ID to update.</param>
        /// <param name="dayType">The day type to update.</param>
        /// <param name="blocks">The timetable blocks to apply.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The persisted timetable blocks returned by the API.</returns>
        public Task<IReadOnlyList<TimetableBlock>> SetTimetableBlocksForDayTypeAsync(int homeId, int zoneId, int timetableTypeId, string dayType, IReadOnlyList<TimetableBlock> blocks, CancellationToken cancellationToken = default)
            => _zoneService.SetTimetableBlocksForDayTypeAsync(homeId, zoneId, timetableTypeId, dayType, blocks, cancellationToken);

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
        /// Deletes the currently active manual overlay for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone whose overlay should be deleted.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns><see langword="true"/> when the command succeeds.</returns>
        public Task<bool> DeleteZoneOverlayAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.DeleteZoneOverlayAsync(homeId, zoneId, cancellationToken);

        /// <summary>
        /// Assigns the zone to a specific heating circuit or removes the assignment.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="circuitNumber">The heating circuit number to assign, or <see langword="null"/> to remove the assignment.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The updated zone control details.</returns>
        public Task<ZoneControl> SetHeatingCircuitAsync(int homeId, int zoneId, int? circuitNumber, CancellationToken cancellationToken = default)
            => _zoneService.SetHeatingCircuitAsync(homeId, zoneId, circuitNumber, cancellationToken);

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
        /// Sets a manual heating overlay in Fahrenheit that lasts until the next manual change.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="temperature">The target temperature in Fahrenheit.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The resulting zone summary, or <see langword="null"/> when no payload is returned.</returns>
        public Task<ZoneSummary?> SetHeatingTemperatureFahrenheitAsync(int homeId, int zoneId, double temperature, CancellationToken cancellationToken = default)
            => _zoneService.SetHeatingTemperatureFahrenheitAsync(homeId, zoneId, temperature, cancellationToken);

        /// <summary>
        /// Sets a manual hot water overlay in Celsius that lasts until the next manual change.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="temperature">The target temperature in Celsius.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The resulting zone summary, or <see langword="null"/> when no payload is returned.</returns>
        public Task<ZoneSummary?> SetHotWaterTemperatureCelsiusAsync(int homeId, int zoneId, double temperature, CancellationToken cancellationToken = default)
            => _zoneService.SetHotWaterTemperatureCelsiusAsync(homeId, zoneId, temperature, cancellationToken);

        /// <summary>
        /// Sets a manual hot water overlay in Fahrenheit that lasts until the next manual change.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="temperature">The target temperature in Fahrenheit.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The resulting zone summary, or <see langword="null"/> when no payload is returned.</returns>
        public Task<ZoneSummary?> SetHotWaterTemperatureFahrenheitAsync(int homeId, int zoneId, double temperature, CancellationToken cancellationToken = default)
            => _zoneService.SetHotWaterTemperatureFahrenheitAsync(homeId, zoneId, temperature, cancellationToken);

        /// <summary>
        /// Switches heating off for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The resulting zone summary, or <see langword="null"/> when no payload is returned.</returns>
        public Task<ZoneSummary?> SwitchHeatingOffAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.SwitchHeatingOffAsync(homeId, zoneId, cancellationToken);

        /// <summary>
        /// Switches hot water off for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The resulting zone summary, or <see langword="null"/> when no payload is returned.</returns>
        public Task<ZoneSummary?> SwitchHotWaterOffAsync(int homeId, int zoneId, CancellationToken cancellationToken = default)
            => _zoneService.SwitchHotWaterOffAsync(homeId, zoneId, cancellationToken);

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
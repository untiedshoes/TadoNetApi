using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Enums;

namespace TadoNetApi.Domain.Interfaces
{
    /// <summary>
    /// Defines operations for interacting with zones.
    /// </summary>
    public interface IZoneService
    {
        /// <summary>
        /// Retrieves all zones for a home.
        /// </summary>
        /// <param name="homeId">The ID of the home whose zones should be retrieved.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>A read-only list of zones.</returns>
        Task<IReadOnlyList<Zone>> GetZonesAsync(int homeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a single zone definition.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The requested zone.</returns>
        Task<Zone> GetZoneAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the current state of a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The current zone state.</returns>
        Task<State> GetZoneStateAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches a summary of the zone's current settings and termination conditions.
        /// Returns null if no overlay is active.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>A summary of the zone's current settings and termination conditions, or null if no overlay.</returns>
        Task<ZoneSummary?> GetZoneSummaryAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the capabilities of a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>A read-only list of zone capabilities.</returns>
        Task<IReadOnlyList<Capability>> GetZoneCapabilitiesAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the control configuration for a zone, including heating circuit and device duties.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The zone control details.</returns>
        Task<ZoneControl> GetZoneControlAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the default overlay configuration for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The default overlay configuration.</returns>
        Task<DefaultZoneOverlay> GetDefaultZoneOverlayAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the early start settings for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The early start configuration.</returns>
        Task<EarlyStart> GetEarlyStartAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the settings used for the zone while the home is in AWAY mode.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The away configuration for the zone.</returns>
        Task<AwayConfiguration> GetAwayConfigurationAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the active timetable type for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The active timetable type.</returns>
        Task<TimetableType> GetActiveTimetableTypeAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the timetable types supported by a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The available timetable types.</returns>
        Task<IReadOnlyList<TimetableType>> GetZoneTimetablesAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches a specific timetable type definition for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="timetableTypeId">The timetable type ID to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The requested timetable type.</returns>
        Task<TimetableType> GetZoneTimetableAsync(int homeId, int zoneId, int timetableTypeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches all timetable blocks for a specific timetable type.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="timetableTypeId">The timetable type ID to inspect.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The timetable blocks for the specified timetable type.</returns>
        Task<IReadOnlyList<TimetableBlock>> GetZoneTimetableBlocksAsync(int homeId, int zoneId, int timetableTypeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the timetable blocks for a specific day type in a timetable.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="timetableTypeId">The timetable type ID to inspect.</param>
        /// <param name="dayType">The day type to retrieve.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The timetable blocks for the specified day type.</returns>
        Task<IReadOnlyList<TimetableBlock>> GetTimetableBlocksByDayTypeAsync(int homeId, int zoneId, int timetableTypeId, string dayType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the day report payload for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to inspect.</param>
        /// <param name="date">An optional report date. When omitted, the current date is used.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The requested day report.</returns>
        Task<ZoneDayReport> GetZoneDayReportAsync(int homeId, int zoneId, DateOnly? date = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the temperature offset for a zone by using the first associated device serial.
        /// </summary>
        /// <param name="zone">The zone whose measuring device offset should be resolved.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The resolved temperature offset.</returns>
        Task<Temperature> GetZoneTemperatureOffsetAsync(Zone zone, CancellationToken cancellationToken = default);

        /// <summary>
        /// Enables or disables the early start mode for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="enabled">True to enable early start; otherwise false.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns><see langword="true"/> when the command succeeds.</returns>
        Task<bool> SetEarlyStartAsync(int homeId, int zoneId, bool enabled, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the open window detection settings for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="settings">The open window detection settings to apply.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        Task SetOpenWindowDetectionAsync(int homeId, int zoneId, OpenWindowDetection settings, CancellationToken cancellationToken = default);

        /// <summary>
        /// Activates the open window state for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        Task ActivateOpenWindowAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Resets the open window state for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        Task ResetOpenWindowAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the writable details of a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="zoneDetails">The writable zone details payload.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The updated zone definition.</returns>
        Task<Zone> SetZoneDetailsAsync(int homeId, int zoneId, Zone zoneDetails, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the default overlay configuration for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="defaultOverlay">The default overlay configuration to apply.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The updated default overlay configuration.</returns>
        Task<DefaultZoneOverlay> SetDefaultZoneOverlayAsync(int homeId, int zoneId, DefaultZoneOverlay defaultOverlay, CancellationToken cancellationToken = default);

        /// <summary>
        /// Applies manual overlays to multiple zones in a single home-level command.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zones.</param>
        /// <param name="zoneOverlays">The set of zone IDs and overlay payloads to apply.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        Task SetZoneOverlaysAsync(int homeId, IReadOnlyDictionary<int, Overlay> zoneOverlays, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the currently active manual overlays for multiple zones in a single home-level command.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zones.</param>
        /// <param name="zoneIds">The IDs of the zones whose overlays should be deleted.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        Task DeleteZoneOverlaysAsync(int homeId, IReadOnlyList<int> zoneIds, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the settings used for a zone while the home is in AWAY mode.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="awayConfiguration">The away configuration to apply.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        Task SetAwayConfigurationAsync(int homeId, int zoneId, AwayConfiguration awayConfiguration, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the active timetable type for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="timetableType">The timetable type to activate.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The active timetable type returned by the API.</returns>
        Task<TimetableType> SetActiveTimetableTypeAsync(int homeId, int zoneId, TimetableType timetableType, CancellationToken cancellationToken = default);

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
        Task<IReadOnlyList<TimetableBlock>> SetTimetableBlocksForDayTypeAsync(int homeId, int zoneId, int timetableTypeId, string dayType, IReadOnlyList<TimetableBlock> blocks, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new zone and moves the specified devices into it.
        /// </summary>
        /// <param name="homeId">The ID of the home in which the zone should be created.</param>
        /// <param name="zoneType">The zone type, such as HEATING or HOT_WATER.</param>
        /// <param name="deviceSerialNumbers">The device serial numbers to move into the new zone.</param>
        /// <param name="force">Optional flag indicating whether the previous zone should be forcibly removed when supported.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        Task CreateZoneAsync(int homeId, string zoneType, IReadOnlyList<string> deviceSerialNumbers, bool? force = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes the currently active manual overlay for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone whose overlay should be deleted.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns><see langword="true"/> when the command succeeds.</returns>
        Task<bool> DeleteZoneOverlayAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Assigns the zone to a specific heating circuit or removes the assignment.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="circuitNumber">The heating circuit number to assign, or <see langword="null"/> to remove the assignment.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The updated zone control details.</returns>
        Task<ZoneControl> SetHeatingCircuitAsync(int homeId, int zoneId, int? circuitNumber, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the heating temperature in Celsius for a zone, keeping it until the next manual change.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="temperature">The target temperature in Celsius.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The resulting zone summary, or <see langword="null"/> when no payload is returned.</returns>
        Task<ZoneSummary?> SetHeatingTemperatureCelsiusAsync(int homeId, int zoneId, double temperature, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the heating temperature in Fahrenheit for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="temperature">The target temperature in Fahrenheit.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The resulting zone summary, or <see langword="null"/> when no payload is returned.</returns>
        Task<ZoneSummary?> SetHeatingTemperatureFahrenheitAsync(int homeId, int zoneId, double temperature, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the hot water temperature in Celsius for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="temperature">The target temperature in Celsius.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The resulting zone summary, or <see langword="null"/> when no payload is returned.</returns>
        Task<ZoneSummary?> SetHotWaterTemperatureCelsiusAsync(int homeId, int zoneId, double temperature, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the hot water temperature in Fahrenheit for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="temperature">The target temperature in Fahrenheit.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The resulting zone summary, or <see langword="null"/> when no payload is returned.</returns>
        Task<ZoneSummary?> SetHotWaterTemperatureFahrenheitAsync(int homeId, int zoneId, double temperature, CancellationToken cancellationToken = default);

        /// <summary>
        /// Switches heating off for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The resulting zone summary, or <see langword="null"/> when no payload is returned.</returns>
        Task<ZoneSummary?> SwitchHeatingOffAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Switches hot water off for a zone.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The resulting zone summary, or <see langword="null"/> when no payload is returned.</returns>
        Task<ZoneSummary?> SwitchHotWaterOffAsync(int homeId, int zoneId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets the heating temperature in Celsius for a zone for the specified duration.
        /// </summary>
        /// <param name="homeId">The ID of the home containing the zone.</param>
        /// <param name="zoneId">The ID of the zone to update.</param>
        /// <param name="temperature">The target temperature in Celsius.</param>
        /// <param name="durationMode">How long the overlay should remain active.</param>
        /// <param name="timer">The timer duration when <paramref name="durationMode"/> is timer-based.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The resulting zone summary, or <see langword="null"/> when no payload is returned.</returns>
        Task<ZoneSummary?> SetHeatingTemperatureCelsiusAsync(int homeId, int zoneId, double temperature, DurationModes durationMode, TimeSpan? timer = null, CancellationToken cancellationToken = default);
    }
}
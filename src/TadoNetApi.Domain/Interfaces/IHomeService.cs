namespace TadoNetApi.Domain.Interfaces;

using TadoNetApi.Domain.Entities;

/// <summary>
/// Service interface for managing homes and their presence states.
/// </summary>
public interface IHomeService
{
    /// <summary>
    /// Retrieves a specific home by its unique identifier.
    /// </summary>
    /// <param name="homeId">The ID of the home to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The requested home, or <see langword="null"/> when no payload is returned.</returns>
    Task<House?> GetHomeAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the state of a home (presence, etc.).
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The current home state, or <see langword="null"/> when no payload is returned.</returns>
    Task<HomeState?> GetHomeStateAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the users associated with a home.
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A read-only list of users associated with the home.</returns>
    Task<IReadOnlyList<User>> GetUsersAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the air comfort indicators for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The current air comfort indicators.</returns>
    Task<AirComfort> GetAirComfortAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the installations configured for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A read-only list of installations associated with the home.</returns>
    Task<IReadOnlyList<Installation>> GetInstallationsAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific installation for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    /// <param name="installationId">The ID of the installation to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The requested installation, or <see langword="null"/> when no payload is returned.</returns>
    Task<Installation?> GetInstallationAsync(int homeId, int installationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the incident detection settings for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The incident detection configuration.</returns>
    Task<IncidentDetection> GetIncidentDetectionAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the heating circuits configured for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>A read-only list of configured heating circuits.</returns>
    Task<IReadOnlyList<HeatingCircuit>> GetHeatingCircuitsAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the heating system configuration for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The configured heating system.</returns>
    Task<HeatingSystem> GetHeatingSystemAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the flow-temperature optimisation settings for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The flow-temperature optimisation settings.</returns>
    Task<FlowTemperatureOptimisation> GetFlowTemperatureOptimisationAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the presence state of a home (e.g., HOME, AWAY).
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    /// <param name="presence">The new presence state.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    Task SetHomePresenceAsync(int homeId, string presence, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the manually set presence state of a home and returns control to geo-tracking.
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    Task ResetHomePresenceAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the geo-tracking away radius for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="awayRadiusInMeters">The distance in meters at which a device is considered away from home.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    Task SetAwayRadiusInMetersAsync(int homeId, double awayRadiusInMeters, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enables or disables incident detection for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="enabled">Whether incident detection should be enabled.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    Task SetIncidentDetectionAsync(int homeId, bool enabled, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the writable home details for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="homeDetails">The complete writable home details payload.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    Task SetHomeDetailsAsync(int homeId, House homeDetails, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the maximum flow temperature for a home's boiler optimisation.
    /// </summary>
    /// <param name="homeId">The ID of the home to update.</param>
    /// <param name="maxFlowTemperature">The maximum flow temperature to apply.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    Task SetFlowTemperatureOptimisationAsync(int homeId, int maxFlowTemperature, CancellationToken cancellationToken = default);
}
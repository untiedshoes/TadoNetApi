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
    Task<House?> GetHomeAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the state of a home (presence, etc.).
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    Task<HomeState?> GetHomeStateAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the users associated with a home.
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    Task<IReadOnlyList<User>> GetUsersAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the air comfort indicators for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    Task<AirComfort> GetAirComfortAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the incident detection settings for a home.
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    Task<IncidentDetection> GetIncidentDetectionAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the presence state of a home (e.g., HOME, AWAY).
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    /// <param name="presence">The new presence state.</param>
    Task SetHomePresenceAsync(int homeId, string presence, CancellationToken cancellationToken = default);
}
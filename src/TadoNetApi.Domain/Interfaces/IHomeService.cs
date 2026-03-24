namespace TadoNetApi.Domain.Interfaces;

using TadoNetApi.Domain.Entities;

/// <summary>
/// Service interface for managing homes and their presence states.
/// </summary>
public interface IHomeService
{
    /// <summary>
    /// Retrieves a list of all homes accessible by the user.
    /// </summary>
    Task<List<Home>> GetHomesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific home by its unique identifier.
    /// </summary>
    /// <param name="homeId">The ID of the home to retrieve.</param>
    Task<Home?> GetHomeAsync(int homeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the presence state of a home (e.g., HOME, AWAY).
    /// </summary>
    /// <param name="homeId">The ID of the home.</param>
    /// <param name="presence">The new presence state.</param>
    Task SetHomePresenceAsync(int homeId, string presence, CancellationToken cancellationToken = default);
}
using TadoNetApi.Domain.Entities;

namespace TadoNetApi.Domain.Interfaces;

/// <summary>
/// Service interface for retrieving weather information for homes.
/// </summary>
public interface IWeatherService
{
    /// <summary>
    /// Retrieves the current weather for the given home.
    /// </summary>
    /// <param name="homeId">The ID of the home whose weather should be retrieved.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The current weather for the home.</returns>
    Task<Weather> GetWeatherAsync(int homeId, CancellationToken cancellationToken = default);
}
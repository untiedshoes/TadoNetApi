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
    Task<Weather> GetWeatherAsync(int homeId, CancellationToken cancellationToken = default);
}
using TadoNetApi.Domain.Entities;

namespace TadoNetApi.Domain.Interfaces;

/// <summary>
/// Provides boiler configuration and diagnostics through bridge-scoped endpoints.
/// </summary>
public interface IBoilerByBridgeService
{
    /// <summary>
    /// Retrieves information about the home's boiler.
    /// </summary>
    Task<BoilerInfo?> GetBoilerInfoAsync(string bridgeId, string authKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the configured maximum boiler output temperature.
    /// </summary>
    Task<BoilerMaxOutputTemperature?> GetBoilerMaxOutputTemperatureAsync(string bridgeId, string authKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the configured maximum boiler output temperature.
    /// </summary>
    Task SetBoilerMaxOutputTemperatureAsync(string bridgeId, string authKey, double boilerMaxOutputTemperatureInCelsius, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the boiler wiring installation state.
    /// </summary>
    Task<BoilerWiringInstallationState?> GetBoilerWiringInstallationStateAsync(string bridgeId, string authKey, CancellationToken cancellationToken = default);
}
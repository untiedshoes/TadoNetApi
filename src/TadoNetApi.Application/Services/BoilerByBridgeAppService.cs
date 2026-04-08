using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;

namespace TadoNetApi.Application.Services
{
    /// <summary>
    /// Application service for bridge-scoped boiler operations.
    /// </summary>
    public class BoilerByBridgeAppService
    {
        private readonly IBoilerByBridgeService _boilerByBridgeService;

        public BoilerByBridgeAppService(IBoilerByBridgeService boilerByBridgeService)
        {
            _boilerByBridgeService = boilerByBridgeService;
        }

        /// <summary>
        /// Retrieves information about the home's boiler.
        /// </summary>
        public Task<BoilerInfo?> GetBoilerInfoAsync(string bridgeId, string authKey, CancellationToken cancellationToken = default)
            => _boilerByBridgeService.GetBoilerInfoAsync(bridgeId, authKey, cancellationToken);

        /// <summary>
        /// Retrieves the configured maximum boiler output temperature.
        /// </summary>
        public Task<BoilerMaxOutputTemperature?> GetBoilerMaxOutputTemperatureAsync(string bridgeId, string authKey, CancellationToken cancellationToken = default)
            => _boilerByBridgeService.GetBoilerMaxOutputTemperatureAsync(bridgeId, authKey, cancellationToken);

        /// <summary>
        /// Updates the configured maximum boiler output temperature.
        /// </summary>
        public Task SetBoilerMaxOutputTemperatureAsync(string bridgeId, string authKey, double boilerMaxOutputTemperatureInCelsius, CancellationToken cancellationToken = default)
            => _boilerByBridgeService.SetBoilerMaxOutputTemperatureAsync(bridgeId, authKey, boilerMaxOutputTemperatureInCelsius, cancellationToken);

        /// <summary>
        /// Retrieves the boiler wiring installation state.
        /// </summary>
        public Task<BoilerWiringInstallationState?> GetBoilerWiringInstallationStateAsync(string bridgeId, string authKey, CancellationToken cancellationToken = default)
            => _boilerByBridgeService.GetBoilerWiringInstallationStateAsync(bridgeId, authKey, cancellationToken);
    }
}
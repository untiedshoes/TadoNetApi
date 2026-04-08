using System.Net;
using System.Net.Http;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Dtos.Requests;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Mappers;
using TadoNetApi.Infrastructure.Validation;

namespace TadoNetApi.Infrastructure.Services
{
    /// <summary>
    /// Concrete implementation of <see cref="IBoilerByBridgeService"/> using bridge-scoped public Tado endpoints.
    /// </summary>
    public class TadoBoilerByBridgeService : IBoilerByBridgeService
    {
        private readonly IPublicTadoHttpClient _httpClient;

        public TadoBoilerByBridgeService(IPublicTadoHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BoilerInfo?> GetBoilerInfoAsync(string bridgeId, string authKey, CancellationToken cancellationToken = default)
        {
            var dto = await _httpClient.GetAsync<TadoBoilerInfoResponse>(BuildEndpoint(bridgeId, authKey, "boilerInfo"), cancellationToken);
            return dto == null ? null : dto.ToDomain();
        }

        public async Task<BoilerMaxOutputTemperature?> GetBoilerMaxOutputTemperatureAsync(string bridgeId, string authKey, CancellationToken cancellationToken = default)
        {
            var dto = await _httpClient.GetAsync<TadoBoilerMaxOutputTemperatureResponse>(BuildEndpoint(bridgeId, authKey, "boilerMaxOutputTemperature"), cancellationToken);
            return dto == null ? null : dto.ToDomain();
        }

        public async Task SetBoilerMaxOutputTemperatureAsync(string bridgeId, string authKey, double boilerMaxOutputTemperatureInCelsius, CancellationToken cancellationToken = default)
        {
            await _httpClient.SendAsync(
                BuildEndpoint(bridgeId, authKey, "boilerMaxOutputTemperature"),
                HttpMethod.Put,
                cancellationToken,
                HttpStatusCode.NoContent,
                new SetBoilerMaxOutputTemperatureRequest
                {
                    BoilerMaxOutputTemperatureInCelsius = boilerMaxOutputTemperatureInCelsius
                });
        }

        public async Task<BoilerWiringInstallationState?> GetBoilerWiringInstallationStateAsync(string bridgeId, string authKey, CancellationToken cancellationToken = default)
        {
            var dto = await _httpClient.GetAsync<TadoBoilerWiringInstallationStateResponse>(BuildEndpoint(bridgeId, authKey, "boilerWiringInstallationState"), cancellationToken);
            return dto == null ? null : dto.ToDomain();
        }

        private static string BuildEndpoint(string bridgeId, string authKey, string resource)
        {
            Guard.NotNullOrWhiteSpace(bridgeId, nameof(bridgeId));
            Guard.NotNullOrWhiteSpace(authKey, nameof(authKey));

            return $"homeByBridge/{Uri.EscapeDataString(bridgeId)}/{resource}?authKey={Uri.EscapeDataString(authKey)}";
        }
    }
}
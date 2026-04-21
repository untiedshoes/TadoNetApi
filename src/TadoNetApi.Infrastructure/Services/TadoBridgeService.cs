using System.Net;
using System.Net.Http;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Exceptions;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Mappers;
using TadoNetApi.Infrastructure.Validation;

namespace TadoNetApi.Infrastructure.Services
{
    /// <summary>
    /// Concrete implementation of <see cref="IBridgeService"/> using bridge-scoped public Tado endpoints.
    /// </summary>
    public class TadoBridgeService : IBridgeService
    {
        private readonly IPublicTadoHttpClient _httpClient;

        public TadoBridgeService(IPublicTadoHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Bridge?> GetBridgeAsync(string bridgeId, string authKey, CancellationToken cancellationToken = default)
        {
            Guard.NotNullOrWhiteSpace(bridgeId, nameof(bridgeId));
            Guard.NotNullOrWhiteSpace(authKey, nameof(authKey));

            try
            {
                var dto = await _httpClient.GetAsync<TadoBridgeResponse>(
                    $"bridges/{Uri.EscapeDataString(bridgeId)}?authKey={Uri.EscapeDataString(authKey)}",
                    cancellationToken);

                return dto == null ? null : dto.ToDomain();
            }
            catch (HttpRequestException ex)
            {
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable,
                    $"Failed to retrieve bridge: {ex.Message}");
            }
        }
    }
}
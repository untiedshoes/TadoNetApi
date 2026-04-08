using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;

namespace TadoNetApi.Application.Services
{
    /// <summary>
    /// Application service for bridge-scoped operations.
    /// </summary>
    public class BridgeAppService
    {
        private readonly IBridgeService _bridgeService;

        public BridgeAppService(IBridgeService bridgeService)
        {
            _bridgeService = bridgeService;
        }

        /// <summary>
        /// Retrieves details about a Tado Internet Bridge.
        /// </summary>
        public Task<Bridge?> GetBridgeAsync(string bridgeId, string authKey, CancellationToken cancellationToken = default)
            => _bridgeService.GetBridgeAsync(bridgeId, authKey, cancellationToken);
    }
}
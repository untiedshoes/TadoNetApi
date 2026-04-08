using TadoNetApi.Domain.Entities;

namespace TadoNetApi.Domain.Interfaces;

/// <summary>
/// Provides bridge-scoped operations that use the bridge serial number and auth key.
/// </summary>
public interface IBridgeService
{
    /// <summary>
    /// Retrieves details about a Tado Internet Bridge.
    /// </summary>
    /// <param name="bridgeId">The bridge serial number.</param>
    /// <param name="authKey">The auth key printed on the bridge.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The requested bridge details, or <see langword="null"/> when no payload is returned.</returns>
    Task<Bridge?> GetBridgeAsync(string bridgeId, string authKey, CancellationToken cancellationToken = default);
}
using System.Threading;
using System.Threading.Tasks;
using TadoNetApi.Infrastructure.Dtos.Auth;

namespace TadoNetApi.Infrastructure.Auth
{
    /// <summary>
    /// Defines authentication responsibilities for Tado API.
    /// </summary>
    public interface ITadoAuthService
    {

        /// <summary>
        /// Retrieves a valid access token, refreshing if necessary.
        /// </summary>
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Requests an access token from Tado API with retry for transient errors (503, 429)
        /// </summary>
        Task<TadoAuthResponse> RequestTokenAsync(CancellationToken cancellationToken);
    }
}
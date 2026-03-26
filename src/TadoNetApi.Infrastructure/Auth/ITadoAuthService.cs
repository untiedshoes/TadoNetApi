using System.Threading;
using System.Threading.Tasks;

namespace TadoNetApi.Infrastructure.Auth
{
    /// <summary>
    /// Defines authentication responsibilities for Tado API.
    /// </summary>
    public interface ITadoAuthService
    {
        /// <summary>
        /// Returns a valid access token, refreshing if expired.
        /// </summary>
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
    }
}
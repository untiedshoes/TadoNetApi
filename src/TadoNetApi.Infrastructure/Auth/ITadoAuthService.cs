using System.Threading;
using System.Threading.Tasks;
using TadoNetApi.Infrastructure.Dtos.Auth;

namespace TadoNetApi.Infrastructure.Auth
{
    /// <summary>
    /// Handles authentication with Tado API, including OAuth2 Device Code flow.
    /// </summary>
    public interface ITadoAuthService
    {
        /// <summary>
        /// Starts the device authorisation process.
        /// Returns the device_code, user_code, and verification URI to show to the user.
        /// </summary>
        Task<DeviceCodeResponse> StartDeviceAuthorisationAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Polls the Tado token endpoint until the user approves the device.
        /// Returns the access token when available.
        /// </summary>
        Task<TadoAuthResponse> WaitForDeviceTokenAsync(string deviceCode, int intervalSeconds, int maxWaitSeconds, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a valid access token. Will request new token if expired.
        /// </summary>
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
    }
}
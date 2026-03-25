using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Dtos.Auth;

namespace TadoNetApi.Infrastructure.Auth
{
    /// <summary>
    /// Handles Tado OAuth2 Device Authorization and token management.
    /// </summary>
    public class TadoAuthService : ITadoAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly TadoApiConfig _config;
        private TadoAuthResponse? _authResponse;

        public TadoAuthService(HttpClient httpClient, TadoApiConfig config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        /// <summary>
        /// Indicates whether there is a valid access token.
        /// </summary>
        public bool IsAuthenticated => _authResponse != null && !_authResponse.IsExpired;

        /// <summary>
        /// Returns a valid access token, authorizing the device if necessary.
        /// </summary>
        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            if (!IsAuthenticated)
                await AuthorizeDeviceAsync(cancellationToken);

            return _authResponse!.AccessToken;
        }

        /// <summary>
        /// Ensures the service is authenticated, refreshing token if needed.
        /// </summary>
        public async Task EnsureAuthenticatedAsync(CancellationToken cancellationToken = default)
        {
            if (!IsAuthenticated)
                await AuthorizeDeviceAsync(cancellationToken);
        }

        /// <summary>
        /// Performs the device authorization flow.
        /// </summary>
        private async Task AuthorizeDeviceAsync(CancellationToken cancellationToken)
        {
            // Step 1: Request Device Code
            var deviceRequest = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", TadoApiEndpoints.ClientId),
                new KeyValuePair<string, string>("scope", TadoApiEndpoints.ScopeHomeUser)
            });

            var deviceResponse = await _httpClient.PostAsync(TadoApiEndpoints.DeviceAuthorizeUrl, deviceRequest, cancellationToken);
            deviceResponse.EnsureSuccessStatusCode();

            var deviceJson = await deviceResponse.Content.ReadAsStringAsync(cancellationToken);
            var deviceCodeResponse = JsonSerializer.Deserialize<DeviceCodeResponse>(deviceJson)!;

            Console.WriteLine("Please authorize your device in the browser:");
            Console.WriteLine($"URL: {deviceCodeResponse.VerificationUri}");
            Console.WriteLine($"Code: {deviceCodeResponse.UserCode}");

            // Step 2: Poll for token
            var polling = true;
            while (polling)
            {
                await Task.Delay(deviceCodeResponse.Interval * 1000, cancellationToken);

                var tokenRequest = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", TadoApiEndpoints.ClientId),
                    new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:device_code"),
                    new KeyValuePair<string, string>("device_code", deviceCodeResponse.DeviceCode)
                });

                var tokenResponse = await _httpClient.PostAsync(TadoApiEndpoints.TokenUrl, tokenRequest, cancellationToken);
                var tokenJson = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);

                if (tokenResponse.IsSuccessStatusCode)
                {
                    _authResponse = JsonSerializer.Deserialize<TadoAuthResponse>(tokenJson)!;
                    polling = false;
                    Console.WriteLine("Device authorized! Access token obtained.");
                }
                else
                {
                    var error = JsonSerializer.Deserialize<TokenErrorResponse>(tokenJson);
                    if (error!.Error != "authorization_pending")
                        throw new Exception($"Authorization failed: {error.Error}");
                }
            }
        }
    }
}
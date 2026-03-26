using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Dtos.Auth;

namespace TadoNetApi.Infrastructure.Auth
{
    /// <summary>
    /// Handles OAuth2 authentication for Tado API using device authorization / password grant.
    /// Caches access tokens and refreshes automatically.
    /// </summary>
    public class TadoAuthService : ITadoAuthService
    {
        private readonly TadoApiConfig _config;
        private readonly HttpClient _httpClient;

        private TadoAuthResponse? _token;
        private readonly object _lock = new();

        public TadoAuthService(TadoApiConfig config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            lock (_lock)
            {
                if (_token != null && !_token.IsExpired)
                    return _token.AccessToken;
            }

            var token = await RequestTokenAsync(cancellationToken);

            lock (_lock)
            {
                _token = token;
                return _token.AccessToken;
            }
        }

        public async Task<TadoAuthResponse> RequestTokenAsync(CancellationToken cancellationToken)
        {
            var formData = new MultipartFormDataContent
            {
                { new StringContent("password"), "grant_type" },
                { new StringContent(_config.Username), "username" },
                { new StringContent(_config.Password), "password" },
                { new StringContent(TadoApiEndpoints.ClientId), "client_id" },
                { new StringContent(TadoApiEndpoints.ScopeHomeUser), "scope" }
            };

            var response = await _httpClient.PostAsync(TadoApiEndpoints.TokenUrl, formData, cancellationToken);
            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadFromJsonAsync<TadoAuthResponse>(cancellationToken: cancellationToken)
                        ?? throw new InvalidOperationException("Failed to get access token from Tado API.");

            return token;
        }
    }
}
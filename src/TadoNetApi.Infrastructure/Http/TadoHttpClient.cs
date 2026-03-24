using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using TadoNetApi.Infrastructure.Auth;
using TadoNetApi.Infrastructure.Config;

namespace TadoNetApi.Infrastructure.Http
{
    /// <summary>
    /// Handles HTTP requests to the Tado API, including authentication and base URL setup.
    /// </summary>
    public class TadoHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly TadoAuthService _authService;

        public TadoHttpClient(HttpClient httpClient, TadoAuthService authService)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(TadoApiEndpoints.ApiBaseUrl);
            _authService = authService;
        }

        /// <summary>
        /// Performs an authenticated GET request and deserializes the JSON response.
        /// </summary>
        public async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default)
        {
            await _authService.EnsureAuthenticatedAsync(cancellationToken);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await _authService.GetAccessTokenAsync(cancellationToken));

            return await _httpClient.GetFromJsonAsync<T>(url, cancellationToken);
        }

        /// <summary>
        /// Performs an authenticated POST request with a JSON payload and deserializes the response.
        /// </summary>
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest payload, CancellationToken cancellationToken = default)
        {
            await _authService.EnsureAuthenticatedAsync(cancellationToken);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await _authService.GetAccessTokenAsync(cancellationToken));

            var response = await _httpClient.PostAsJsonAsync(url, payload, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
        }
    }
}
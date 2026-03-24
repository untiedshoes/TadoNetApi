using System.Net.Http.Headers;
using System.Net.Http.Json;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Dtos.Responses;

namespace TadoNetApi.Infrastructure.Http;

/// <summary>
/// Handles HTTP requests to the Tado API, including authentication, throttling, and base URL setup.
/// </summary>
public class TadoHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly TadoApiConfig _config;
    private string? _accessToken;

    public TadoHttpClient(HttpClient httpClient, TadoApiConfig config)
    {
        _httpClient = httpClient;
        _config = config;
        _httpClient.BaseAddress = new Uri("https://my.tado.com/api/v2/");
    }

    /// <summary>
    /// Authenticates with Tado API and stores access token for subsequent requests.
    /// </summary>
    public async Task AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        var request = new
        {
            username = _config.Username,
            password = _config.Password,
            client_id = "tado-web-app",
            grant_type = "password"
        };

        var response = await _httpClient.PostAsJsonAsync("oauth/token", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<TadoAuthResponse>(cancellationToken: cancellationToken);
        if (auth == null) throw new Exception("Authentication failed");

        _accessToken = auth.AccessToken;
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _accessToken);
    }

    /// <summary>
    /// GET request with automatic throttling retry.
    /// </summary>
    public async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        int retries = 0;
        int delayMs = _config.InitialRetryDelayMs;

        while (true)
        {
            HttpResponseMessage? response = null;

            try
            {
                response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
            }
            catch (HttpRequestException ex) when (response != null && response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                retries++;
                if (retries > _config.MaxRetries) throw;

                // Use Retry-After header if present
                var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromMilliseconds(delayMs);
                Console.WriteLine($"Rate limit hit on GET {url}, retrying in {retryAfter.TotalSeconds:F1}s...");
                await Task.Delay(retryAfter, cancellationToken);

                delayMs *= 2; // exponential backoff
            }
        }
    }

    //Summary>
    /// POST request with automatic throttling retry.   
    /// </Summary>
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest payload, CancellationToken cancellationToken = default)
    {
        int retries = 0;
        int delayMs = _config.InitialRetryDelayMs;

        while (true)
        {
            HttpResponseMessage? response = null;

            try
            {
                response = await _httpClient.PostAsJsonAsync(url, payload, cancellationToken);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
            }
            catch (HttpRequestException ex) when (response != null && response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                retries++;
                if (retries > _config.MaxRetries) throw;

                var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromMilliseconds(delayMs);
                Console.WriteLine($"Rate limit hit on POST {url}, retrying in {retryAfter.TotalSeconds:F1}s...");
                await Task.Delay(retryAfter, cancellationToken);

                delayMs *= 2;
            }
        }
    }
}
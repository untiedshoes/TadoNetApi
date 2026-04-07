using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TadoNetApi.Infrastructure.Exceptions;

namespace TadoNetApi.Infrastructure.Http
{
    /// <summary>
    /// Central HTTP client for communicating with the Tado API.
    /// 
    /// Responsibilities:
    /// - Sending HTTP requests (GET, POST, etc.)
    /// - Handling response deserialization
    /// - Logging request/response activity
    /// - Translating HTTP errors into domain-friendly exceptions
    /// 
    /// NOTE:
    /// Authentication is NOT handled here directly.
    /// It is delegated to <see cref="AuthDelegatingHandler"/> via HttpClient pipeline.
    /// </summary>
    public class TadoHttpClient : ITadoHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TadoHttpClient> _logger;

        /// <summary>
        /// Shared JSON serializer options for all API responses.
        /// - Case insensitive to match Tado API responses
        /// </summary>
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Constructs the TadoHttpClient.
        /// HttpClient is injected via DI and configured with:
        /// - BaseAddress
        /// - DelegatingHandlers (Auth, retry, etc.)
        /// </summary>
        public TadoHttpClient(HttpClient httpClient, ILogger<TadoHttpClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        #region Data Retrieval

        /// <summary>
        /// Performs a GET request to the specified endpoint and deserializes the response.
        /// </summary>
        public async Task<T?> GetAsync<T>(
            string endpoint,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

            return await SendAsync<T>(request, cancellationToken);
        }

        #endregion

        #region Data Submission

        /// <summary>
        /// Performs a POST request with a JSON body and deserializes the response.
        /// </summary>
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(
            string endpoint,
            TRequest body,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json")
            };

            return await SendAsync<TResponse>(request, cancellationToken);
        }

        /// <summary>
        /// Performs a PUT request with a JSON body and deserializes the response.
        /// </summary>
        public async Task<TResponse?> PutAsync<TRequest, TResponse>(
            string endpoint,
            TRequest body,
            CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, endpoint);

            if (body != null)
            {
                request.Content = new StringContent(
                    JsonSerializer.Serialize(body),
                    Encoding.UTF8,
                    "application/json");
            }

            return await SendAsync<TResponse>(request, cancellationToken);
        }

        #endregion

        #region Send Commands

        /// <summary>
        /// Sends a command-style request (POST/PUT/DELETE) and validates the expected status code.
        /// Intended for endpoints that return no payload but do return a known success status.
        /// </summary>
        public async Task<bool> SendAsync(
            string endpoint,
            HttpMethod method,
            CancellationToken cancellationToken = default,
            HttpStatusCode expectedStatusCode = HttpStatusCode.NoContent,
            object? body = null)
        {
            try
            {
                var request = new HttpRequestMessage(method, endpoint);

                if (body != null)
                {
                    request.Content = new StringContent(
                        JsonSerializer.Serialize(body),
                        Encoding.UTF8,
                        "application/json");
                }

                _logger.LogInformation(
                    "Sending HTTP command: {Method} {Url}",
                    request.Method,
                    request.RequestUri);

                using var response = await _httpClient.SendAsync(request, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (response.StatusCode != expectedStatusCode)
                {
                    LogApiFailure(
                        "Tado API command failed. Expected: {ExpectedStatusCode}, Actual: {StatusCode}, Response: {Response}",
                        expectedStatusCode,
                        response.StatusCode,
                        content);

                    throw new TadoApiException(response.StatusCode, content);
                }

                return true;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "HTTP command timed out.");
                throw new TadoApiException(HttpStatusCode.RequestTimeout, "Request timed out");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP command error while calling Tado API.");
                throw new TadoApiException(HttpStatusCode.ServiceUnavailable, ex.Message);
            }
        }

        #endregion

        #region Core Pipeline

        /// <summary>
        /// Core HTTP execution pipeline.
        /// 
        /// This method:
        /// - Sends the HTTP request
        /// - Logs request/response details
        /// - Handles non-success status codes
        /// - Deserializes JSON responses
        /// - Wraps errors in TadoApiException
        /// 
        /// IMPORTANT:
        /// Authentication headers are already applied before this point
        /// via AuthDelegatingHandler in the HttpClient pipeline.
        /// </summary>
        private async Task<T?> SendAsync<T>(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Log outgoing request
                _logger.LogInformation(
                    "Sending HTTP request: {Method} {Url}",
                    request.Method,
                    request.RequestUri);

                // Execute request through HttpClient pipeline
                using var response = await _httpClient.SendAsync(request, cancellationToken);

                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                // Handle non-success responses
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        _logger.LogInformation(
                            "Tado API request returned not found. Status: {StatusCode}, Response: {Response}",
                            response.StatusCode,
                            content);
                    }
                    else
                    {
                        _logger.LogError(
                            "Tado API request failed. Status: {StatusCode}, Response: {Response}",
                            response.StatusCode,
                            content);
                    }

                    // Wrap external API failure in a controlled exception
                    throw new TadoApiException(response.StatusCode, content);
                }

                // Deserialize JSON response
                var result = JsonSerializer.Deserialize<T>(content, _serializerOptions);

                if (result == null)
                {
                    throw new TadoApiException(response.StatusCode, "Failed to deserialize response.");
                }

                return result;
            }
            catch (TaskCanceledException ex)
            {
                // Timeout or cancellation
                _logger.LogError(ex, "HTTP request timed out.");

                throw new TadoApiException(HttpStatusCode.RequestTimeout, "Request timed out");
            }
            catch (HttpRequestException ex)
            {
                // Network or connection failure
                _logger.LogError(ex, "HTTP request error while calling Tado API.");

                throw new TadoApiException(HttpStatusCode.ServiceUnavailable, ex.Message);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize JSON response from Tado API.");

                throw new TadoApiException(HttpStatusCode.UnprocessableEntity,
                    "Failed to deserialize API response.");
            }
        }

        private void LogApiFailure(
            string message,
            HttpStatusCode expectedStatusCode,
            HttpStatusCode actualStatusCode,
            string content)
        {
            if (actualStatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation(
                    message,
                    expectedStatusCode,
                    actualStatusCode,
                    content);
                return;
            }

            _logger.LogError(
                message,
                expectedStatusCode,
                actualStatusCode,
                content);
        }

        #endregion
    }
}
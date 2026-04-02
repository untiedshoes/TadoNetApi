using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using TadoNetApi.Infrastructure.Exceptions;
using TadoNetApi.Infrastructure.Config;

namespace TadoNetApi.Infrastructure.Http;

/// <summary>
/// DelegatingHandler that automatically retries requests when
/// the Tado API returns a 429 (Too Many Requests).
/// </summary>
public class RetryDelegatingHandler : DelegatingHandler
{
    private readonly TadoApiConfig _config;
    private readonly ILogger<RetryDelegatingHandler> _logger;

    public RetryDelegatingHandler(TadoApiConfig config, ILogger<RetryDelegatingHandler> logger)
    {
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Sends the HTTP request through the handler pipeline and retries when the API returns
    /// <c>429 Too Many Requests</c>.
    /// </summary>
    /// <param name="request">The outgoing HTTP request.</param>
    /// <param name="cancellationToken">Token used to cancel request execution and retry delays.</param>
    /// <returns>
    /// The first non-429 <see cref="HttpResponseMessage"/> returned by the downstream handler.
    /// </returns>
    /// <exception cref="TadoApiException">
    /// Thrown when rate limiting persists beyond the configured retry count.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown when the request or retry delay is canceled.
    /// </exception>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        int attempt = 0;
        int delayMs = _config.InitialRetryDelayMs;

        while (true)
        {
            attempt++;
            using var retryRequest = await CloneHttpRequestMessageAsync(request, cancellationToken);
            HttpResponseMessage response = await base.SendAsync(retryRequest, cancellationToken);

            if (response.StatusCode != (HttpStatusCode)429) // Not rate-limited
                return response;

            if (attempt > _config.MaxRetries)
            {
                _logger.LogError("Tado API rate limit exceeded after {Attempts} attempts.", attempt);
                response.Dispose();
                throw new TadoApiException(response.StatusCode, "Rate limit exceeded.");
            }

            // Read Retry-After header (seconds)
            int waitSeconds = 0;
            if (response.Headers.TryGetValues("Retry-After", out var values))
            {
                var retryAfterValue = values.FirstOrDefault();
                if (int.TryParse(retryAfterValue, out int parsed))
                    waitSeconds = parsed;
            }

            if (waitSeconds == 0)
                waitSeconds = delayMs / 1000; // fallback to exponential backoff

            _logger.LogWarning(
                "Tado API rate limited. Attempt {Attempt}/{MaxRetries}. Waiting {Seconds}s before retrying.",
                attempt, _config.MaxRetries, waitSeconds);

            response.Dispose();
            await Task.Delay(waitSeconds * 1000, cancellationToken);

            delayMs *= 2; // exponential backoff
        }
    }

    /// <summary>
    /// Creates a deep clone of an <see cref="HttpRequestMessage"/> so it can be safely re-sent
    /// on retry attempts.
    /// </summary>
    /// <param name="request">The original request to clone.</param>
    /// <param name="cancellationToken">Token used while reading request content.</param>
    /// <returns>A new <see cref="HttpRequestMessage"/> instance with copied metadata and content.</returns>
    /// <exception cref="OperationCanceledException">
    /// Thrown when content copy is canceled.
    /// </exception>
    private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri)
        {
            Version = request.Version,
            VersionPolicy = request.VersionPolicy
        };

        foreach (var header in request.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        foreach (var option in request.Options)
            clone.Options.Set(new HttpRequestOptionsKey<object?>(option.Key), option.Value);

        if (request.Content != null)
        {
            var contentBytes = await request.Content.ReadAsByteArrayAsync(cancellationToken);
            clone.Content = new ByteArrayContent(contentBytes);

            foreach (var header in request.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clone;
    }
}
using System.Net;
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
            {
                // Log a recovery message so that a preceding rate-limit warning
                // isn't left dangling in the log without a resolution event.
                if (attempt > 1)
                    _logger.LogInformation(
                        "Tado API recovered after {Attempts} retries. {Method} {Uri}",
                        attempt - 1,
                        retryRequest.Method,
                        retryRequest.RequestUri);
                return response;
            }

            if (attempt > _config.MaxRetries)
            {
                // Include the rate-limit headers on the terminal error so the full
                // context is available even if the preceding warnings were filtered out.
                var exception = new RequestThrottledException(retryRequest, response);
                _logger.LogError(
                    "Tado API rate limit exceeded after {Attempts} attempts. {Method} {Uri} Policy={Policy} Remaining={Remaining} Reset={Reset}s",
                    attempt,
                    retryRequest.Method,
                    retryRequest.RequestUri,
                    exception.RateLimitPolicyName,
                    exception.RemainingRequests,
                    exception.ResetTimeSeconds);
                response.Dispose();
                throw exception;
            }

            var waitSeconds = GetRetryDelaySeconds(response, delayMs);

            var throttledException = new RequestThrottledException(retryRequest, response);

            _logger.LogWarning(
                "Tado API rate limited. {Method} {Uri} Attempt {Attempt}/{MaxRetries}. Waiting {Seconds}s before retrying. Policy={Policy} Remaining={Remaining} Reset={Reset}s",
                retryRequest.Method,
                retryRequest.RequestUri,
                attempt,
                _config.MaxRetries,
                waitSeconds,
                throttledException.RateLimitPolicyName,
                throttledException.RemainingRequests,
                throttledException.ResetTimeSeconds);

            response.Dispose();
            await Task.Delay(waitSeconds * 1000, cancellationToken);

            delayMs *= 2; // exponential backoff
        }
    }

    private static int GetRetryDelaySeconds(HttpResponseMessage response, int fallbackDelayMs)
    {
        if (response.Headers.RetryAfter?.Delta != null)
        {
            return Math.Max(1, (int)Math.Ceiling(response.Headers.RetryAfter.Delta.Value.TotalSeconds));
        }

        if (response.Headers.RetryAfter?.Date != null)
        {
            var secondsUntilRetry = response.Headers.RetryAfter.Date.Value - DateTimeOffset.UtcNow;
            if (secondsUntilRetry > TimeSpan.Zero)
                return Math.Max(1, (int)Math.Ceiling(secondsUntilRetry.TotalSeconds));
        }

        return Math.Max(1, (int)Math.Ceiling(fallbackDelayMs / 1000d));
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
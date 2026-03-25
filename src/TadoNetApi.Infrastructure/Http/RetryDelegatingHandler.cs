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

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        int attempt = 0;
        int delayMs = _config.InitialRetryDelayMs;

        while (true)
        {
            attempt++;
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != (HttpStatusCode)429) // Not rate-limited
                return response;

            if (attempt > _config.MaxRetries)
            {
                _logger.LogError("Tado API rate limit exceeded after {Attempts} attempts.", attempt);
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

            await Task.Delay(waitSeconds * 1000, cancellationToken);

            delayMs *= 2; // exponential backoff
        }
    }
}
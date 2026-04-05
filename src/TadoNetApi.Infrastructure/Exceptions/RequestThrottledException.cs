using System.Net;
using System.Net.Http;

namespace TadoNetApi.Infrastructure.Exceptions;

/// <summary>
/// Exception thrown when a request is throttled by the Tado API.
/// </summary>
public class RequestThrottledException : TadoApiException
{
    /// <summary>
    /// The request URI that was throttled.
    /// </summary>
    public string? RequestUri { get; }

    /// <summary>
    /// The rate limit policy name, for example <c>perday</c>.
    /// </summary>
    public string? RateLimitPolicyName { get; private set; }

    /// <summary>
    /// The total quota for the current rate-limit policy.
    /// </summary>
    public int? RateLimitQuota { get; private set; }

    /// <summary>
    /// The policy time window in seconds.
    /// </summary>
    public int? RateLimitWindowSeconds { get; private set; }

    /// <summary>
    /// The remaining requests in the current time window.
    /// </summary>
    public int? RemainingRequests { get; private set; }

    /// <summary>
    /// The number of seconds until the limit resets.
    /// </summary>
    public int? ResetTimeSeconds { get; private set; }

    /// <summary>
    /// The retry-after delay, in seconds, when explicitly provided.
    /// </summary>
    public int? RetryAfterSeconds { get; private set; }

    /// <summary>
    /// The retry-after date when the server provides one.
    /// </summary>
    public DateTimeOffset? RetryAfterDate { get; private set; }

    public RequestThrottledException(HttpRequestMessage? request, HttpResponseMessage? response)
        : base(HttpStatusCode.TooManyRequests,
            $"The request to {response?.RequestMessage?.RequestUri ?? request?.RequestUri} failed because of throttling.")
    {
        RequestUri = response?.RequestMessage?.RequestUri?.ToString() ?? request?.RequestUri?.ToString();

        if (response != null)
            ParseHeaders(response);
    }

    private void ParseHeaders(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("RateLimit-Policy", out var policyValues))
        {
            var policyHeader = policyValues.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(policyHeader))
                ParseRateLimitPolicy(policyHeader);
        }

        if (response.Headers.TryGetValues("RateLimit", out var rateLimitValues))
        {
            var rateLimitHeader = rateLimitValues.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(rateLimitHeader))
                ParseRateLimit(rateLimitHeader);
        }

        var retryAfter = response.Headers.RetryAfter;
        if (retryAfter?.Delta != null)
            RetryAfterSeconds = (int)Math.Ceiling(retryAfter.Delta.Value.TotalSeconds);

        if (retryAfter?.Date != null)
            RetryAfterDate = retryAfter.Date;
    }

    private void ParseRateLimitPolicy(string policyHeader)
    {
        var parts = policyHeader.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
            return;

        RateLimitPolicyName = parts[0].Trim('"');

        foreach (var part in parts.Skip(1))
        {
            if (part.StartsWith("q=", StringComparison.OrdinalIgnoreCase)
                && int.TryParse(part[2..], out var quota))
            {
                RateLimitQuota = quota;
            }
            else if (part.StartsWith("w=", StringComparison.OrdinalIgnoreCase)
                && int.TryParse(part[2..], out var window))
            {
                RateLimitWindowSeconds = window;
            }
        }
    }

    private void ParseRateLimit(string rateLimitHeader)
    {
        var parts = rateLimitHeader.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var part in parts.Skip(1))
        {
            if (part.StartsWith("r=", StringComparison.OrdinalIgnoreCase)
                && int.TryParse(part[2..], out var remaining))
            {
                RemainingRequests = remaining;
            }
            else if (part.StartsWith("t=", StringComparison.OrdinalIgnoreCase)
                && int.TryParse(part[2..], out var reset))
            {
                ResetTimeSeconds = reset;
            }
        }
    }
}
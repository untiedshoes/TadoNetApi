using System;
using System.Net;
using System.Net.Http;
using TadoNetApi.Infrastructure.Exceptions;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Exceptions;

/// <summary>
/// Unit tests for <see cref="RequestThrottledException"/>.
/// </summary>
public class RequestThrottledExceptionTests
{
    /// <summary>
    /// Constructor parses rate-limit headers and retry-after delta values.
    /// </summary>
    [Fact(DisplayName = "Constructor parses rate-limit headers and Retry-After delta")]
    public void Constructor_ParsesRateLimitHeaders_AndRetryAfterDelta()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/homes/1");
        var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
        {
            RequestMessage = request
        };

        response.Headers.TryAddWithoutValidation("RateLimit-Policy", "\"perday\";q=20000;w=86400");
        response.Headers.TryAddWithoutValidation("RateLimit", "\"perday\";r=0;t=7082");
        response.Headers.TryAddWithoutValidation("Retry-After", "2");

        var exception = new RequestThrottledException(request, response);

        Assert.Equal(HttpStatusCode.TooManyRequests, exception.StatusCode);
        Assert.Equal("https://example.test/homes/1", exception.RequestUri);
        Assert.Equal("perday", exception.RateLimitPolicyName);
        Assert.Equal(20000, exception.RateLimitQuota);
        Assert.Equal(86400, exception.RateLimitWindowSeconds);
        Assert.Equal(0, exception.RemainingRequests);
        Assert.Equal(7082, exception.ResetTimeSeconds);
        Assert.Equal(2, exception.RetryAfterSeconds);
    }

    /// <summary>
    /// Constructor parses Retry-After date and ignores malformed rate-limit values.
    /// </summary>
    [Fact(DisplayName = "Constructor parses Retry-After date and ignores malformed values")]
    public void Constructor_ParsesRetryAfterDate_AndIgnoresMalformedValues()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.test/homes/2");
        var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
        {
            RequestMessage = request
        };

        var retryDate = DateTimeOffset.UtcNow.AddMinutes(5);
        response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(retryDate);
        response.Headers.TryAddWithoutValidation("RateLimit-Policy", "\"perhour\";q=abc;w=xyz");
        response.Headers.TryAddWithoutValidation("RateLimit", "\"perhour\";r=abc;t=xyz");

        var exception = new RequestThrottledException(request, response);

        Assert.Equal("perhour", exception.RateLimitPolicyName);
        Assert.Null(exception.RateLimitQuota);
        Assert.Null(exception.RateLimitWindowSeconds);
        Assert.Null(exception.RemainingRequests);
        Assert.Null(exception.ResetTimeSeconds);
        Assert.Equal(retryDate, exception.RetryAfterDate);
    }
}
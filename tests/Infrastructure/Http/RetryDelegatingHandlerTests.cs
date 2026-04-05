using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging.Abstractions;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Exceptions;
using TadoNetApi.Infrastructure.Http;

namespace TadoNetApi.Tests.Infrastructure.Http;

public class RetryDelegatingHandlerTests
{
    [Fact(DisplayName = "RetryDelegatingHandler retries 429 responses and returns success")]
    public async Task SendAsync_RetriesTooManyRequests_AndReturnsSuccess()
    {
        var responses = new Queue<HttpResponseMessage>(
        [
            CreateResponse(HttpStatusCode.TooManyRequests, retryAfterSeconds: 1),
            CreateResponse(HttpStatusCode.OK)
        ]);

        var innerHandler = new SequenceHandler(responses);
        var handler = CreateHandler(innerHandler, maxRetries: 2, initialRetryDelayMs: 1);
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };

        using var response = await client.GetAsync("homes/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, innerHandler.CallCount);
    }

    [Fact(DisplayName = "RetryDelegatingHandler throws RequestThrottledException with rate-limit details")]
    public async Task SendAsync_ThrowsRequestThrottledException_WithRateLimitDetails()
    {
        var responses = new Queue<HttpResponseMessage>(
        [
            CreateResponse(
                HttpStatusCode.TooManyRequests,
                retryAfterSeconds: 1,
                rateLimitPolicy: "\"perday\";q=20000;w=86400",
                rateLimit: "\"perday\";r=0;t=7082"),
            CreateResponse(
                HttpStatusCode.TooManyRequests,
                retryAfterSeconds: 1,
                rateLimitPolicy: "\"perday\";q=20000;w=86400",
                rateLimit: "\"perday\";r=0;t=7082")
        ]);

        var innerHandler = new SequenceHandler(responses);
        var handler = CreateHandler(innerHandler, maxRetries: 1, initialRetryDelayMs: 1);
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };

        var exception = await Assert.ThrowsAsync<RequestThrottledException>(() => client.GetAsync("homes/1"));

        Assert.Equal(HttpStatusCode.TooManyRequests, exception.StatusCode);
        Assert.Equal("perday", exception.RateLimitPolicyName);
        Assert.Equal(20000, exception.RateLimitQuota);
        Assert.Equal(86400, exception.RateLimitWindowSeconds);
        Assert.Equal(0, exception.RemainingRequests);
        Assert.Equal(7082, exception.ResetTimeSeconds);
        Assert.Equal(1, exception.RetryAfterSeconds);
        Assert.Equal(2, innerHandler.CallCount);
    }

    private static RetryDelegatingHandler CreateHandler(HttpMessageHandler innerHandler, int maxRetries, int initialRetryDelayMs)
    {
        return new RetryDelegatingHandler(
            new TadoApiConfig
            {
                MaxRetries = maxRetries,
                InitialRetryDelayMs = initialRetryDelayMs
            },
            NullLogger<RetryDelegatingHandler>.Instance)
        {
            InnerHandler = innerHandler
        };
    }

    private static HttpResponseMessage CreateResponse(
        HttpStatusCode statusCode,
        int? retryAfterSeconds = null,
        string? rateLimitPolicy = null,
        string? rateLimit = null)
    {
        var response = new HttpResponseMessage(statusCode);

        if (retryAfterSeconds.HasValue)
            response.Headers.TryAddWithoutValidation("Retry-After", retryAfterSeconds.Value.ToString());

        if (!string.IsNullOrWhiteSpace(rateLimitPolicy))
            response.Headers.TryAddWithoutValidation("RateLimit-Policy", rateLimitPolicy);

        if (!string.IsNullOrWhiteSpace(rateLimit))
            response.Headers.TryAddWithoutValidation("RateLimit", rateLimit);

        return response;
    }

    private sealed class SequenceHandler(Queue<HttpResponseMessage> responses) : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> _responses = responses;

        public int CallCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;

            if (_responses.Count == 0)
                throw new InvalidOperationException("No more responses configured.");

            var response = _responses.Dequeue();
            response.RequestMessage = request;
            return Task.FromResult(response);
        }
    }
}
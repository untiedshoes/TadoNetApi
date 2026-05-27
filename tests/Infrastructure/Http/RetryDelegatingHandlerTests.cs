using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging.Abstractions;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Exceptions;
using TadoNetApi.Infrastructure.Http;

namespace TadoNetApi.Tests.Infrastructure.Http;

public class RetryDelegatingHandlerTests
{
    /// <summary>
    /// RetryDelegatingHandler retries 429 responses and returns success.
    /// </summary>
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

    /// <summary>
    /// RetryDelegatingHandler throws RequestThrottledException with rate-limit details.
    /// </summary>
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

    /// <summary>
    /// RetryDelegatingHandler returns the first non-429 response without retrying and preserves request content.
    /// </summary>
    [Fact(DisplayName = "RetryDelegatingHandler returns first non-429 response and clones request content")]
    public async Task SendAsync_ReturnsFirstNon429Response_AndClonesRequestContent()
    {
        var responses = new Queue<HttpResponseMessage>(
        [
            CreateResponse(HttpStatusCode.OK)
        ]);

        var innerHandler = new SequenceHandler(responses);
        var handler = CreateHandler(innerHandler, maxRetries: 2, initialRetryDelayMs: 1);
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };
        using var request = new HttpRequestMessage(HttpMethod.Post, "homes/1")
        {
            Content = new StringContent("{\"hello\":\"world\"}")
        };
        request.Headers.TryAddWithoutValidation("X-Test-Header", "alpha");
        request.Options.Set(new HttpRequestOptionsKey<string>("TraceId"), "trace-123");

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, innerHandler.CallCount);
        Assert.Single(innerHandler.Requests);
        Assert.Equal("{\"hello\":\"world\"}", Assert.Single(innerHandler.RequestBodies));
        Assert.Equal("alpha", Assert.Single(innerHandler.Requests[0].Headers.GetValues("X-Test-Header")));
        Assert.True(innerHandler.Requests[0].Options.TryGetValue(new HttpRequestOptionsKey<object?>("TraceId"), out var traceId));
        Assert.Equal("trace-123", traceId);
    }

    /// <summary>
    /// RetryDelegatingHandler falls back to configured delay when Retry-After is missing.
    /// </summary>
    [Fact(DisplayName = "RetryDelegatingHandler retries without Retry-After using configured fallback delay")]
    public async Task SendAsync_RetriesWithoutRetryAfter_UsingFallbackDelay()
    {
        var responses = new Queue<HttpResponseMessage>(
        [
            CreateResponse(HttpStatusCode.TooManyRequests),
            CreateResponse(HttpStatusCode.OK)
        ]);

        var innerHandler = new SequenceHandler(responses);
        var handler = CreateHandler(innerHandler, maxRetries: 2, initialRetryDelayMs: 1);
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };

        using var response = await client.GetAsync("homes/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, innerHandler.CallCount);
    }

    /// <summary>
    /// RetryDelegatingHandler honors Retry-After date headers when retrying.
    /// </summary>
    [Fact(DisplayName = "RetryDelegatingHandler retries using Retry-After date header")]
    public async Task SendAsync_RetriesUsingRetryAfterDateHeader()
    {
        var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
        response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddMilliseconds(50));

        var responses = new Queue<HttpResponseMessage>(
        [
            response,
            CreateResponse(HttpStatusCode.OK)
        ]);

        var innerHandler = new SequenceHandler(responses);
        var handler = CreateHandler(innerHandler, maxRetries: 2, initialRetryDelayMs: 1);
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };

        using var finalResponse = await client.GetAsync("homes/1");

        Assert.Equal(HttpStatusCode.OK, finalResponse.StatusCode);
        Assert.Equal(2, innerHandler.CallCount);
    }

    /// <summary>
    /// RetryDelegatingHandler falls back to configured delay when Retry-After date is in the past.
    /// </summary>
    [Fact(DisplayName = "RetryDelegatingHandler falls back when Retry-After date is in the past")]
    public async Task SendAsync_FallsBack_WhenRetryAfterDateIsInThePast()
    {
        var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
        response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddSeconds(-1));

        var responses = new Queue<HttpResponseMessage>(
        [
            response,
            CreateResponse(HttpStatusCode.OK)
        ]);

        var innerHandler = new SequenceHandler(responses);
        var handler = CreateHandler(innerHandler, maxRetries: 2, initialRetryDelayMs: 1);
        using var client = new HttpClient(handler) { BaseAddress = new Uri("https://example.test/") };

        using var finalResponse = await client.GetAsync("homes/1");

        Assert.Equal(HttpStatusCode.OK, finalResponse.StatusCode);
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

        public List<HttpRequestMessage> Requests { get; } = [];

        public List<string?> RequestBodies { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            Requests.Add(request);
            RequestBodies.Add(request.Content == null ? null : request.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult());

            if (_responses.Count == 0)
                throw new InvalidOperationException("No more responses configured.");

            var response = _responses.Dequeue();
            response.RequestMessage = request;
            return Task.FromResult(response);
        }
    }
}
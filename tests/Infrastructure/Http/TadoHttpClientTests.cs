using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using TadoNetApi.Infrastructure.Exceptions;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Tests.Fakes;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Http;

public class TadoHttpClientTests
{
    private static TadoHttpClient CreateClient(HttpMessageHandler fakeHandler)
    {
        var httpClient = new HttpClient(fakeHandler) { BaseAddress = new Uri("https://my.tado.com/api/v2/") };
        return new TadoHttpClient(httpClient, NullLogger<TadoHttpClient>.Instance);
    }

    private static HttpResponseMessage JsonResponse<T>(HttpStatusCode status, T body)
    {
        var json = JsonSerializer.Serialize(body);
        return new HttpResponseMessage(status)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    // -------------------------------------------------------------------------
    // GetAsync — happy path
    // -------------------------------------------------------------------------

    /// <summary>
    /// GetAsync deserializes and returns response body on 200 OK.
    /// </summary>
    [Fact(DisplayName = "GetAsync deserializes and returns response body on 200 OK")]
    public async Task GetAsync_ShouldReturnDeserializedBody_WhenApiReturns200()
    {
        var fake = new FakeHttpMessageHandler(_ => JsonResponse(HttpStatusCode.OK, new { name = "Living Room" }));
        var client = CreateClient(fake);

        var result = await client.GetAsync<JsonElement>("homes/1/zones/1", CancellationToken.None);

        Assert.Equal("Living Room", result.GetProperty("name").GetString());
    }

    // -------------------------------------------------------------------------
    // GetAsync — 4xx errors
    // -------------------------------------------------------------------------

    /// <summary>
    /// GetAsync throws TadoApiException with Unauthorized when API returns 401.
    /// </summary>
    [Fact(DisplayName = "GetAsync throws TadoApiException with Unauthorized when API returns 401")]
    public async Task GetAsync_ShouldThrowTadoApiException_WhenApiReturns401()
    {
        var fake = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Unauthorized));
        var client = CreateClient(fake);

        var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
            client.GetAsync<JsonElement>("homes/1/zones", CancellationToken.None));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    /// <summary>
    /// GetAsync throws TadoApiException with Forbidden when API returns 403.
    /// </summary>
    [Fact(DisplayName = "GetAsync throws TadoApiException with Forbidden when API returns 403")]
    public async Task GetAsync_ShouldThrowTadoApiException_WhenApiReturns403()
    {
        var fake = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.Forbidden));
        var client = CreateClient(fake);

        var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
            client.GetAsync<JsonElement>("homes/1/zones", CancellationToken.None));

        Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
    }

    /// <summary>
    /// GetAsync throws TadoApiException with NotFound when API returns 404.
    /// </summary>
    [Fact(DisplayName = "GetAsync throws TadoApiException with NotFound when API returns 404")]
    public async Task GetAsync_ShouldThrowTadoApiException_WhenApiReturns404()
    {
        var fake = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        var client = CreateClient(fake);

        var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
            client.GetAsync<JsonElement>("homes/1/zones/999", CancellationToken.None));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    // -------------------------------------------------------------------------
    // GetAsync — 5xx errors
    // -------------------------------------------------------------------------

    /// <summary>
    /// GetAsync throws TadoApiException with InternalServerError when API returns 500.
    /// </summary>
    [Fact(DisplayName = "GetAsync throws TadoApiException with InternalServerError when API returns 500")]
    public async Task GetAsync_ShouldThrowTadoApiException_WhenApiReturns500()
    {
        var fake = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError));
        var client = CreateClient(fake);

        var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
            client.GetAsync<JsonElement>("homes/1/zones", CancellationToken.None));

        Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
    }

    // -------------------------------------------------------------------------
    // GetAsync — timeout / cancellation
    // -------------------------------------------------------------------------

    /// <summary>
    /// GetAsync throws TadoApiException with RequestTimeout when request times out.
    /// </summary>
    [Fact(DisplayName = "GetAsync throws TadoApiException with RequestTimeout when request times out")]
    public async Task GetAsync_ShouldThrowTadoApiException_WhenRequestTimesOut()
    {
        // HttpClient translates its own timeout into a TaskCanceledException;
        // TadoHttpClient's catch block maps that to TadoApiException(RequestTimeout).
        var fake = new FakeHttpMessageHandler(_ => throw new TaskCanceledException("Request timed out"));
        var client = CreateClient(fake);

        var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
            client.GetAsync<JsonElement>("homes/1/zones", CancellationToken.None));

        Assert.Equal(HttpStatusCode.RequestTimeout, exception.StatusCode);
    }

    /// <summary>
    /// GetAsync throws TadoApiException with RequestTimeout when caller cancels (TadoHttpClient maps all TaskCanceledException to RequestTimeout).
    /// </summary>
    [Fact(DisplayName = "GetAsync throws TadoApiException with RequestTimeout when caller cancels (TadoHttpClient maps all TaskCanceledException to RequestTimeout)")]
    public async Task GetAsync_ShouldThrowTadoApiException_WhenCallerCancels()
    {
        // TadoHttpClient catches TaskCanceledException — which HttpClient raises for both
        // timeouts and external cancellation — and maps it to TadoApiException(RequestTimeout).
        // This means caller-initiated cancellation and timeout are indistinguishable at this layer.
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var fake = new FakeHttpMessageHandler(_ => JsonResponse(HttpStatusCode.OK, new { }));
        var client = CreateClient(fake);

        var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
            client.GetAsync<JsonElement>("homes/1/zones", cts.Token));

        Assert.Equal(HttpStatusCode.RequestTimeout, exception.StatusCode);
    }

    // -------------------------------------------------------------------------
    // GetAsync — malformed response
    // -------------------------------------------------------------------------

    /// <summary>
    /// GetAsync throws TadoApiException with UnprocessableEntity when response is not valid JSON.
    /// </summary>
    [Fact(DisplayName = "GetAsync throws TadoApiException with UnprocessableEntity when response is not valid JSON")]
    public async Task GetAsync_ShouldThrowTadoApiException_WhenResponseBodyIsInvalidJson()
    {
        var fake = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("not-json", Encoding.UTF8, "text/plain")
        });
        var client = CreateClient(fake);

        var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
            client.GetAsync<JsonElement>("homes/1/zones", CancellationToken.None));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, exception.StatusCode);
    }

    /// <summary>
    /// GetAsync throws TadoApiException with ServiceUnavailable when network fails.
    /// </summary>
    [Fact(DisplayName = "GetAsync throws TadoApiException with ServiceUnavailable when network fails")]
    public async Task GetAsync_ShouldThrowTadoApiException_WhenNetworkFails()
    {
        var fake = new FakeHttpMessageHandler(_ => throw new HttpRequestException("Connection refused"));
        var client = CreateClient(fake);

        var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
            client.GetAsync<JsonElement>("homes/1/zones", CancellationToken.None));

        Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
    }

    /// <summary>
    /// GetAsync throws TadoApiException with BadRequest when API returns 400.
    /// </summary>
    [Fact(DisplayName = "GetAsync throws TadoApiException with BadRequest when API returns 400")]
    public async Task GetAsync_ShouldThrowTadoApiException_WhenApiReturns400()
    {
        var fake = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.BadRequest));
        var client = CreateClient(fake);

        var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
            client.GetAsync<JsonElement>("homes/1/zones", CancellationToken.None));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    /// <summary>
    /// GetAsync throws TadoApiException when 200 response body deserializes to null.
    /// </summary>
    [Fact(DisplayName = "GetAsync throws TadoApiException when 200 response body deserializes to null")]
    public async Task GetAsync_ShouldThrowTadoApiException_WhenResponseBodyDeserializesToNull()
    {
        // JSON \"null\" deserializes to C# null for reference types.
        // TadoHttpClient throws using the response status code (200 OK) — a known inconsistency.
        var fake = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", Encoding.UTF8, "application/json")
        });
        var client = CreateClient(fake);

        var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
            client.GetAsync<object>("homes/1/zones", CancellationToken.None));

        Assert.Equal(HttpStatusCode.OK, exception.StatusCode);
    }
}

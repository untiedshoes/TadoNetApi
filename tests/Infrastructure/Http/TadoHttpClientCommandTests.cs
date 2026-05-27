using System;
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

/// <summary>
/// Tests command and write operations in <see cref="TadoHttpClient"/>.
/// </summary>
public class TadoHttpClientCommandTests
{
    /// <summary>
    /// SendAsync returns true when API returns the expected status code.
    /// </summary>
    [Fact(DisplayName = "SendAsync returns true when response matches expected status")]
    public async Task SendAsync_ReturnsTrue_WhenResponseMatchesExpectedStatus()
    {
        string? requestBody = null;
        HttpMethod? requestMethod = null;
        Uri? requestUri = null;

        var fake = new FakeHttpMessageHandler(request =>
        {
            requestMethod = request.Method;
            requestUri = request.RequestUri;
            requestBody = request.Content?.ReadAsStringAsync().GetAwaiter().GetResult();

            return new HttpResponseMessage(HttpStatusCode.NoContent)
            {
                Content = new StringContent(string.Empty)
            };
        });

        var client = CreateClient(fake);

        var result = await client.SendAsync(
            "homes/1/zones/2/overlay",
            HttpMethod.Delete,
            CancellationToken.None,
            HttpStatusCode.NoContent,
            new { reason = "cleanup" });

        Assert.True(result);
        Assert.Equal(HttpMethod.Delete, requestMethod);
        Assert.Equal("https://my.tado.com/api/v2/homes/1/zones/2/overlay", requestUri?.ToString());
        Assert.NotNull(requestBody);
        Assert.Contains("\"reason\":\"cleanup\"", requestBody);
    }

    /// <summary>
    /// SendAsync throws TadoApiException when actual status differs from expected status.
    /// </summary>
    [Fact(DisplayName = "SendAsync throws TadoApiException when response status does not match expected status")]
    public async Task SendAsync_ThrowsTadoApiException_WhenResponseStatusDiffersFromExpected()
    {
        var fake = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("unexpected")
        });

        var client = CreateClient(fake);

        var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
            client.SendAsync(
                "homes/1/presenceLock",
                HttpMethod.Put,
                CancellationToken.None,
                HttpStatusCode.NoContent,
                new { homePresence = "HOME" }));

        Assert.Equal(HttpStatusCode.OK, exception.StatusCode);
        Assert.Contains("unexpected", exception.Message);
    }

    /// <summary>
    /// SendAsync throws TadoApiException when the API returns NotFound instead of the expected status.
    /// </summary>
    [Fact(DisplayName = "SendAsync throws TadoApiException when NotFound differs from expected status")]
    public async Task SendAsync_ThrowsTadoApiException_WhenNotFoundDiffersFromExpectedStatus()
    {
        var fake = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("missing")
        });

        var client = CreateClient(fake);

        var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
            client.SendAsync(
                "homes/1/presenceLock",
                HttpMethod.Put,
                CancellationToken.None,
                HttpStatusCode.NoContent,
                new { homePresence = "HOME" }));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Contains("missing", exception.Message);
    }

    /// <summary>
    /// SendAsync throws TadoApiException with RequestTimeout when request is canceled or times out.
    /// </summary>
    [Fact(DisplayName = "SendAsync throws RequestTimeout when HTTP pipeline raises TaskCanceledException")]
    public async Task SendAsync_ThrowsRequestTimeout_WhenHttpPipelineRaisesTaskCanceledException()
    {
        var fake = new FakeHttpMessageHandler(_ => throw new TaskCanceledException("timed out"));
        var client = CreateClient(fake);

        var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
            client.SendAsync("homes/1/presenceLock", HttpMethod.Put, CancellationToken.None));

        Assert.Equal(HttpStatusCode.RequestTimeout, exception.StatusCode);
    }

    /// <summary>
    /// SendAsync throws TadoApiException with ServiceUnavailable when network request fails.
    /// </summary>
    [Fact(DisplayName = "SendAsync throws ServiceUnavailable when HTTP pipeline raises HttpRequestException")]
    public async Task SendAsync_ThrowsServiceUnavailable_WhenHttpPipelineRaisesHttpRequestException()
    {
        var fake = new FakeHttpMessageHandler(_ => throw new HttpRequestException("connection reset"));
        var client = CreateClient(fake);

        var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
            client.SendAsync("homes/1/presenceLock", HttpMethod.Put, CancellationToken.None));

        Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
        Assert.Contains("connection reset", exception.Message);
    }

    /// <summary>
    /// PostAsync sends JSON body and deserializes successful response payload.
    /// </summary>
    [Fact(DisplayName = "PostAsync sends JSON body and returns deserialized response payload")]
    public async Task PostAsync_SendsJsonAndReturnsDeserializedResponse()
    {
        string? requestBody = null;

        var fake = new FakeHttpMessageHandler(request =>
        {
            requestBody = request.Content?.ReadAsStringAsync().GetAwaiter().GetResult();

            var responsePayload = new { id = 123, status = "created" };
            return JsonResponse(HttpStatusCode.OK, responsePayload);
        });

        var client = CreateClient(fake);

        var result = await client.PostAsync<object, JsonElement>(
            "homes/1/test",
            new { name = "demo" },
            CancellationToken.None);

        Assert.Equal(123, result.GetProperty("id").GetInt32());
        Assert.Equal("created", result.GetProperty("status").GetString());
        Assert.NotNull(requestBody);
        Assert.Contains("\"name\":\"demo\"", requestBody);
    }

    /// <summary>
    /// PutAsync serializes JSON body when body is provided and returns deserialized response payload.
    /// </summary>
    [Fact(DisplayName = "PutAsync serializes provided body and returns deserialized response payload")]
    public async Task PutAsync_SerializesBodyAndReturnsDeserializedResponse()
    {
        string? requestBody = null;

        var fake = new FakeHttpMessageHandler(request =>
        {
            requestBody = request.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonResponse(HttpStatusCode.OK, new { accepted = true });
        });

        var client = CreateClient(fake);

        var result = await client.PutAsync<object, JsonElement>(
            "homes/1/test",
            new { enabled = true },
            CancellationToken.None);

        Assert.True(result.GetProperty("accepted").GetBoolean());
        Assert.NotNull(requestBody);
        Assert.Contains("\"enabled\":true", requestBody);
    }

    /// <summary>
    /// PutAsync omits request content when body is null and still deserializes successful response.
    /// </summary>
    [Fact(DisplayName = "PutAsync does not send content when body is null")]
    public async Task PutAsync_DoesNotSendContent_WhenBodyIsNull()
    {
        bool hadContent = true;

        var fake = new FakeHttpMessageHandler(request =>
        {
            hadContent = request.Content != null;
            return JsonResponse(HttpStatusCode.OK, new { done = true });
        });

        var client = CreateClient(fake);

        var result = await client.PutAsync<object?, JsonElement>(
            "homes/1/test",
            null,
            CancellationToken.None);

        Assert.True(result.GetProperty("done").GetBoolean());
        Assert.False(hadContent);
    }

    /// <summary>
    /// Creates a TadoHttpClient with a fake handler for deterministic HTTP responses.
    /// </summary>
    private static TadoHttpClient CreateClient(HttpMessageHandler fakeHandler)
    {
        var httpClient = new HttpClient(fakeHandler)
        {
            BaseAddress = new Uri("https://my.tado.com/api/v2/")
        };

        return new TadoHttpClient(httpClient, NullLogger<TadoHttpClient>.Instance);
    }

    /// <summary>
    /// Creates a JSON HTTP response from payload object.
    /// </summary>
    private static HttpResponseMessage JsonResponse<T>(HttpStatusCode statusCode, T payload)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json")
        };
    }
}

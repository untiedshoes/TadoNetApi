using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TadoNetApi.Tests.Fakes;

/// <summary>
/// A fake <see cref="HttpMessageHandler"/> that delegates response creation to a caller-supplied function.
/// Use this to test code that depends on <see cref="HttpClient"/> without hitting a real network.
/// </summary>
public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

    public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
    {
        _handler = handler;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_handler(request));
    }
}

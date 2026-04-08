using System.Net;
using System.Net.Http;

namespace TadoNetApi.Infrastructure.Http
{
    /// <summary>
    /// Defines HTTP operations for unauthenticated Tado endpoints that rely on bridge auth keys.
    /// </summary>
    public interface IPublicTadoHttpClient
    {
        Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest body, CancellationToken cancellationToken = default);
        Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest body, CancellationToken cancellationToken = default);
        Task<bool> SendAsync(string endpoint, HttpMethod method, CancellationToken cancellationToken = default, HttpStatusCode expectedStatusCode = HttpStatusCode.NoContent, object? body = null);
    }
}
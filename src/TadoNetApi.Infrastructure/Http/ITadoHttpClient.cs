using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace TadoNetApi.Infrastructure.Http
{
    /// <summary>
    /// Interface for the Tado HTTP client, defining methods for authenticated GET and POST requests.
    /// </summary>
    public interface ITadoHttpClient
    {
        Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest body, CancellationToken cancellationToken = default);
        Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest body, CancellationToken cancellationToken = default);
        Task<bool> SendAsync(string endpoint, HttpMethod method, CancellationToken cancellationToken = default, HttpStatusCode expectedStatusCode = HttpStatusCode.NoContent, object? body = null);
    }
}
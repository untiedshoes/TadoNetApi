using System.Threading;
using System.Threading.Tasks;

namespace TadoNetApi.Infrastructure.Http
{
    /// <summary>
    /// Interface for the Tado HTTP client, defining methods for authenticated GET and POST requests.
    /// </summary>
    public interface ITadoHttpClient
    {
        Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest body, CancellationToken cancellationToken = default);
    }
}
namespace TadoNetApi.Infrastructure.Auth
{
    /// <summary>
    /// Interface for TadoAuthService, enabling abstraction and easier testing.
    /// </summary>
    public interface ITadoAuthService
    {
        /// <summary>
        /// Indicates whether there is a valid access token.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Returns a valid access token, authorizing the device if necessary.
        /// </summary>
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Ensures the service is authenticated, refreshing token if needed.
        /// </summary>
        Task EnsureAuthenticatedAsync(CancellationToken cancellationToken = default);
    }
}
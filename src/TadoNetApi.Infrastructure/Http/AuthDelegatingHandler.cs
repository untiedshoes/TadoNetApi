using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TadoNetApi.Infrastructure.Auth;

namespace TadoNetApi.Infrastructure.Http
{
    /// <summary>
    /// DelegatingHandler responsible for attaching the OAuth2 Bearer token
    /// to all outgoing HTTP requests.
    /// 
    /// This ensures that:
    /// - Every request is authenticated
    /// - Token retrieval and refresh logic is centralized in TadoAuthService
    /// - TadoHttpClient remains focused purely on request/response handling
    /// 
    /// This handler is part of the HttpClient pipeline and runs automatically
    /// for every request.
    /// </summary>
    public class AuthDelegatingHandler : DelegatingHandler
    {
        private readonly TadoAuthService _authService;

        /// <summary>
        /// Constructs the handler with the TadoAuthService dependency.
        /// </summary>
        public AuthDelegatingHandler(TadoAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Intercepts outgoing HTTP requests and adds the Authorization header.
        /// 
        /// Flow:
        /// 1. Retrieve a valid access token (refresh if needed)
        /// 2. Attach Bearer token to request headers
        /// 3. Continue the pipeline
        /// </summary>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Ensure we have a valid access token
            var token = await _authService.GetAccessTokenAsync(cancellationToken);

            // Attach Bearer token to request
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Continue request pipeline
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
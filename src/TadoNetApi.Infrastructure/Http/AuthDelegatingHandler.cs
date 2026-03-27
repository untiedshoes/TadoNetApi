using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using TadoNetApi.Infrastructure.Auth;

namespace TadoNetApi.Infrastructure.Http
{
    /// <summary>
    /// Attaches Bearer token to outgoing requests.
    /// DOES NOT handle authentication flow.
    /// </summary>
    public class AuthDelegatingHandler : DelegatingHandler
    {
        private readonly ITadoAuthService _authService;

        public AuthDelegatingHandler(ITadoAuthService authService)
        {
            _authService = authService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = await _authService.GetAccessTokenAsync(cancellationToken);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
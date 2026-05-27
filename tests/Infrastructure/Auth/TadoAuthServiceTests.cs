using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TadoNetApi.Infrastructure.Auth;
using TadoNetApi.Infrastructure.Config;
using TadoNetApi.Infrastructure.Dtos.Auth;
using TadoNetApi.Tests.Fakes;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Auth
{
    /// <summary>
    /// Tests for <see cref="TadoAuthService"/> covering device authorization,
    /// token polling, and refresh behavior.
    /// </summary>
    public class TadoAuthServiceTests
    {
        /// <summary>
        /// StartDeviceAuthorisationAsync returns parsed device authorization details and posts expected form data.
        /// </summary>
        [Fact(DisplayName = "StartDeviceAuthorisationAsync returns parsed response and sends expected form data")]
        public async Task StartDeviceAuthorisationAsync_ReturnsParsedResponseAndSendsExpectedFormData()
        {
            string? postedBody = null;
            Uri? postedUri = null;

            var service = CreateService(request =>
            {
                postedUri = request.RequestUri;
                postedBody = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();

                var payload = new DeviceCodeResponse
                {
                    DeviceCode = "device-123",
                    UserCode = "user-abc",
                    VerificationUri = "https://login.tado.com/activate",
                    VerificationUriComplete = "https://login.tado.com/activate?code=user-abc",
                    ExpiresIn = 600,
                    Interval = 5
                };

                return JsonResponse(HttpStatusCode.OK, payload);
            });

            var result = await service.StartDeviceAuthorisationAsync(CancellationToken.None);

            Assert.Equal("device-123", result.DeviceCode);
            Assert.Equal("user-abc", result.UserCode);
            Assert.Equal(600, result.ExpiresIn);
            Assert.Equal(5, result.Interval);
            Assert.Equal(TadoApiEndpoints.DeviceAuthorizeUrl, postedUri?.ToString());
            Assert.NotNull(postedBody);
            Assert.Contains("client_id=1bb50063-6b0c-4d11-bd99-387f4a91cc46", postedBody);
            Assert.Contains("scope=home.user", postedBody);
        }

        /// <summary>
        /// WaitForDeviceTokenAsync returns token and GetAccessTokenAsync reuses the same non-expired token.
        /// </summary>
        [Fact(DisplayName = "WaitForDeviceTokenAsync returns token and GetAccessTokenAsync reuses non-expired token")]
        public async Task WaitForDeviceTokenAsync_ReturnsTokenAndGetAccessTokenAsync_ReusesNonExpiredToken()
        {
            var tokenPayload = new TadoAuthResponse
            {
                AccessToken = "access-token",
                RefreshToken = "refresh-token",
                ExpiresIn = 3600
            };

            var service = CreateService(_ => JsonResponse(HttpStatusCode.OK, tokenPayload));

            var token = await service.WaitForDeviceTokenAsync("device-123", pollingIntervalSeconds: 0, expiresInSeconds: 30, CancellationToken.None);
            var accessToken = await service.GetAccessTokenAsync(CancellationToken.None);

            Assert.Equal("access-token", token.AccessToken);
            Assert.Equal("access-token", accessToken);
        }

        /// <summary>
        /// WaitForDeviceTokenAsync throws HttpRequestException when OAuth returns unrecoverable error payload.
        /// </summary>
        [Fact(DisplayName = "WaitForDeviceTokenAsync throws HttpRequestException on unrecoverable OAuth error")]
        public async Task WaitForDeviceTokenAsync_ThrowsHttpRequestException_OnUnrecoverableOAuthError()
        {
            var errorPayload = new TokenErrorResponse
            {
                Error = "access_denied",
                ErrorDescription = "User denied access"
            };

            var service = CreateService(_ => JsonResponse(HttpStatusCode.BadRequest, errorPayload));

            var exception = await Assert.ThrowsAsync<HttpRequestException>(() =>
                service.WaitForDeviceTokenAsync("device-123", pollingIntervalSeconds: 0, expiresInSeconds: 30, CancellationToken.None));

            Assert.Contains("OAuth error: access_denied", exception.Message);
        }

        /// <summary>
        /// WaitForDeviceTokenAsync throws TimeoutException when max wait is already elapsed.
        /// </summary>
        [Fact(DisplayName = "WaitForDeviceTokenAsync throws TimeoutException when wait window is elapsed")]
        public async Task WaitForDeviceTokenAsync_ThrowsTimeoutException_WhenWaitWindowElapsed()
        {
            var service = CreateService(_ => JsonResponse(HttpStatusCode.OK, new { }));

            var exception = await Assert.ThrowsAsync<TimeoutException>(() =>
                service.WaitForDeviceTokenAsync("device-123", pollingIntervalSeconds: 0, expiresInSeconds: 0, CancellationToken.None));

            Assert.Equal("Device authorisation timed out.", exception.Message);
        }

        /// <summary>
        /// GetAccessTokenAsync refreshes token when current token is expired and refresh succeeds.
        /// </summary>
        [Fact(DisplayName = "GetAccessTokenAsync refreshes expired token when refresh succeeds")]
        public async Task GetAccessTokenAsync_RefreshesExpiredToken_WhenRefreshSucceeds()
        {
            var postedBodies = new List<string>();
            var callCount = 0;

            var service = CreateService(request =>
            {
                callCount++;
                postedBodies.Add(request.Content!.ReadAsStringAsync().GetAwaiter().GetResult());

                if (callCount == 1)
                {
                    return JsonResponse(HttpStatusCode.OK, new TadoAuthResponse
                    {
                        AccessToken = "initial-access",
                        RefreshToken = "refresh-123",
                        ExpiresIn = -1
                    });
                }

                return JsonResponse(HttpStatusCode.OK, new TadoAuthResponse
                {
                    AccessToken = "refreshed-access",
                    RefreshToken = "refresh-456",
                    ExpiresIn = 3600
                });
            });

            await service.WaitForDeviceTokenAsync("device-123", pollingIntervalSeconds: 0, expiresInSeconds: 30, CancellationToken.None);
            var refreshedAccessToken = await service.GetAccessTokenAsync(CancellationToken.None);

            Assert.Equal("refreshed-access", refreshedAccessToken);
            Assert.Equal(2, callCount);
            Assert.Contains("grant_type=refresh_token", postedBodies[1]);
            Assert.Contains("refresh_token=refresh-123", postedBodies[1]);
        }

        /// <summary>
        /// GetAccessTokenAsync throws InvalidOperationException when refresh endpoint rejects refresh request.
        /// </summary>
        [Fact(DisplayName = "GetAccessTokenAsync throws InvalidOperationException when refresh is rejected")]
        public async Task GetAccessTokenAsync_ThrowsInvalidOperationException_WhenRefreshIsRejected()
        {
            var callCount = 0;

            var service = CreateService(_ =>
            {
                callCount++;

                if (callCount == 1)
                {
                    return JsonResponse(HttpStatusCode.OK, new TadoAuthResponse
                    {
                        AccessToken = "initial-access",
                        RefreshToken = "refresh-123",
                        ExpiresIn = -1
                    });
                }

                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("{}", Encoding.UTF8, "application/json")
                };
            });

            await service.WaitForDeviceTokenAsync("device-123", pollingIntervalSeconds: 0, expiresInSeconds: 30, CancellationToken.None);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.GetAccessTokenAsync(CancellationToken.None));

            Assert.Equal("Re-authorisation required", exception.Message);
        }

        /// <summary>
        /// GetAccessTokenAsync throws InvalidOperationException when token is expired and no refresh token is present.
        /// </summary>
        [Fact(DisplayName = "GetAccessTokenAsync throws InvalidOperationException when refresh token is missing")]
        public async Task GetAccessTokenAsync_ThrowsInvalidOperationException_WhenRefreshTokenMissing()
        {
            var service = CreateService(_ => JsonResponse(HttpStatusCode.OK, new TadoAuthResponse
            {
                AccessToken = "initial-access",
                RefreshToken = string.Empty,
                ExpiresIn = -1
            }));

            await service.WaitForDeviceTokenAsync("device-123", pollingIntervalSeconds: 0, expiresInSeconds: 30, CancellationToken.None);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.GetAccessTokenAsync(CancellationToken.None));

            Assert.Equal("No refresh token available", exception.Message);
        }

        /// <summary>
        /// GetAccessTokenAsync waits for initial authorisation and respects cancellation while waiting.
        /// </summary>
        [Fact(DisplayName = "GetAccessTokenAsync respects cancellation while waiting for initial authorisation")]
        public async Task GetAccessTokenAsync_RespectsCancellation_WhileWaitingForInitialAuthorisation()
        {
            var service = CreateService(_ => JsonResponse(HttpStatusCode.OK, new { }));
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                service.GetAccessTokenAsync(cancellationTokenSource.Token));
        }

        /// <summary>
        /// WaitForDeviceTokenAsync retries authorization-pending and service-unavailable responses until success.
        /// </summary>
        [Fact(DisplayName = "WaitForDeviceTokenAsync retries authorization pending and service unavailable responses until success")]
        public async Task WaitForDeviceTokenAsync_RetriesAuthorizationPendingAndServiceUnavailable_UntilSuccess()
        {
            var callCount = 0;
            var service = CreateService(_ =>
            {
                callCount++;

                return callCount switch
                {
                    1 => JsonResponse(HttpStatusCode.BadRequest, new TokenErrorResponse
                    {
                        Error = "authorization_pending",
                        ErrorDescription = "Waiting"
                    }),
                    2 => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                    {
                        Content = new StringContent("{}", Encoding.UTF8, "application/json")
                    },
                    _ => JsonResponse(HttpStatusCode.OK, new TadoAuthResponse
                    {
                        AccessToken = "access-token",
                        RefreshToken = "refresh-token",
                        ExpiresIn = 3600
                    })
                };
            });

            var token = await service.WaitForDeviceTokenAsync("device-123", pollingIntervalSeconds: 0, expiresInSeconds: 30, CancellationToken.None);

            Assert.Equal("access-token", token.AccessToken);
            Assert.Equal(3, callCount);
        }

        /// <summary>
        /// GetAccessTokenAsync throws InvalidOperationException when refresh response cannot be parsed.
        /// </summary>
        [Fact(DisplayName = "GetAccessTokenAsync throws InvalidOperationException when refresh response cannot be parsed")]
        public async Task GetAccessTokenAsync_ThrowsInvalidOperationException_WhenRefreshResponseCannotBeParsed()
        {
            var callCount = 0;

            var service = CreateService(_ =>
            {
                callCount++;

                if (callCount == 1)
                {
                    return JsonResponse(HttpStatusCode.OK, new TadoAuthResponse
                    {
                        AccessToken = "initial-access",
                        RefreshToken = "refresh-123",
                        ExpiresIn = -1
                    });
                }

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("null", Encoding.UTF8, "application/json")
                };
            });

            await service.WaitForDeviceTokenAsync("device-123", pollingIntervalSeconds: 0, expiresInSeconds: 30, CancellationToken.None);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.GetAccessTokenAsync(CancellationToken.None));

            Assert.Equal("Failed to refresh token", exception.Message);
        }

        /// <summary>
        /// Creates a TadoAuthService with fake HTTP behavior for deterministic test scenarios.
        /// </summary>
        /// <param name="handler">Function that maps request messages to mocked responses.</param>
        /// <returns>A configured <see cref="TadoAuthService"/> instance.</returns>
        private static TadoAuthService CreateService(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            var httpClient = new HttpClient(new FakeHttpMessageHandler(handler));
            var factory = new Mock<IHttpClientFactory>();
            factory
                .Setup(f => f.CreateClient("TadoAuth"))
                .Returns(httpClient);

            return new TadoAuthService(factory.Object, NullLogger<TadoAuthService>.Instance);
        }

        /// <summary>
        /// Creates an HTTP response containing JSON payload.
        /// </summary>
        /// <typeparam name="T">Payload type to serialize.</typeparam>
        /// <param name="statusCode">HTTP status code for the response.</param>
        /// <param name="payload">Payload object to serialize into JSON response body.</param>
        /// <returns>A JSON HTTP response message.</returns>
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
}

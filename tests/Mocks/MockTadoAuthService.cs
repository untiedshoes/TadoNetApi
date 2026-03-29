using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Infrastructure.Auth;
using TadoNetApi.Infrastructure.Dtos.Auth;

namespace TadoNetApi.Tests.Mocks
{
    public static class MockTadoAuthService
    {
        public static Mock<ITadoAuthService> CreateAuthenticated(string token = "mock-token")
        {
            var mock = new Mock<ITadoAuthService>();
            mock.Setup(a => a.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(token);
            return mock;
        }

        public static Mock<ITadoAuthService> CreateUnauthenticated()
        {
            var mock = new Mock<ITadoAuthService>();
            mock.Setup(a => a.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Access token unavailable. Start device flow first."));
            return mock;
        }

        /// <summary>
        /// Mock auth service that simulates a full device authorisation flow
        /// </summary>
        public static Mock<ITadoAuthService> CreateDeviceFlowMock(
            string deviceCode = "mock-device-code",
            string userCode = "MOCK123",
            string verificationUri = "https://login.tado.com/oauth2/device",
            string verificationUriComplete = "https://login.tado.com/oauth2/device?user_code=MOCK123",
            int pollingIntervalSeconds = 5,
            int expiresInSeconds = 300,
            string accessToken = "mock-access-token")
        {
            var mock = new Mock<ITadoAuthService>();

            // Start device authorisation
            mock.Setup(a => a.StartDeviceAuthorisationAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeviceCodeResponse
                {
                    DeviceCode = deviceCode,
                    UserCode = userCode,
                    VerificationUri = verificationUri,
                    VerificationUriComplete = verificationUriComplete,
                    Interval = pollingIntervalSeconds,
                    ExpiresIn = expiresInSeconds
                });

            // Wait for device token
            mock.Setup(a => a.WaitForDeviceTokenAsync(
                    deviceCode,
                    pollingIntervalSeconds,
                    expiresInSeconds,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TadoAuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = "mock-refresh-token",
                    ExpiresIn = 3600
                });

            // Get access token
            mock.Setup(a => a.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(accessToken);

            return mock;
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Infrastructure.Auth;

namespace TadoNetApi.Tests.Mocks
{
    public static class MockTadoAuthService
    {
        /// <summary>
        /// Mock auth service that always returns a valid token
        /// </summary>
        public static Mock<ITadoAuthService> CreateAuthenticated(string token = "mock-token")
        {
            var mock = new Mock<ITadoAuthService>();
            mock.Setup(a => a.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(token);

            return mock;
        }

        /// <summary>
        /// Mock auth service that simulates token not available or expired
        /// </summary>
        public static Mock<ITadoAuthService> CreateUnauthenticated()
        {
            var mock = new Mock<ITadoAuthService>();
            mock.Setup(a => a.GetAccessTokenAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            return mock;
        }
    }
}
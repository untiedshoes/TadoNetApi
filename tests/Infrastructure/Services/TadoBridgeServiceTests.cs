using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Exceptions;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Services;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Services
{
    public class TadoBridgeServiceTests
    {
        /// <summary>
        /// GetBridgeAsync returns mapped bridge.
        /// </summary>
        [Fact(DisplayName = "GetBridgeAsync returns mapped bridge")]
        public async Task GetBridgeAsync_ReturnsMappedBridge()
        {
            var response = new TadoBridgeResponse
            {
                HomeId = 1522717,
                Partner = new object()
            };

            var mockHttp = new Mock<IPublicTadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoBridgeResponse>("bridges/IB3328595968?authKey=1234", It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var service = new TadoBridgeService(mockHttp.Object);

            var bridge = await service.GetBridgeAsync("IB3328595968", "1234", CancellationToken.None);

            Assert.NotNull(bridge);
            Assert.Equal(1522717, bridge!.HomeId);
            Assert.NotNull(bridge.Partner);
        }

        /// <summary>
        /// GetBridgeAsync throws TadoApiException when API returns Unauthorized.
        /// </summary>
        [Fact(DisplayName = "GetBridgeAsync throws TadoApiException when API returns Unauthorized")]
        public async Task GetBridgeAsync_ShouldThrowTadoApiException_WhenApiReturnsUnauthorized()
        {
            var mockHttp = new Mock<IPublicTadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoBridgeResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TadoApiException(HttpStatusCode.Unauthorized, "Unauthorized"));

            var service = new TadoBridgeService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetBridgeAsync("IB3328595968", "1234", CancellationToken.None));

            Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        }

        /// <summary>
        /// GetBridgeAsync throws TadoApiException with ServiceUnavailable when network fails.
        /// </summary>
        [Fact(DisplayName = "GetBridgeAsync throws TadoApiException with ServiceUnavailable when network fails")]
        public async Task GetBridgeAsync_ShouldThrowTadoApiException_WhenNetworkFails()
        {
            var mockHttp = new Mock<IPublicTadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoBridgeResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Network failed"));

            var service = new TadoBridgeService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetBridgeAsync("IB3328595968", "1234", CancellationToken.None));

            Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
        }
    }
}
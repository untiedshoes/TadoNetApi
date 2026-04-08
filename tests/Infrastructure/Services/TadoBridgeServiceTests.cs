using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Services;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Services
{
    public class TadoBridgeServiceTests
    {
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
    }
}
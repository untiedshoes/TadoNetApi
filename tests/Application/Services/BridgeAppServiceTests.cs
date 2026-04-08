using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Application.Services;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;
using Xunit;

namespace TadoNetApi.Tests.Application.Services
{
    public class BridgeAppServiceTests
    {
        [Fact(DisplayName = "GetBridgeAsync returns bridge")]
        public async Task GetBridgeAsync_ReturnsBridge()
        {
            var expectedBridge = new Bridge
            {
                HomeId = 1522717,
                Partner = new object()
            };

            var mockBridgeService = new Mock<IBridgeService>();
            mockBridgeService
                .Setup(s => s.GetBridgeAsync("IB3328595968", "1234", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedBridge);

            var service = new BridgeAppService(mockBridgeService.Object);

            var bridge = await service.GetBridgeAsync("IB3328595968", "1234", CancellationToken.None);

            Assert.NotNull(bridge);
            Assert.Equal(1522717, bridge?.HomeId);
            Assert.NotNull(bridge?.Partner);
        }
    }
}
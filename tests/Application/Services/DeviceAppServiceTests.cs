using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Application.Services;
using TadoNetApi.Domain.Entities.MobileDevice;
using TadoNetApi.Domain.Interfaces;
using Xunit;

namespace TadoNetApi.Tests.Application.Services
{
    public class DeviceAppServiceTests
    {
        [Fact]
        public async Task GetMobileDeviceAsync_ReturnsMappedMobileDevice()
        {
            // Arrange
            var expectedDevice = new Item
            {
                Id = 42,
                Name = "Craig's iPhone",
                Settings = new Settings { GeoTrackingEnabled = true }
            };

            var mockDeviceService = new Mock<IDeviceService>();
            mockDeviceService.Setup(s => s.GetMobileDeviceAsync(1, 42, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDevice);

            var service = new DeviceAppService(mockDeviceService.Object);

            // Act
            var device = await service.GetMobileDeviceAsync(1, 42, CancellationToken.None);

            // Assert
            Assert.NotNull(device);
            Assert.Equal(42, device.Id);
            Assert.Equal("Craig's iPhone", device.Name);
        }

        [Fact]
        public async Task GetZoneMeasuringDeviceAsync_ReturnsMappedDevice()
        {
            // Arrange
            var expectedDevice = new TadoNetApi.Domain.Entities.Device
            {
                SerialNo = "SU1234567890",
                ShortSerialNo = "SU1234567890",
                DeviceType = "SU02"
            };

            var mockDeviceService = new Mock<IDeviceService>();
            mockDeviceService.Setup(s => s.GetZoneMeasuringDeviceAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDevice);

            var service = new DeviceAppService(mockDeviceService.Object);

            // Act
            var device = await service.GetZoneMeasuringDeviceAsync(1, 2, CancellationToken.None);

            // Assert
            Assert.NotNull(device);
            Assert.Equal("SU1234567890", device.SerialNo);
            Assert.Equal("SU02", device.DeviceType);
        }
    }
}
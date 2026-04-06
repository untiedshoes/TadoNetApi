using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Application.Services;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Entities.MobileDevice;
using TadoNetApi.Domain.Interfaces;
using Xunit;

namespace TadoNetApi.Tests.Application.Services
{
    public class DeviceAppServiceTests
    {
        /// <summary>
        /// Tests that <see cref="DeviceAppService.GetDeviceListAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "GetDeviceListAsync passes through to domain service")]
        public async Task GetDeviceListAsync_PassesThroughToDomainService()
        {
            var expectedEntries = new[]
            {
                new DeviceListEntry
                {
                    Type = "SU02",
                    Device = new Device
                    {
                        SerialNo = "SU3339800320",
                        DeviceType = "SU02"
                    },
                    ZoneId = 3,
                    ZoneDuties = ["UI"]
                }
            };

            var mockDeviceService = new Mock<IDeviceService>();
            mockDeviceService.Setup(s => s.GetDeviceListAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedEntries);

            var service = new DeviceAppService(mockDeviceService.Object);

            var entries = await service.GetDeviceListAsync(1, CancellationToken.None);

            var entry = Assert.Single(entries);
            Assert.Equal("SU02", entry.Type);
            Assert.Equal("SU3339800320", entry.Device?.SerialNo);
            Assert.Equal(3, entry.ZoneId);
            mockDeviceService.Verify(s => s.GetDeviceListAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="DeviceAppService.GetDeviceListAsync"/> preserves domain-service exceptions.
        /// </summary>
        [Fact(DisplayName = "GetDeviceListAsync preserves domain service exceptions")]
        public async Task GetDeviceListAsync_PreservesDomainServiceExceptions()
        {
            var expectedException = new InvalidOperationException("device list unavailable");

            var mockDeviceService = new Mock<IDeviceService>();
            mockDeviceService.Setup(s => s.GetDeviceListAsync(1, It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            var service = new DeviceAppService(mockDeviceService.Object);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.GetDeviceListAsync(1, CancellationToken.None));

            Assert.Same(expectedException, exception);
            mockDeviceService.Verify(s => s.GetDeviceListAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="DeviceAppService.GetDeviceAsync(string, CancellationToken)"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "GetDeviceAsync string overload passes through to domain service")]
        public async Task GetDeviceAsync_StringOverload_PassesThroughToDomainService()
        {
            var expectedDevice = new Device
            {
                SerialNo = "SU1234567890",
                DeviceType = "SU02"
            };

            var mockDeviceService = new Mock<IDeviceService>();
            mockDeviceService.Setup(s => s.GetDeviceAsync("SU1234567890", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDevice);

            var service = new DeviceAppService(mockDeviceService.Object);

            var device = await service.GetDeviceAsync("SU1234567890", CancellationToken.None);

            Assert.NotNull(device);
            Assert.Equal("SU1234567890", device.SerialNo);
            mockDeviceService.Verify(s => s.GetDeviceAsync("SU1234567890", It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="DeviceAppService.GetMobileDeviceAsync"/> returns the mobile device from the domain service.
        /// </summary>
        [Fact(DisplayName = "GetMobileDeviceAsync returns mapped mobile device")]
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

        /// <summary>
        /// Tests that <see cref="DeviceAppService.GetZoneMeasuringDeviceAsync"/> returns the measuring device from the domain service.
        /// </summary>
        [Fact(DisplayName = "GetZoneMeasuringDeviceAsync returns mapped device")]
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

        /// <summary>
        /// Tests that <see cref="DeviceAppService.MoveDeviceToZoneAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "MoveDeviceToZoneAsync passes through to domain service")]
        public async Task MoveDeviceToZoneAsync_PassesThroughToDomainService()
        {
            var mockDeviceService = new Mock<IDeviceService>();
            mockDeviceService.Setup(s => s.MoveDeviceToZoneAsync(1, 2, "SU1234567890", true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var service = new DeviceAppService(mockDeviceService.Object);

            var moved = await service.MoveDeviceToZoneAsync(1, 2, "SU1234567890", true, CancellationToken.None);

            Assert.True(moved);
            mockDeviceService.Verify(s => s.MoveDeviceToZoneAsync(1, 2, "SU1234567890", true, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="DeviceAppService.SetZoneMeasuringDeviceAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "SetZoneMeasuringDeviceAsync passes through to domain service")]
        public async Task SetZoneMeasuringDeviceAsync_PassesThroughToDomainService()
        {
            var expectedDevice = new Device
            {
                SerialNo = "SU1234567890",
                DeviceType = "SU02"
            };

            var mockDeviceService = new Mock<IDeviceService>();
            mockDeviceService.Setup(s => s.SetZoneMeasuringDeviceAsync(1, 2, "SU1234567890", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDevice);

            var service = new DeviceAppService(mockDeviceService.Object);

            var device = await service.SetZoneMeasuringDeviceAsync(1, 2, "SU1234567890", CancellationToken.None);

            Assert.NotNull(device);
            Assert.Equal("SU1234567890", device.SerialNo);
            mockDeviceService.Verify(s => s.SetZoneMeasuringDeviceAsync(1, 2, "SU1234567890", It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="DeviceAppService.DeleteMobileDeviceAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "DeleteMobileDeviceAsync passes through to domain service")]
        public async Task DeleteMobileDeviceAsync_PassesThroughToDomainService()
        {
            var mockDeviceService = new Mock<IDeviceService>();
            mockDeviceService.Setup(s => s.DeleteMobileDeviceAsync(1, 42, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var service = new DeviceAppService(mockDeviceService.Object);

            var deleted = await service.DeleteMobileDeviceAsync(1, 42, CancellationToken.None);

            Assert.True(deleted);
            mockDeviceService.Verify(s => s.DeleteMobileDeviceAsync(1, 42, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="DeviceAppService.SetZoneTemperatureOffsetFahrenheitAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "SetZoneTemperatureOffsetFahrenheitAsync passes through to domain service")]
        public async Task SetZoneTemperatureOffsetFahrenheitAsync_PassesThroughToDomainService()
        {
            var mockDeviceService = new Mock<IDeviceService>();
            mockDeviceService.Setup(s => s.SetZoneTemperatureOffsetFahrenheitAsync("ABC123", -2.25, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var service = new DeviceAppService(mockDeviceService.Object);

            var updated = await service.SetZoneTemperatureOffsetFahrenheitAsync("ABC123", -2.25, CancellationToken.None);

            Assert.True(updated);
            mockDeviceService.Verify(s => s.SetZoneTemperatureOffsetFahrenheitAsync("ABC123", -2.25, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="DeviceAppService.SetMobileDeviceSettingsAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "SetMobileDeviceSettingsAsync passes through to domain service")]
        public async Task SetMobileDeviceSettingsAsync_PassesThroughToDomainService()
        {
            var settings = new Settings { GeoTrackingEnabled = true };
            var mockDeviceService = new Mock<IDeviceService>();
            mockDeviceService.Setup(s => s.SetMobileDeviceSettingsAsync(1, 42, settings, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var service = new DeviceAppService(mockDeviceService.Object);

            var updated = await service.SetMobileDeviceSettingsAsync(1, 42, settings, CancellationToken.None);

            Assert.True(updated);
            mockDeviceService.Verify(s => s.SetMobileDeviceSettingsAsync(1, 42, settings, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="DeviceAppService.GetDeviceAsync(int, int, CancellationToken)"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "GetDeviceAsync legacy overload passes through to domain service")]
        public async Task GetDeviceAsync_LegacyOverload_PassesThroughToDomainService()
        {
            var expectedDevice = new Device
            {
                SerialNo = "123",
                DeviceType = "SU02"
            };

            var mockDeviceService = new Mock<IDeviceService>();
            mockDeviceService.Setup(s => s.GetDeviceAsync(1, 123, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDevice);

            var service = new DeviceAppService(mockDeviceService.Object);

            var device = await service.GetDeviceAsync(1, 123, CancellationToken.None);

            Assert.NotNull(device);
            Assert.Equal("123", device.SerialNo);
            mockDeviceService.Verify(s => s.GetDeviceAsync(1, 123, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
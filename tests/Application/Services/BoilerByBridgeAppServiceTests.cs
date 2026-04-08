using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Application.Services;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Interfaces;
using Xunit;

namespace TadoNetApi.Tests.Application.Services
{
    public class BoilerByBridgeAppServiceTests
    {
        [Fact(DisplayName = "GetBoilerInfoAsync returns boiler info")]
        public async Task GetBoilerInfoAsync_ReturnsBoilerInfo()
        {
            var expectedBoilerInfo = new BoilerInfo
            {
                BoilerPresent = true,
                BoilerId = 2699
            };

            var mockService = new Mock<IBoilerByBridgeService>();
            mockService
                .Setup(s => s.GetBoilerInfoAsync("IB3328595968", "1234", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedBoilerInfo);

            var service = new BoilerByBridgeAppService(mockService.Object);

            var boilerInfo = await service.GetBoilerInfoAsync("IB3328595968", "1234", CancellationToken.None);

            Assert.NotNull(boilerInfo);
            Assert.True(boilerInfo?.BoilerPresent);
            Assert.Equal(2699, boilerInfo?.BoilerId);
        }

        [Fact(DisplayName = "GetBoilerMaxOutputTemperatureAsync returns max output temperature")]
        public async Task GetBoilerMaxOutputTemperatureAsync_ReturnsMaxOutputTemperature()
        {
            var expectedTemperature = new BoilerMaxOutputTemperature
            {
                BoilerMaxOutputTemperatureInCelsius = 55
            };

            var mockService = new Mock<IBoilerByBridgeService>();
            mockService
                .Setup(s => s.GetBoilerMaxOutputTemperatureAsync("IB3328595968", "1234", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTemperature);

            var service = new BoilerByBridgeAppService(mockService.Object);

            var temperature = await service.GetBoilerMaxOutputTemperatureAsync("IB3328595968", "1234", CancellationToken.None);

            Assert.NotNull(temperature);
            Assert.Equal(55, temperature?.BoilerMaxOutputTemperatureInCelsius);
        }

        [Fact(DisplayName = "GetBoilerWiringInstallationStateAsync returns wiring installation state")]
        public async Task GetBoilerWiringInstallationStateAsync_ReturnsWiringInstallationState()
        {
            var expectedState = new BoilerWiringInstallationState
            {
                State = "INSTALLATION_COMPLETED",
                BridgeConnected = true,
                DeviceWiredToBoiler = new BoilerWiredDevice
                {
                    Type = "BR02",
                    SerialNo = "BR123456789",
                    ThermInterfaceType = "UBA_BUS",
                    Connected = true,
                    LastRequestTimestamp = new DateTime(2024, 12, 30, 11, 10, 24, DateTimeKind.Utc)
                }
            };

            var mockService = new Mock<IBoilerByBridgeService>();
            mockService
                .Setup(s => s.GetBoilerWiringInstallationStateAsync("IB3328595968", "1234", It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedState);

            var service = new BoilerByBridgeAppService(mockService.Object);

            var state = await service.GetBoilerWiringInstallationStateAsync("IB3328595968", "1234", CancellationToken.None);

            Assert.NotNull(state);
            Assert.Equal("INSTALLATION_COMPLETED", state?.State);
            Assert.True(state?.BridgeConnected);
            Assert.Equal("BR123456789", state?.DeviceWiredToBoiler?.SerialNo);
        }

        [Fact(DisplayName = "SetBoilerMaxOutputTemperatureAsync forwards command")]
        public async Task SetBoilerMaxOutputTemperatureAsync_ForwardsCommand()
        {
            var mockService = new Mock<IBoilerByBridgeService>();
            mockService
                .Setup(s => s.SetBoilerMaxOutputTemperatureAsync("IB3328595968", "1234", 55, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = new BoilerByBridgeAppService(mockService.Object);

            await service.SetBoilerMaxOutputTemperatureAsync("IB3328595968", "1234", 55, CancellationToken.None);

            mockService.Verify(s => s.SetBoilerMaxOutputTemperatureAsync("IB3328595968", "1234", 55, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
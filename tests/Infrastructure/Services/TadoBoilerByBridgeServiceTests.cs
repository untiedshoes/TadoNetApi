using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Infrastructure.Dtos.Requests;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Exceptions;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Services;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Services
{
    public class TadoBoilerByBridgeServiceTests
    {
        /// <summary>
        /// GetBoilerInfoAsync returns mapped boiler info.
        /// </summary>
        [Fact(DisplayName = "GetBoilerInfoAsync returns mapped boiler info")]
        public async Task GetBoilerInfoAsync_ReturnsMappedBoilerInfo()
        {
            var response = new TadoBoilerInfoResponse
            {
                BoilerPresent = true,
                BoilerId = 2699
            };

            var mockHttp = new Mock<IPublicTadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoBoilerInfoResponse>("homeByBridge/IB3328595968/boilerInfo?authKey=1234", It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var service = new TadoBoilerByBridgeService(mockHttp.Object);

            var boilerInfo = await service.GetBoilerInfoAsync("IB3328595968", "1234", CancellationToken.None);

            Assert.NotNull(boilerInfo);
            Assert.True(boilerInfo!.BoilerPresent);
            Assert.Equal(2699, boilerInfo.BoilerId);
        }

        /// <summary>
        /// GetBoilerMaxOutputTemperatureAsync returns mapped output temperature.
        /// </summary>
        [Fact(DisplayName = "GetBoilerMaxOutputTemperatureAsync returns mapped output temperature")]
        public async Task GetBoilerMaxOutputTemperatureAsync_ReturnsMappedOutputTemperature()
        {
            var response = new TadoBoilerMaxOutputTemperatureResponse
            {
                BoilerMaxOutputTemperatureInCelsius = 55
            };

            var mockHttp = new Mock<IPublicTadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoBoilerMaxOutputTemperatureResponse>("homeByBridge/IB3328595968/boilerMaxOutputTemperature?authKey=1234", It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var service = new TadoBoilerByBridgeService(mockHttp.Object);

            var temperature = await service.GetBoilerMaxOutputTemperatureAsync("IB3328595968", "1234", CancellationToken.None);

            Assert.NotNull(temperature);
            Assert.Equal(55, temperature!.BoilerMaxOutputTemperatureInCelsius);
        }

        /// <summary>
        /// SetBoilerMaxOutputTemperatureAsync sends bridge-scoped max output temperature command.
        /// </summary>
        [Fact(DisplayName = "SetBoilerMaxOutputTemperatureAsync sends bridge-scoped max output temperature command")]
        public async Task SetBoilerMaxOutputTemperatureAsync_SendsBridgeScopedCommand()
        {
            var mockHttp = new Mock<IPublicTadoHttpClient>();
            mockHttp
                .Setup(c => c.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<HttpMethod>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<HttpStatusCode>(),
                    It.IsAny<object?>()))
                .ReturnsAsync(true);

            var service = new TadoBoilerByBridgeService(mockHttp.Object);

            await service.SetBoilerMaxOutputTemperatureAsync("IB3328595968", "1234", 55, CancellationToken.None);

            mockHttp.Verify(c => c.SendAsync(
                    "homeByBridge/IB3328595968/boilerMaxOutputTemperature?authKey=1234",
                    HttpMethod.Put,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.NoContent,
                    It.Is<SetBoilerMaxOutputTemperatureRequest>(body => body.BoilerMaxOutputTemperatureInCelsius == 55)),
                Times.Once);
        }

        /// <summary>
        /// GetBoilerWiringInstallationStateAsync returns mapped boiler wiring state.
        /// </summary>
        [Fact(DisplayName = "GetBoilerWiringInstallationStateAsync returns mapped boiler wiring state")]
        public async Task GetBoilerWiringInstallationStateAsync_ReturnsMappedBoilerWiringState()
        {
            var response = new TadoBoilerWiringInstallationStateResponse
            {
                State = "INSTALLATION_COMPLETED",
                BridgeConnected = true,
                HotWaterZonePresent = true,
                DeviceWiredToBoiler = new TadoBoilerWiredDeviceResponse
                {
                    Type = "BR02",
                    SerialNo = "BR123456789",
                    ThermInterfaceType = "UBA_BUS",
                    Connected = true,
                    LastRequestTimestamp = new DateTime(2024, 12, 30, 11, 10, 24, DateTimeKind.Utc)
                },
                Boiler = new TadoBoilerWiringBoilerResponse
                {
                    OutputTemperature = new TadoBoilerOutputTemperatureResponse
                    {
                        Celsius = 56,
                        Timestamp = new DateTime(2024, 12, 30, 10, 58, 44, DateTimeKind.Utc)
                    }
                }
            };

            var mockHttp = new Mock<IPublicTadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoBoilerWiringInstallationStateResponse>("homeByBridge/IB3328595968/boilerWiringInstallationState?authKey=1234", It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var service = new TadoBoilerByBridgeService(mockHttp.Object);

            var state = await service.GetBoilerWiringInstallationStateAsync("IB3328595968", "1234", CancellationToken.None);

            Assert.NotNull(state);
            Assert.Equal("INSTALLATION_COMPLETED", state!.State);
            Assert.True(state.BridgeConnected);
            Assert.True(state.HotWaterZonePresent);
            Assert.Equal("BR123456789", state.DeviceWiredToBoiler?.SerialNo);
            Assert.Equal(56, state.Boiler?.OutputTemperature?.Celsius);
        }

        /// <summary>
        /// GetBoilerInfoAsync throws TadoApiException when API returns Unauthorized.
        /// </summary>
        [Fact(DisplayName = "GetBoilerInfoAsync throws TadoApiException when API returns Unauthorized")]
        public async Task GetBoilerInfoAsync_ShouldThrowTadoApiException_WhenApiReturnsUnauthorized()
        {
            var mockHttp = new Mock<IPublicTadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoBoilerInfoResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TadoApiException(HttpStatusCode.Unauthorized, "Unauthorized"));

            var service = new TadoBoilerByBridgeService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetBoilerInfoAsync("IB3328595968", "1234", CancellationToken.None));

            Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        }

        /// <summary>
        /// GetBoilerInfoAsync throws TadoApiException with ServiceUnavailable when network fails.
        /// </summary>
        [Fact(DisplayName = "GetBoilerInfoAsync throws TadoApiException with ServiceUnavailable when network fails")]
        public async Task GetBoilerInfoAsync_ShouldThrowTadoApiException_WhenNetworkFails()
        {
            var mockHttp = new Mock<IPublicTadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoBoilerInfoResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Network failed"));

            var service = new TadoBoilerByBridgeService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetBoilerInfoAsync("IB3328595968", "1234", CancellationToken.None));

            Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
        }

        /// <summary>
        /// GetBoilerMaxOutputTemperatureAsync returns null when API payload is null.
        /// </summary>
        [Fact(DisplayName = "GetBoilerMaxOutputTemperatureAsync returns null when API payload is null")]
        public async Task GetBoilerMaxOutputTemperatureAsync_ReturnsNull_WhenApiPayloadIsNull()
        {
            var mockHttp = new Mock<IPublicTadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoBoilerMaxOutputTemperatureResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TadoBoilerMaxOutputTemperatureResponse?)null);

            var service = new TadoBoilerByBridgeService(mockHttp.Object);

            var result = await service.GetBoilerMaxOutputTemperatureAsync("IB3328595968", "1234", CancellationToken.None);

            Assert.Null(result);
        }

        /// <summary>
        /// GetBoilerWiringInstallationStateAsync returns null when API payload is null.
        /// </summary>
        [Fact(DisplayName = "GetBoilerWiringInstallationStateAsync returns null when API payload is null")]
        public async Task GetBoilerWiringInstallationStateAsync_ReturnsNull_WhenApiPayloadIsNull()
        {
            var mockHttp = new Mock<IPublicTadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoBoilerWiringInstallationStateResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TadoBoilerWiringInstallationStateResponse?)null);

            var service = new TadoBoilerByBridgeService(mockHttp.Object);

            var result = await service.GetBoilerWiringInstallationStateAsync("IB3328595968", "1234", CancellationToken.None);

            Assert.Null(result);
        }

        /// <summary>
        /// GetBoilerMaxOutputTemperatureAsync throws ServiceUnavailable when network fails.
        /// </summary>
        [Fact(DisplayName = "GetBoilerMaxOutputTemperatureAsync throws ServiceUnavailable when network fails")]
        public async Task GetBoilerMaxOutputTemperatureAsync_ThrowsServiceUnavailable_WhenNetworkFails()
        {
            var mockHttp = new Mock<IPublicTadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoBoilerMaxOutputTemperatureResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Network failed"));

            var service = new TadoBoilerByBridgeService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetBoilerMaxOutputTemperatureAsync("IB3328595968", "1234", CancellationToken.None));

            Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
        }

        /// <summary>
        /// GetBoilerWiringInstallationStateAsync throws ServiceUnavailable when network fails.
        /// </summary>
        [Fact(DisplayName = "GetBoilerWiringInstallationStateAsync throws ServiceUnavailable when network fails")]
        public async Task GetBoilerWiringInstallationStateAsync_ThrowsServiceUnavailable_WhenNetworkFails()
        {
            var mockHttp = new Mock<IPublicTadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoBoilerWiringInstallationStateResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Network failed"));

            var service = new TadoBoilerByBridgeService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetBoilerWiringInstallationStateAsync("IB3328595968", "1234", CancellationToken.None));

            Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
        }

        /// <summary>
        /// Service methods validate bridge and auth key before building endpoint.
        /// </summary>
        [Fact(DisplayName = "BoilerByBridge service validates bridge and auth key")]
        public async Task BoilerByBridgeService_ValidatesBridgeAndAuthKey()
        {
            var mockHttp = new Mock<IPublicTadoHttpClient>();
            var service = new TadoBoilerByBridgeService(mockHttp.Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.GetBoilerInfoAsync(" ", "1234", CancellationToken.None));

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.GetBoilerInfoAsync("IB3328595968", " ", CancellationToken.None));
        }
    }
}
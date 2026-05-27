using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Enums;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Exceptions;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Services;
using TadoNetApi.Tests.Mocks;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Services
{
    /// <summary>
    /// Additional coverage tests for untested <see cref="TadoZoneService"/> read and command paths.
    /// </summary>
    public class TadoZoneServiceCoverageTests
    {
        /// <summary>
        /// GetZoneStateAsync returns mapped state when API returns a valid payload.
        /// </summary>
        [Fact(DisplayName = "GetZoneStateAsync returns mapped state when API returns a valid payload")]
        public async Task GetZoneStateAsync_ReturnsMappedState_WhenApiReturnsValidPayload()
        {
            var response = new TadoStateResponse
            {
                TadoMode = "HOME",
                OverlayType = "MANUAL",
                OpenWindowDetected = false,
                Setting = new TadoSettingResponse
                {
                    DeviceType = DeviceTypes.Heating,
                    Power = PowerStates.On,
                    Temperature = new TadoTemperatureResponse { Celsius = 21.5 }
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoZoneService(mockHttp.Object);

            var state = await service.GetZoneStateAsync(1, 2, CancellationToken.None);

            Assert.Equal("HOME", state.TadoMode);
            Assert.Equal("MANUAL", state.OverlayType);
            Assert.Equal(PowerStates.On, state.Setting?.Power);
            Assert.Equal(21.5, state.Setting?.Temperature?.Celsius);
        }

        /// <summary>
        /// GetZoneStateAsync throws ServiceUnavailable when network request fails.
        /// </summary>
        [Fact(DisplayName = "GetZoneStateAsync throws ServiceUnavailable when network request fails")]
        public async Task GetZoneStateAsync_ThrowsServiceUnavailable_WhenNetworkFails()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoStateResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("network down"));

            var service = new TadoZoneService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetZoneStateAsync(1, 2, CancellationToken.None));

            Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
            Assert.Contains("Failed to retrieve zone state", exception.Message);
        }

        /// <summary>
        /// GetZoneSummaryAsync returns null when API returns NotFound for missing overlay.
        /// </summary>
        [Fact(DisplayName = "GetZoneSummaryAsync returns null when API returns NotFound for missing overlay")]
        public async Task GetZoneSummaryAsync_ReturnsNull_WhenOverlayNotFound()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoZoneSummaryResponse>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new TadoApiException(HttpStatusCode.NotFound, "not found"));

            var service = new TadoZoneService(mockHttp.Object);

            var summary = await service.GetZoneSummaryAsync(1, 2, CancellationToken.None);

            Assert.Null(summary);
        }

        /// <summary>
        /// GetZoneCapabilitiesAsync maps array payloads into capability list.
        /// </summary>
        [Fact(DisplayName = "GetZoneCapabilitiesAsync maps array payloads into capability list")]
        public async Task GetZoneCapabilitiesAsync_MapsArrayPayload()
        {
            var payload = JsonSerializer.Deserialize<JsonElement>("[{\"type\":\"HEATING\"},{\"type\":\"HOT_WATER\"}]");
            var mockHttp = MockTadoHttpClient.CreateGet(payload);
            var service = new TadoZoneService(mockHttp.Object);

            var capabilities = await service.GetZoneCapabilitiesAsync(1, 2, CancellationToken.None);

            Assert.Equal(2, capabilities.Count);
            Assert.Equal("HEATING", capabilities[0].PurpleType);
            Assert.Equal("HOT_WATER", capabilities[1].PurpleType);
        }

        /// <summary>
        /// GetZoneCapabilitiesAsync maps object payload into a single capability result.
        /// </summary>
        [Fact(DisplayName = "GetZoneCapabilitiesAsync maps object payload into a single capability result")]
        public async Task GetZoneCapabilitiesAsync_MapsObjectPayload()
        {
            var payload = JsonSerializer.Deserialize<JsonElement>("{\"type\":\"HEATING\"}");
            var mockHttp = MockTadoHttpClient.CreateGet(payload);
            var service = new TadoZoneService(mockHttp.Object);

            var capabilities = await service.GetZoneCapabilitiesAsync(1, 2, CancellationToken.None);

            Assert.Single(capabilities);
            Assert.Equal("HEATING", capabilities[0].PurpleType);
        }

        /// <summary>
        /// GetZoneCapabilitiesAsync throws UnprocessableEntity when payload kind is unexpected.
        /// </summary>
        [Fact(DisplayName = "GetZoneCapabilitiesAsync throws UnprocessableEntity when payload kind is unexpected")]
        public async Task GetZoneCapabilitiesAsync_ThrowsUnprocessableEntity_WhenPayloadUnexpectedKind()
        {
            var payload = JsonSerializer.Deserialize<JsonElement>("\"invalid\"");
            var mockHttp = MockTadoHttpClient.CreateGet(payload);
            var service = new TadoZoneService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetZoneCapabilitiesAsync(1, 2, CancellationToken.None));

            Assert.Equal(HttpStatusCode.UnprocessableEntity, exception.StatusCode);
        }

        /// <summary>
        /// GetZoneTemperatureOffsetAsync returns mapped temperature when zone contains a valid short serial device.
        /// </summary>
        [Fact(DisplayName = "GetZoneTemperatureOffsetAsync returns mapped temperature when zone contains a valid short serial device")]
        public async Task GetZoneTemperatureOffsetAsync_ReturnsMappedTemperature_WhenZoneHasDeviceShortSerial()
        {
            var response = new TadoTemperatureResponse { Celsius = 1.5, Fahrenheit = 34.7 };
            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoZoneService(mockHttp.Object);

            var zone = new Zone
            {
                Devices =
                [
                    new Device { ShortSerialNo = "SU1234567890" }
                ]
            };

            var offset = await service.GetZoneTemperatureOffsetAsync(zone, CancellationToken.None);

            Assert.Equal(1.5, offset.Celsius);
            Assert.Equal(34.7, offset.Fahrenheit);
        }

        /// <summary>
        /// GetZoneTemperatureOffsetAsync throws ArgumentException when zone has no valid short serial devices.
        /// </summary>
        [Fact(DisplayName = "GetZoneTemperatureOffsetAsync throws ArgumentException when zone has no valid short serial devices")]
        public async Task GetZoneTemperatureOffsetAsync_ThrowsArgumentException_WhenZoneHasNoValidShortSerial()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoZoneService(mockHttp.Object);

            var zone = new Zone
            {
                Devices =
                [
                    new Device { ShortSerialNo = "" }
                ]
            };

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.GetZoneTemperatureOffsetAsync(zone, CancellationToken.None));
        }

        /// <summary>
        /// SetEarlyStartAsync sends the expected command route and body.
        /// </summary>
        [Fact(DisplayName = "SetEarlyStartAsync sends the expected command route and body")]
        public async Task SetEarlyStartAsync_SendsExpectedCommandRouteAndBody()
        {
            object? capturedBody = null;
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<HttpMethod>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<HttpStatusCode>(),
                    It.IsAny<object?>()))
                .Callback<string, HttpMethod, CancellationToken, HttpStatusCode, object?>((_, _, _, _, body) =>
                {
                    capturedBody = body;
                })
                .ReturnsAsync(true);

            var service = new TadoZoneService(mockHttp.Object);

            var result = await service.SetEarlyStartAsync(1, 2, true, CancellationToken.None);

            Assert.True(result);
            var bodyJson = JsonSerializer.Serialize(capturedBody);
            Assert.Contains("\"enabled\":true", bodyJson);

            mockHttp.Verify(c => c.SendAsync(
                "homes/1/zones/2/earlyStart",
                HttpMethod.Put,
                It.IsAny<CancellationToken>(),
                HttpStatusCode.OK,
                It.IsAny<object?>()), Times.Once);
        }

        /// <summary>
        /// GetEarlyStartAsync returns mapped early start settings when API payload is present.
        /// </summary>
        [Fact(DisplayName = "GetEarlyStartAsync returns mapped early start settings when payload is present")]
        public async Task GetEarlyStartAsync_ReturnsMappedEarlyStart_WhenPayloadPresent()
        {
            var mockHttp = MockTadoHttpClient.CreateGet(new TadoEarlyStartResponse { Enabled = true });
            var service = new TadoZoneService(mockHttp.Object);

            var result = await service.GetEarlyStartAsync(1, 2, CancellationToken.None);

            Assert.True(result.Enabled);
        }

        /// <summary>
        /// GetEarlyStartAsync throws NotFound when API payload is null.
        /// </summary>
        [Fact(DisplayName = "GetEarlyStartAsync throws NotFound when payload is null")]
        public async Task GetEarlyStartAsync_ThrowsNotFound_WhenPayloadIsNull()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoEarlyStartResponse>("homes/1/zones/2/earlyStart", It.IsAny<CancellationToken>()))
                .ReturnsAsync((TadoEarlyStartResponse?)null);

            var service = new TadoZoneService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetEarlyStartAsync(1, 2, CancellationToken.None));

            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }

        /// <summary>
        /// GetDefaultZoneOverlayAsync throws ServiceUnavailable when network request fails.
        /// </summary>
        [Fact(DisplayName = "GetDefaultZoneOverlayAsync throws ServiceUnavailable on network failure")]
        public async Task GetDefaultZoneOverlayAsync_ThrowsServiceUnavailable_OnNetworkFailure()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoDefaultZoneOverlayResponse>("homes/1/zones/2/defaultOverlay", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("network down"));

            var service = new TadoZoneService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetDefaultZoneOverlayAsync(1, 2, CancellationToken.None));

            Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
        }

        /// <summary>
        /// GetAwayConfigurationAsync throws NotFound when API payload is null.
        /// </summary>
        [Fact(DisplayName = "GetAwayConfigurationAsync throws NotFound when payload is null")]
        public async Task GetAwayConfigurationAsync_ThrowsNotFound_WhenPayloadIsNull()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoAwayConfigurationResponse>("homes/1/zones/2/schedule/awayConfiguration", It.IsAny<CancellationToken>()))
                .ReturnsAsync((TadoAwayConfigurationResponse?)null);

            var service = new TadoZoneService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetAwayConfigurationAsync(1, 2, CancellationToken.None));

            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}

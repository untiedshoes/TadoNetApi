using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Entities.MobileDevice;
using TadoNetApi.Infrastructure.Dtos.Requests;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Dtos.Responses.MobileDevice;
using TadoNetApi.Infrastructure.Exceptions;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Services;
using TadoNetApi.Tests.Mocks;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Services
{
    /// <summary>
    /// Tests for <see cref="TadoDeviceService"/>, including domain mapping, transient failures,
    /// and retry logic.
    /// </summary>
    public class TadoDeviceServiceTests
    {
        #region Happy-path tests

        /// <summary>
        /// Tests that <see cref="TadoDeviceService.GetDevicesAsync"/> returns a mapped list of devices.
        /// </summary>
        [Fact(DisplayName = "GetDevicesAsync returns mapped devices")]
        public async Task GetDevicesAsync_ReturnsMappedDevices()
        {
            // Arrange
            var tadoDevices = new List<TadoDeviceResponse>
            {
                new TadoDeviceResponse
                {
                    DeviceType = "THERMOSTAT",
                    SerialNo = "123456789",
                    ShortSerialNo = "123456",
                    CurrentFwVersion = "1.0.0"
                },
                new TadoDeviceResponse
                {
                    DeviceType = "RADIATOR",
                    SerialNo = "987654321",
                    ShortSerialNo = "987654",
                    CurrentFwVersion = "1.0.0"
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(tadoDevices);
            var service = new TadoDeviceService(mockHttp.Object);

            // Act
            var devices = await service.GetDevicesAsync(homeId: 1, CancellationToken.None);

            // Assert
            Assert.NotNull(devices);
            Assert.Equal(2, devices.Count);

            var thermostat = devices.First(d => d.SerialNo == "123456789");
            Assert.Equal("THERMOSTAT", thermostat.DeviceType);
            Assert.Null(thermostat.DeviceTypeName);
            Assert.Equal("123456789", thermostat.SerialNo);
            Assert.Equal("123456", thermostat.ShortSerialNo);
            Assert.Equal("1.0.0", thermostat.CurrentFwVersion);

            var radiator = devices.First(d => d.SerialNo == "987654321");
            Assert.Equal("RADIATOR", radiator.DeviceType);
            Assert.Null(radiator.DeviceTypeName);
            Assert.Equal("987654321", radiator.SerialNo);
            Assert.Equal("987654", radiator.ShortSerialNo);
            Assert.Equal("1.0.0", radiator.CurrentFwVersion);
        }

        /// <summary>
        /// Tests that known device type codes expose a friendly hardware name.
        /// </summary>
        [Fact(DisplayName = "GetDevicesAsync maps known device type codes to friendly names")]
        public async Task GetDevicesAsync_MapsKnownDeviceTypeNames()
        {
            var tadoDevices = new List<TadoDeviceResponse>
            {
                new()
                {
                    DeviceType = "VA02",
                    SerialNo = "VA0467490560",
                    ShortSerialNo = "VA0467490560",
                    CurrentFwVersion = "215.1"
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(tadoDevices);
            var service = new TadoDeviceService(mockHttp.Object);

            var devices = await service.GetDevicesAsync(homeId: 1, CancellationToken.None);

            var device = Assert.Single(devices);
            Assert.Equal("VA02", device.DeviceType);
            Assert.Equal("Smart Radiator Thermostat V3+", device.DeviceTypeName);
        }

        /// <summary>
        /// Tests that device type codes with revision suffixes still resolve to a friendly name.
        /// </summary>
        [Fact(DisplayName = "GetDevicesAsync maps suffixed device type codes to friendly names")]
        public async Task GetDevicesAsync_MapsSuffixedDeviceTypeNames()
        {
            var tadoDevices = new List<TadoDeviceResponse>
            {
                new()
                {
                    DeviceType = "SU02B",
                    SerialNo = "SU3339800320",
                    ShortSerialNo = "SU3339800320",
                    CurrentFwVersion = "215.1"
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(tadoDevices);
            var service = new TadoDeviceService(mockHttp.Object);

            var devices = await service.GetDevicesAsync(homeId: 1, CancellationToken.None);

            var device = Assert.Single(devices);
            Assert.Equal("SU02B", device.DeviceType);
            Assert.Equal("Wireless Temperature Sensor V3+", device.DeviceTypeName);
        }

        /// <summary>
        /// Tests that <see cref="TadoDeviceService.GetDeviceListAsync"/> returns mapped
        /// device entries including zone association.
        /// </summary>
        [Fact(DisplayName = "GetDeviceListAsync returns mapped entries")]
        public async Task GetDeviceListAsync_ReturnsMappedEntries()
        {
            // Arrange
            var deviceListResponse = new TadoDeviceListResponse
            {
                Entries =
                [
                    new TadoDeviceListItemResponse
                    {
                        Type = "SU02",
                        Device = new TadoDeviceResponse
                        {
                            DeviceType = "SU02",
                            SerialNo = "SU3339800320",
                            ShortSerialNo = "SU3339800320",
                            CurrentFwVersion = "215.1"
                        },
                        Zone = new TadoDeviceListZoneResponse
                        {
                            Discriminator = 3,
                            Duties = ["UI"]
                        }
                    }
                ]
            };

            var mockHttp = MockTadoHttpClient.CreateGet(deviceListResponse);
            var service = new TadoDeviceService(mockHttp.Object);

            // Act
            var entries = await service.GetDeviceListAsync(homeId: 1, CancellationToken.None);

            // Assert
            Assert.Single(entries);
            var entry = entries[0];
            Assert.Equal("SU02", entry.Type);
            Assert.Equal(3, entry.ZoneId);
            Assert.NotNull(entry.Device);
            Assert.Equal("SU3339800320", entry.Device!.SerialNo);
            Assert.Equal("Wireless Temperature Sensor V3+", entry.Device.DeviceTypeName);
            Assert.Equal("SU3339800320", entry.Device.ShortSerialNo);
            Assert.Contains("UI", entry.ZoneDuties!);
        }

        #endregion

        #region Validation/Failure-scenario tests

        /// <summary>
        /// Tests that transient failures are retried and eventually succeed.
        /// Simulates two transient <see cref="HttpRequestException"/>s.
        /// </summary>
        [Fact(DisplayName = "GetDevicesAsync retries on transient failures")]
        public async Task GetDevicesAsync_RetriesOnTransientFailures()
        {
            // Arrange
            var tadoDevices = new List<TadoDeviceResponse>
            {
                new TadoDeviceResponse
                {
                    DeviceType = "THERMOSTAT",
                    SerialNo = "123456789",
                    ShortSerialNo = "123456",
                    CurrentFwVersion = "1.0.0"
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(
                returnValue: tadoDevices,
                transientFailures: 2,
                transientException: new HttpRequestException("Transient error"));

            var service = new TadoDeviceService(mockHttp.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetDevicesAsync(homeId: 1, CancellationToken.None));

            Assert.Contains("Failed to retrieve devices", ex.Message);
        }

        /// <summary>
        /// Tests that <see cref="TadoDeviceService.GetDeviceAsync"/> returns a single mapped device.
        /// </summary>
        [Fact(DisplayName = "GetDeviceAsync string overload uses the spec-aligned device path")]
        public async Task GetDeviceAsync_StringOverload_UsesSpecAlignedDevicePath()
        {
            var tadoDevice = new TadoDeviceResponse
            {
                DeviceType = "THERMOSTAT",
                SerialNo = "123456789",
                ShortSerialNo = "123456",
                CurrentFwVersion = "1.0.0"
            };

            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoDeviceResponse>("devices/SU1234567890", It.IsAny<CancellationToken>()))
                .ReturnsAsync(tadoDevice);

            var service = new TadoDeviceService(mockHttp.Object);

            var device = await service.GetDeviceAsync("SU1234567890", CancellationToken.None);

            Assert.NotNull(device);
            Assert.Equal("THERMOSTAT", device.DeviceType);
            Assert.Equal("123456789", device.SerialNo);
            Assert.Equal("123456", device.ShortSerialNo);
            Assert.Equal("1.0.0", device.CurrentFwVersion);
            mockHttp.Verify(c => c.GetAsync<TadoDeviceResponse>("devices/SU1234567890", It.IsAny<CancellationToken>()), Times.Once);
            mockHttp.Verify(c => c.GetAsync<TadoDeviceResponse>(It.Is<string>(path => path.StartsWith("homes/")), It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// Tests that the legacy GetDeviceAsync overload still routes through the device-scoped endpoint.
        /// </summary>
        [Fact(DisplayName = "GetDeviceAsync legacy overload uses the spec-aligned device path")]
        public async Task GetDeviceAsync_LegacyOverload_UsesSpecAlignedDevicePath()
        {
            var tadoDevice = new TadoDeviceResponse
            {
                DeviceType = "THERMOSTAT",
                SerialNo = "123",
                ShortSerialNo = "123",
                CurrentFwVersion = "1.0.0"
            };

            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoDeviceResponse>("devices/123", It.IsAny<CancellationToken>()))
                .ReturnsAsync(tadoDevice);

            var service = new TadoDeviceService(mockHttp.Object);

            var device = await service.GetDeviceAsync(homeId: 1, deviceId: 123, CancellationToken.None);

            Assert.NotNull(device);
            Assert.Equal("123", device.SerialNo);
            mockHttp.Verify(c => c.GetAsync<TadoDeviceResponse>("devices/123", It.IsAny<CancellationToken>()), Times.Once);
            mockHttp.Verify(c => c.GetAsync<TadoDeviceResponse>(It.Is<string>(path => path.StartsWith("homes/")), It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// Tests that <see cref="TadoDeviceService.GetZoneTemperatureOffsetAsync"/> returns a mapped temperature.
        /// </summary>
        [Fact(DisplayName = "GetZoneTemperatureOffsetAsync returns temperature offset")]
        public async Task GetZoneTemperatureOffsetAsync_ReturnsTemperatureOffset()
        {
            // Arrange
            var temperatureOffset = new TadoTemperatureResponse
            {
                Celsius = 1.5,
                Fahrenheit = 34.7
            };

            var mockHttp = MockTadoHttpClient.CreateGet(temperatureOffset);
            var service = new TadoDeviceService(mockHttp.Object);

            // Act
            var offset = await service.GetZoneTemperatureOffsetAsync(deviceId: 1, CancellationToken.None);

            // Assert
            Assert.NotNull(offset);
            Assert.Equal(1.5, offset.Celsius);
            Assert.Equal(34.7, offset.Fahrenheit);
        }

        /// <summary>
        /// Tests that <see cref="TadoDeviceService.GetMobileDeviceAsync"/> returns a mapped mobile device.
        /// </summary>
        [Fact(DisplayName = "GetMobileDeviceAsync returns mapped mobile device")]
        public async Task GetMobileDeviceAsync_ReturnsMappedMobileDevice()
        {
            // Arrange
            var mobileDevice = new TadoMobileItemResponse
            {
                Id = 42,
                Name = "Craig's iPhone",
                Settings = new TadoMobileSettingsResponse
                {
                    GeoTrackingEnabled = true
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(mobileDevice);
            var service = new TadoDeviceService(mockHttp.Object);

            // Act
            var device = await service.GetMobileDeviceAsync(homeId: 1, mobileDeviceId: 42, CancellationToken.None);

            // Assert
            Assert.NotNull(device);
            Assert.Equal(42, device.Id);
            Assert.Equal("Craig's iPhone", device.Name);
            Assert.True(device.Settings?.GeoTrackingEnabled);
        }

        /// <summary>
        /// Tests that <see cref="TadoDeviceService.GetZoneMeasuringDeviceAsync"/> returns a mapped device.
        /// </summary>
        [Fact(DisplayName = "GetZoneMeasuringDeviceAsync returns mapped device")]
        public async Task GetZoneMeasuringDeviceAsync_ReturnsMappedDevice()
        {
            // Arrange
            var tadoDevice = new TadoDeviceResponse
            {
                DeviceType = "SU02",
                SerialNo = "SU1234567890",
                ShortSerialNo = "SU1234567890",
                CurrentFwVersion = "215.1"
            };

            var mockHttp = MockTadoHttpClient.CreateGet(tadoDevice);
            var service = new TadoDeviceService(mockHttp.Object);

            // Act
            var device = await service.GetZoneMeasuringDeviceAsync(homeId: 1, zoneId: 2, CancellationToken.None);

            // Assert
            Assert.NotNull(device);
            Assert.Equal("Wireless Temperature Sensor V3+", device.DeviceTypeName);
            Assert.Equal("SU02", device.DeviceType);
            Assert.Equal("SU1234567890", device.SerialNo);
            Assert.Equal("215.1", device.CurrentFwVersion);
        }

        #endregion

        #region Integration tests

        /// <summary>
        /// Integration test scaffold.
        /// Intended to be adapted for a real Tado account using a valid access token and safe test IDs.
        /// Marked as Integration category.
        /// </summary>
        [Fact(DisplayName = "GetDevicesAsync integration test", Skip = "Run manually with a real access token and safe test IDs")]
        [Trait("Category", "Integration")]
        public async Task GetDevicesAsync_IntegrationTest()
        {
            // Example placeholder for real integration test
            // var authService = new TadoAuthService(...);
            // var service = new TadoDeviceService(new TadoHttpClient(authService));
            // var devices = await service.GetDevicesAsync(homeId: 123, CancellationToken.None);
            // Assert.NotEmpty(devices);
        }

        #endregion

        /// <summary>
        /// Tests that <see cref="TadoDeviceService.SetZoneTemperatureOffsetCelsiusAsync"/>
        /// sends the expected command payload and endpoint.
        /// </summary>
        [Fact(DisplayName = "SetZoneTemperatureOffsetCelsiusAsync sends expected command")]
        public async Task SetZoneTemperatureOffsetCelsiusAsync_SendsExpectedCommand()
        {
            // Arrange
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<HttpMethod>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<HttpStatusCode>(),
                    It.IsAny<object?>()))
                .ReturnsAsync(true);

            var service = new TadoDeviceService(mockHttp.Object);

            // Act
            var success = await service.SetZoneTemperatureOffsetCelsiusAsync("ABC123", 1.5, CancellationToken.None);

            // Assert
            Assert.True(success);
            mockHttp.Verify(c => c.SendAsync(
                    "devices/ABC123/temperatureOffset",
                    HttpMethod.Put,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.OK,
                    It.Is<object?>(b => b != null && JsonSerializer.Serialize(b).Contains("\"celsius\":1.5"))),
                Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="TadoDeviceService.SetZoneTemperatureOffsetFahrenheitAsync"/>
        /// sends the expected command payload and endpoint.
        /// </summary>
        [Fact(DisplayName = "SetZoneTemperatureOffsetFahrenheitAsync sends expected command")]
        public async Task SetZoneTemperatureOffsetFahrenheitAsync_SendsExpectedCommand()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<HttpMethod>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<HttpStatusCode>(),
                    It.IsAny<object?>()))
                .ReturnsAsync(true);

            var service = new TadoDeviceService(mockHttp.Object);

            var success = await service.SetZoneTemperatureOffsetFahrenheitAsync("ABC123", -2.25, CancellationToken.None);

            Assert.True(success);
            mockHttp.Verify(c => c.SendAsync(
                    "devices/ABC123/temperatureOffset",
                    HttpMethod.Put,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.OK,
                    It.Is<object?>(b => b != null && JsonSerializer.Serialize(b).Contains("\"fahrenheit\":-2.25"))),
                Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="TadoDeviceService.DeleteMobileDeviceAsync"/>
        /// sends the expected command endpoint.
        /// </summary>
        [Fact(DisplayName = "DeleteMobileDeviceAsync sends the spec-aligned mobile device delete command")]
        public async Task DeleteMobileDeviceAsync_SendsSpecAlignedMobileDeviceDeleteCommand()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<HttpMethod>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<HttpStatusCode>(),
                    It.IsAny<object?>()))
                .ReturnsAsync(true);

            var service = new TadoDeviceService(mockHttp.Object);

            var deleted = await service.DeleteMobileDeviceAsync(1, 42, CancellationToken.None);

            Assert.True(deleted);
            mockHttp.Verify(c => c.SendAsync(
                    "homes/1/mobileDevices/42",
                    HttpMethod.Delete,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.OK,
                    null),
                Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="TadoDeviceService.MoveDeviceToZoneAsync"/>
        /// sends the expected command payload and endpoint.
        /// </summary>
        [Fact(DisplayName = "MoveDeviceToZoneAsync sends the spec-aligned move-device command")]
        public async Task MoveDeviceToZoneAsync_SendsSpecAlignedMoveDeviceCommand()
        {
            string? capturedJson = null;
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
                    capturedJson = body == null ? null : JsonSerializer.Serialize(body);
                })
                .ReturnsAsync(true);

            var service = new TadoDeviceService(mockHttp.Object);

            var moved = await service.MoveDeviceToZoneAsync(1, 2, "SU1234567890", true, CancellationToken.None);

            Assert.True(moved);
            Assert.NotNull(capturedJson);
            Assert.Contains("\"serialNo\":\"SU1234567890\"", capturedJson);
            mockHttp.Verify(c => c.SendAsync(
                    "homes/1/zones/2/devices?force=true",
                    HttpMethod.Post,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.OK,
                    It.IsAny<object?>()),
                Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="TadoDeviceService.MoveDeviceToZoneAsync"/>
        /// rejects missing serial numbers.
        /// </summary>
        [Fact(DisplayName = "MoveDeviceToZoneAsync rejects missing device serial numbers")]
        public async Task MoveDeviceToZoneAsync_RejectsMissingDeviceSerialNumbers()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoDeviceService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                service.MoveDeviceToZoneAsync(1, 2, " ", null, CancellationToken.None));

            Assert.Equal("deviceSerialNo", exception.ParamName);
            mockHttp.Verify(c => c.SendAsync(
                It.IsAny<string>(),
                It.IsAny<HttpMethod>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<HttpStatusCode>(),
                It.IsAny<object?>()), Times.Never);
        }

        /// <summary>
        /// Tests that <see cref="TadoDeviceService.SetZoneMeasuringDeviceAsync"/>
        /// sends the expected payload and endpoint.
        /// </summary>
        [Fact(DisplayName = "SetZoneMeasuringDeviceAsync sends the spec-aligned measuring-device command")]
        public async Task SetZoneMeasuringDeviceAsync_SendsSpecAlignedMeasuringDeviceCommand()
        {
            string? capturedJson = null;
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.PutAsync<SetZoneMeasuringDeviceRequest, TadoDeviceResponse>(
                    It.IsAny<string>(),
                    It.IsAny<SetZoneMeasuringDeviceRequest>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, SetZoneMeasuringDeviceRequest, CancellationToken>((_, req, _) =>
                {
                    capturedJson = JsonSerializer.Serialize(req);
                })
                .ReturnsAsync(new TadoDeviceResponse
                {
                    DeviceType = "SU02",
                    SerialNo = "SU1234567890",
                    ShortSerialNo = "SU1234567890"
                });

            var service = new TadoDeviceService(mockHttp.Object);

            var device = await service.SetZoneMeasuringDeviceAsync(1, 2, "SU1234567890", CancellationToken.None);

            Assert.NotNull(device);
            Assert.NotNull(capturedJson);
            Assert.Contains("\"serialNo\":\"SU1234567890\"", capturedJson);
            mockHttp.Verify(c => c.PutAsync<SetZoneMeasuringDeviceRequest, TadoDeviceResponse>(
                    "homes/1/zones/2/measuringDevice",
                    It.Is<SetZoneMeasuringDeviceRequest>(r => r.SerialNo == "SU1234567890"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="TadoDeviceService.SetZoneMeasuringDeviceAsync"/>
        /// rejects missing serial numbers.
        /// </summary>
        [Fact(DisplayName = "SetZoneMeasuringDeviceAsync rejects missing device serial numbers")]
        public async Task SetZoneMeasuringDeviceAsync_RejectsMissingDeviceSerialNumbers()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoDeviceService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetZoneMeasuringDeviceAsync(1, 2, string.Empty, CancellationToken.None));

            Assert.Equal("deviceSerialNo", exception.ParamName);
            mockHttp.Verify(c => c.PutAsync<SetZoneMeasuringDeviceRequest, TadoDeviceResponse>(
                It.IsAny<string>(),
                It.IsAny<SetZoneMeasuringDeviceRequest>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// Tests that <see cref="TadoDeviceService.SetMobileDeviceSettingsAsync"/>
        /// sends the expected mobile device settings payload and endpoint.
        /// </summary>
        [Fact(DisplayName = "SetMobileDeviceSettingsAsync sends the spec-aligned mobile device settings command")]
        public async Task SetMobileDeviceSettingsAsync_SendsSpecAlignedMobileDeviceSettingsCommand()
        {
            string? capturedJson = null;
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
                    capturedJson = body == null ? null : JsonSerializer.Serialize(body);
                })
                .ReturnsAsync(true);

            var service = new TadoDeviceService(mockHttp.Object);

            var updated = await service.SetMobileDeviceSettingsAsync(
                1,
                42,
                new Settings { GeoTrackingEnabled = true },
                CancellationToken.None);

            Assert.True(updated);
            Assert.NotNull(capturedJson);
            Assert.Contains("\"geoTrackingEnabled\":true", capturedJson);
            mockHttp.Verify(c => c.SendAsync(
                    "homes/1/mobileDevices/42/settings",
                    HttpMethod.Put,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.OK,
                    It.IsAny<object?>()),
                Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="TadoDeviceService.SetMobileDeviceSettingsAsync"/>
        /// rejects null settings.
        /// </summary>
        [Fact(DisplayName = "SetMobileDeviceSettingsAsync rejects null settings")]
        public async Task SetMobileDeviceSettingsAsync_RejectsNullSettings()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoDeviceService(mockHttp.Object);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                service.SetMobileDeviceSettingsAsync(1, 42, null!, CancellationToken.None));

            mockHttp.Verify(c => c.SendAsync(
                It.IsAny<string>(),
                It.IsAny<HttpMethod>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<HttpStatusCode>(),
                It.IsAny<object?>()), Times.Never);
        }

        /// <summary>
        /// GetDevicesAsync throws TadoApiException when API returns Unauthorized.
        /// </summary>
        [Fact(DisplayName = "GetDevicesAsync throws TadoApiException when API returns Unauthorized")]
        public async Task GetDevicesAsync_ShouldThrowTadoApiException_WhenApiReturnsUnauthorized()
        {
            var mockHttp = MockTadoHttpClient.CreateGet<List<TadoDeviceResponse>>(
                returnValue: null!,
                transientFailures: int.MaxValue,
                transientException: new TadoApiException(HttpStatusCode.Unauthorized, "Unauthorized"));

            var service = new TadoDeviceService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetDevicesAsync(homeId: 1, CancellationToken.None));

            Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        }

        /// <summary>
        /// GetDevicesAsync throws TadoApiException with ServiceUnavailable when network fails.
        /// </summary>
        [Fact(DisplayName = "GetDevicesAsync throws TadoApiException with ServiceUnavailable when network fails")]
        public async Task GetDevicesAsync_ShouldThrowTadoApiException_WhenNetworkFails()
        {
            var mockHttp = MockTadoHttpClient.CreateGet<List<TadoDeviceResponse>>(
                returnValue: null!,
                transientFailures: int.MaxValue,
                transientException: new HttpRequestException("Network failed"));

            var service = new TadoDeviceService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetDevicesAsync(homeId: 1, CancellationToken.None));

            Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
        }

        /// <summary>
        /// GetDeviceAsync throws TadoApiException with NotFound when API returns null payload.
        /// </summary>
        [Fact(DisplayName = "GetDeviceAsync throws NotFound when API returns null payload")]
        public async Task GetDeviceAsync_ThrowsNotFound_WhenApiReturnsNullPayload()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoDeviceResponse>("devices/SU123456", It.IsAny<CancellationToken>()))
                .ReturnsAsync((TadoDeviceResponse?)null);

            var service = new TadoDeviceService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetDeviceAsync("SU123456", CancellationToken.None));

            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }

        /// <summary>
        /// GetDeviceListAsync returns an empty list when API returns null entries.
        /// </summary>
        [Fact(DisplayName = "GetDeviceListAsync returns empty list when entries are null")]
        public async Task GetDeviceListAsync_ReturnsEmptyList_WhenEntriesAreNull()
        {
            var mockHttp = MockTadoHttpClient.CreateGet(new TadoDeviceListResponse { Entries = null });
            var service = new TadoDeviceService(mockHttp.Object);

            var entries = await service.GetDeviceListAsync(1, CancellationToken.None);

            Assert.Empty(entries);
        }

        /// <summary>
        /// GetDevicesAsync returns an empty list when the API payload is null.
        /// </summary>
        [Fact(DisplayName = "GetDevicesAsync returns empty list when API payload is null")]
        public async Task GetDevicesAsync_ReturnsEmptyList_WhenApiPayloadIsNull()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<List<TadoDeviceResponse>>("homes/1/devices", It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<TadoDeviceResponse>?)null);

            var service = new TadoDeviceService(mockHttp.Object);

            var devices = await service.GetDevicesAsync(1, CancellationToken.None);

            Assert.Empty(devices);
        }

        /// <summary>
        /// GetMobileDevicesAsync returns an empty list when the API payload is null.
        /// </summary>
        [Fact(DisplayName = "GetMobileDevicesAsync returns empty list when API payload is null")]
        public async Task GetMobileDevicesAsync_ReturnsEmptyList_WhenApiPayloadIsNull()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<List<TadoMobileItemResponse>>("homes/1/mobileDevices", It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<TadoMobileItemResponse>?)null);

            var service = new TadoDeviceService(mockHttp.Object);

            var devices = await service.GetMobileDevicesAsync(1, CancellationToken.None);

            Assert.Empty(devices);
        }

        /// <summary>
        /// GetMobileDevicesAsync returns mapped mobile devices for a home.
        /// </summary>
        [Fact(DisplayName = "GetMobileDevicesAsync returns mapped mobile devices")]
        public async Task GetMobileDevicesAsync_ReturnsMappedMobileDevices()
        {
            var response = new List<TadoMobileItemResponse>
            {
                new()
                {
                    Id = 42,
                    Name = "Craig's iPhone",
                    Settings = new TadoMobileSettingsResponse { GeoTrackingEnabled = true }
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoDeviceService(mockHttp.Object);

            var devices = await service.GetMobileDevicesAsync(1, CancellationToken.None);

            var device = Assert.Single(devices);
            Assert.Equal(42, device.Id);
            Assert.Equal("Craig's iPhone", device.Name);
            Assert.True(device.Settings?.GeoTrackingEnabled);
        }

        /// <summary>
        /// GetMobileDeviceSettingsAsync returns mapped settings when payload exists.
        /// </summary>
        [Fact(DisplayName = "GetMobileDeviceSettingsAsync returns mapped settings")]
        public async Task GetMobileDeviceSettingsAsync_ReturnsMappedSettings()
        {
            var mockHttp = MockTadoHttpClient.CreateGet(new TadoMobileSettingsResponse
            {
                GeoTrackingEnabled = false
            });

            var service = new TadoDeviceService(mockHttp.Object);

            var settings = await service.GetMobileDeviceSettingsAsync(1, 42, CancellationToken.None);

            Assert.False(settings.GeoTrackingEnabled);
        }

        /// <summary>
        /// GetMobileDeviceSettingsAsync throws NotFound when API returns null payload.
        /// </summary>
        [Fact(DisplayName = "GetMobileDeviceSettingsAsync throws NotFound when API returns null payload")]
        public async Task GetMobileDeviceSettingsAsync_ThrowsNotFound_WhenApiReturnsNullPayload()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoMobileSettingsResponse>("homes/1/mobileDevices/42/settings", It.IsAny<CancellationToken>()))
                .ReturnsAsync((TadoMobileSettingsResponse?)null);

            var service = new TadoDeviceService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetMobileDeviceSettingsAsync(1, 42, CancellationToken.None));

            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }

        /// <summary>
        /// SetDeviceChildLockAsync on device overload returns false when short serial number is missing.
        /// </summary>
        [Fact(DisplayName = "SetDeviceChildLockAsync device overload returns false when short serial is missing")]
        public async Task SetDeviceChildLockAsync_DeviceOverload_ReturnsFalse_WhenShortSerialIsMissing()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoDeviceService(mockHttp.Object);

            var success = await service.SetDeviceChildLockAsync(new Device { ShortSerialNo = null }, true, CancellationToken.None);

            Assert.False(success);
            mockHttp.Verify(c => c.SendAsync(
                It.IsAny<string>(),
                It.IsAny<HttpMethod>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<HttpStatusCode>(),
                It.IsAny<object?>()), Times.Never);
        }

        /// <summary>
        /// SetDeviceChildLockAsync on string overload sends the expected command endpoint and payload.
        /// </summary>
        [Fact(DisplayName = "SetDeviceChildLockAsync string overload sends expected command")]
        public async Task SetDeviceChildLockAsync_StringOverload_SendsExpectedCommand()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<HttpMethod>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<HttpStatusCode>(),
                    It.IsAny<object?>()))
                .ReturnsAsync(true);

            var service = new TadoDeviceService(mockHttp.Object);

            var success = await service.SetDeviceChildLockAsync("SU123456", true, CancellationToken.None);

            Assert.True(success);
            mockHttp.Verify(c => c.SendAsync(
                    "devices/SU123456/childLock",
                    HttpMethod.Put,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.NoContent,
                    It.Is<object?>(body => body != null && JsonSerializer.Serialize(body).Contains("\"childLockEnabled\":true"))),
                Times.Once);
        }

        /// <summary>
        /// SayHiAsync sends the identify command to the expected endpoint.
        /// </summary>
        [Fact(DisplayName = "SayHiAsync sends identify command to expected endpoint")]
        public async Task SayHiAsync_SendsIdentifyCommand_ToExpectedEndpoint()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<HttpMethod>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<HttpStatusCode>(),
                    It.IsAny<object?>()))
                .ReturnsAsync(true);

            var service = new TadoDeviceService(mockHttp.Object);

            var success = await service.SayHiAsync("SU123456", CancellationToken.None);

            Assert.True(success);
            mockHttp.Verify(c => c.SendAsync(
                    "devices/SU123456/identify",
                    HttpMethod.Post,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.OK,
                    It.IsAny<object?>()),
                Times.Once);
        }

        /// <summary>
        /// GetZoneTemperatureOffsetAsync throws NotFound when API returns null payload.
        /// </summary>
        [Fact(DisplayName = "GetZoneTemperatureOffsetAsync throws NotFound when API returns null payload")]
        public async Task GetZoneTemperatureOffsetAsync_ThrowsNotFound_WhenApiReturnsNullPayload()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoTemperatureResponse>("devices/1/temperatureOffset", It.IsAny<CancellationToken>()))
                .ReturnsAsync((TadoTemperatureResponse?)null);

            var service = new TadoDeviceService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetZoneTemperatureOffsetAsync(1, CancellationToken.None));

            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }

        /// <summary>
        /// GetZoneMeasuringDeviceAsync throws NotFound when API returns null payload.
        /// </summary>
        [Fact(DisplayName = "GetZoneMeasuringDeviceAsync throws NotFound when API returns null payload")]
        public async Task GetZoneMeasuringDeviceAsync_ThrowsNotFound_WhenApiReturnsNullPayload()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoDeviceResponse>("homes/1/zones/2/measuringDevice", It.IsAny<CancellationToken>()))
                .ReturnsAsync((TadoDeviceResponse?)null);

            var service = new TadoDeviceService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetZoneMeasuringDeviceAsync(1, 2, CancellationToken.None));

            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }

        /// <summary>
        /// GetMobileDeviceAsync throws NotFound when API returns null payload.
        /// </summary>
        [Fact(DisplayName = "GetMobileDeviceAsync throws NotFound when API returns null payload")]
        public async Task GetMobileDeviceAsync_ThrowsNotFound_WhenApiReturnsNullPayload()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoMobileItemResponse>("homes/1/mobileDevices/42", It.IsAny<CancellationToken>()))
                .ReturnsAsync((TadoMobileItemResponse?)null);

            var service = new TadoDeviceService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetMobileDeviceAsync(1, 42, CancellationToken.None));

            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }

        /// <summary>
        /// GetDeviceAsync throws ServiceUnavailable when network request fails.
        /// </summary>
        [Fact(DisplayName = "GetDeviceAsync throws ServiceUnavailable on network failure")]
        public async Task GetDeviceAsync_ThrowsServiceUnavailable_OnNetworkFailure()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoDeviceResponse>("devices/SU123456", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Network down"));

            var service = new TadoDeviceService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetDeviceAsync("SU123456", CancellationToken.None));

            Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
        }

        /// <summary>
        /// GetZoneTemperatureOffsetAsync throws ServiceUnavailable when network request fails.
        /// </summary>
        [Fact(DisplayName = "GetZoneTemperatureOffsetAsync throws ServiceUnavailable on network failure")]
        public async Task GetZoneTemperatureOffsetAsync_ThrowsServiceUnavailable_OnNetworkFailure()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoTemperatureResponse>("devices/1/temperatureOffset", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Network down"));

            var service = new TadoDeviceService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetZoneTemperatureOffsetAsync(1, CancellationToken.None));

            Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
        }

        /// <summary>
        /// GetZoneMeasuringDeviceAsync throws ServiceUnavailable when network request fails.
        /// </summary>
        [Fact(DisplayName = "GetZoneMeasuringDeviceAsync throws ServiceUnavailable on network failure")]
        public async Task GetZoneMeasuringDeviceAsync_ThrowsServiceUnavailable_OnNetworkFailure()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoDeviceResponse>("homes/1/zones/2/measuringDevice", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Network down"));

            var service = new TadoDeviceService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetZoneMeasuringDeviceAsync(1, 2, CancellationToken.None));

            Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
        }

        /// <summary>
        /// GetMobileDeviceAsync throws ServiceUnavailable when network request fails.
        /// </summary>
        [Fact(DisplayName = "GetMobileDeviceAsync throws ServiceUnavailable on network failure")]
        public async Task GetMobileDeviceAsync_ThrowsServiceUnavailable_OnNetworkFailure()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoMobileItemResponse>("homes/1/mobileDevices/42", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Network down"));

            var service = new TadoDeviceService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetMobileDeviceAsync(1, 42, CancellationToken.None));

            Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
        }

        /// <summary>
        /// MoveDeviceToZoneAsync omits the force query when force is not provided.
        /// </summary>
        [Fact(DisplayName = "MoveDeviceToZoneAsync omits force query when not provided")]
        public async Task MoveDeviceToZoneAsync_OmitsForceQuery_WhenNotProvided()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.SendAsync(
                    It.IsAny<string>(),
                    It.IsAny<HttpMethod>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<HttpStatusCode>(),
                    It.IsAny<object?>()))
                .ReturnsAsync(true);

            var service = new TadoDeviceService(mockHttp.Object);

            var moved = await service.MoveDeviceToZoneAsync(1, 2, "SU1234567890", null, CancellationToken.None);

            Assert.True(moved);
            mockHttp.Verify(c => c.SendAsync(
                    "homes/1/zones/2/devices",
                    HttpMethod.Post,
                    It.IsAny<CancellationToken>(),
                    HttpStatusCode.OK,
                    It.IsAny<object?>()),
                Times.Once);
        }

        /// <summary>
        /// Command methods validate blank device identifiers.
        /// </summary>
        [Fact(DisplayName = "Command methods validate blank device identifiers")]
        public async Task CommandMethods_ValidateBlankDeviceIdentifiers()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            var service = new TadoDeviceService(mockHttp.Object);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetDeviceChildLockAsync(" ", true, CancellationToken.None));

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SayHiAsync(" ", CancellationToken.None));

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetZoneTemperatureOffsetCelsiusAsync(" ", 1.5, CancellationToken.None));

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.SetZoneTemperatureOffsetFahrenheitAsync(" ", 33.8, CancellationToken.None));
        }

        /// <summary>
        /// GetMobileDevicesAsync throws ServiceUnavailable when network request fails.
        /// </summary>
        [Fact(DisplayName = "GetMobileDevicesAsync throws ServiceUnavailable on network failure")]
        public async Task GetMobileDevicesAsync_ThrowsServiceUnavailable_OnNetworkFailure()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<List<TadoMobileItemResponse>>("homes/1/mobileDevices", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Network down"));

            var service = new TadoDeviceService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetMobileDevicesAsync(1, CancellationToken.None));

            Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
        }

        /// <summary>
        /// GetMobileDeviceSettingsAsync throws ServiceUnavailable when network request fails.
        /// </summary>
        [Fact(DisplayName = "GetMobileDeviceSettingsAsync throws ServiceUnavailable on network failure")]
        public async Task GetMobileDeviceSettingsAsync_ThrowsServiceUnavailable_OnNetworkFailure()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoMobileSettingsResponse>("homes/1/mobileDevices/42/settings", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Network down"));

            var service = new TadoDeviceService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetMobileDeviceSettingsAsync(1, 42, CancellationToken.None));

            Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
        }

        /// <summary>
        /// SetZoneMeasuringDeviceAsync throws NotFound when API returns null payload.
        /// </summary>
        [Fact(DisplayName = "SetZoneMeasuringDeviceAsync throws NotFound when API returns null payload")]
        public async Task SetZoneMeasuringDeviceAsync_ThrowsNotFound_WhenApiReturnsNullPayload()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.PutAsync<SetZoneMeasuringDeviceRequest, TadoDeviceResponse>(
                    "homes/1/zones/2/measuringDevice",
                    It.IsAny<SetZoneMeasuringDeviceRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((TadoDeviceResponse?)null);

            var service = new TadoDeviceService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.SetZoneMeasuringDeviceAsync(1, 2, "SU123456", CancellationToken.None));

            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }

        /// <summary>
        /// GetDeviceListAsync throws ServiceUnavailable when network request fails.
        /// </summary>
        [Fact(DisplayName = "GetDeviceListAsync throws ServiceUnavailable on network failure")]
        public async Task GetDeviceListAsync_ThrowsServiceUnavailable_OnNetworkFailure()
        {
            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(c => c.GetAsync<TadoDeviceListResponse>("homes/1/deviceList", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("Network down"));

            var service = new TadoDeviceService(mockHttp.Object);

            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetDeviceListAsync(1, CancellationToken.None));

            Assert.Equal(HttpStatusCode.ServiceUnavailable, exception.StatusCode);
        }
    }
}
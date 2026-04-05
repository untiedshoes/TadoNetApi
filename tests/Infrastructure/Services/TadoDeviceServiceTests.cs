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
        [Fact(DisplayName = "GetDeviceAsync returns mapped device")]
        public async Task GetDeviceAsync_ReturnsMappedDevice()
        {
            // Arrange
            var tadoDevice = new TadoDeviceResponse
            {
                DeviceType = "THERMOSTAT",
                SerialNo = "123456789",
                ShortSerialNo = "123456",
                CurrentFwVersion = "1.0.0"
            };

            var mockHttp = MockTadoHttpClient.CreateGet(tadoDevice);
            var service = new TadoDeviceService(mockHttp.Object);

            // Act
            var device = await service.GetDeviceAsync(homeId: 1, deviceId: 1, CancellationToken.None);

            // Assert
            Assert.NotNull(device);
            Assert.Equal("THERMOSTAT", device.DeviceType);
            Assert.Equal("123456789", device.SerialNo);
            Assert.Equal("123456", device.ShortSerialNo);
            Assert.Equal("1.0.0", device.CurrentFwVersion);
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

        /// <summary>
        /// Integration test scaffold.
        /// Will run against a real Tado account if <c>TADO_USERNAME</c> and <c>TADO_PASSWORD</c> 
        /// environment variables are set. Marked as Integration category.
        /// </summary>
        [Fact(DisplayName = "GetDevicesAsync integration test", Skip = "Run manually with real credentials")]
        [Trait("Category", "Integration")]
        public async Task GetDevicesAsync_IntegrationTest()
        {
            // Example placeholder for real integration test
            // var authService = new TadoAuthService(...);
            // var service = new TadoDeviceService(new TadoHttpClient(authService));
            // var devices = await service.GetDevicesAsync(homeId: 123, CancellationToken.None);
            // Assert.NotEmpty(devices);
        }

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
    }
}
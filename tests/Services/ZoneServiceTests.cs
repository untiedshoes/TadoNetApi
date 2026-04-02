using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Enums;
using TadoNetApi.Application.Services;
using TadoNetApi.Domain.Interfaces;
using Xunit;

namespace TadoNetApi.Tests.Services
{
    /// <summary>
    /// Unit tests for <see cref="ZoneAppService"/>.
    /// Covers zone retrieval, zone state mapping, zone summary mapping, and error handling.
    /// </summary>
    public class ZoneServiceTests
    {
        /// <summary>
        /// Creates a <see cref="ZoneAppService"/> with a mocked IZoneService.
        /// </summary>
        /// <returns>A ZoneAppService instance with mocked IZoneService.</returns>
        private (ZoneAppService service, Mock<IZoneService> mock) CreateService()
        {
            var mockZoneService = new Mock<IZoneService>();
            var service = new ZoneAppService(mockZoneService.Object);
            return (service, mockZoneService);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.GetZonesAsync"/> returns correctly mapped zones.
        /// </summary>
        [Fact]
        public async Task GetZonesAsync_ReturnsMappedZones()
        {
            // Arrange
            var expectedZones = new List<Zone>
            {
                new() { Id = 1, Name = "Living Room", CurrentType = "HEATING" },
                new() { Id = 2, Name = "Bedroom", CurrentType = "HEATING" }
            };

            var (service, mock) = CreateService();
            mock.Setup(s => s.GetZonesAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedZones);

            // Act
            var zones = await service.GetZonesAsync(1);

            // Assert
            Assert.Equal(2, zones.Count);
            Assert.Equal("Living Room", zones[0].Name);
            Assert.Equal("Bedroom", zones[1].Name);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.GetZoneAsync"/> returns a correctly mapped zone.
        /// </summary>
        [Fact]
        public async Task GetZoneAsync_ReturnsMappedZone()
        {
            // Arrange
            var expectedZone = new Zone
            {
                Id = 1,
                Name = "Living Room",
                CurrentType = "HEATING"
            };

            var (service, mock) = CreateService();
            mock.Setup(s => s.GetZoneAsync(1, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedZone);

            // Act
            var zone = await service.GetZoneAsync(1, 1);

            // Assert
            Assert.NotNull(zone);
            Assert.Equal(1, zone.Id);
            Assert.Equal("Living Room", zone.Name);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.GetZoneStateAsync"/> returns a correctly mapped state.
        /// </summary>
        [Fact]
        public async Task GetZoneStateAsync_ReturnsMappedState()
        {
            // Arrange
            var expectedState = new State
            {
                TadoMode = "HOME",
                GeolocationOverride = true,
                Setting = new Setting
                {
                    Power = PowerStates.On,
                    Temperature = new Temperature { Celsius = 22 }
                }
            };

            var (service, mock) = CreateService();
            mock.Setup(s => s.GetZoneStateAsync(1, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedState);

            // Act
            var state = await service.GetZoneStateAsync(1, 1);

            // Assert
            Assert.NotNull(state);
            Assert.Equal("HOME", state.TadoMode);
            Assert.True(state.GeolocationOverride);
            Assert.Equal(PowerStates.On, state.Setting?.Power);
            Assert.Equal(22, state.Setting?.Temperature?.Celsius);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.GetZoneSummaryAsync"/> returns a correctly mapped zone summary.
        /// </summary>
        [Fact]
        public async Task GetZoneSummaryAsync_ReturnsMappedSummary()
        {
            // Arrange
            var expectedSummary = new ZoneSummary
            {
                Setting = new Setting
                {
                    Power = PowerStates.On,
                    Temperature = new Temperature { Celsius = 21.5 }
                },
                Termination = new Termination
                {
                    Type = DurationModes.Timer.ToString(),
                    DurationInSeconds = 3600
                }
            };

            var (service, mock) = CreateService();
            mock.Setup(s => s.GetZoneSummaryAsync(1, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSummary);

            // Act
            var summary = await service.GetZoneSummaryAsync(1, 1);

            // Assert
            Assert.NotNull(summary);
            Assert.Equal(PowerStates.On, summary.Setting?.Power);
            Assert.Equal(21.5, summary.Setting?.Temperature?.Celsius);
            Assert.Equal(DurationModes.Timer.ToString(), summary.Termination?.Type);
            Assert.Equal(3600, summary.Termination?.DurationInSeconds);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.GetZoneCapabilitiesAsync"/> returns correctly mapped capabilities.
        /// </summary>
        [Fact]
        public async Task GetZoneCapabilitiesAsync_ReturnsMappedCapabilities()
        {
            // Arrange
            var expectedCapabilities = new List<Capability>
            {
                new()
                {
                    PurpleType = "HEATING",
                    Temperatures = new Temperatures
                    {
                        Celsius = new TemperatureSteps { Min = 5, Max = 25, Step = 1 },
                        Fahrenheit = new TemperatureSteps { Min = 41, Max = 77, Step = 1 }
                    }
                }
            };

            var (service, mock) = CreateService();
            mock.Setup(s => s.GetZoneCapabilitiesAsync(1, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCapabilities);

            // Act
            var capabilities = await service.GetZoneCapabilitiesAsync(1, 1);

            // Assert
            Assert.Single(capabilities);
            Assert.Equal("HEATING", capabilities[0].PurpleType);
            Assert.NotNull(capabilities[0].Temperatures);
            Assert.Equal(5, capabilities[0].Temperatures?.Celsius?.Min);
            Assert.Equal(25, capabilities[0].Temperatures?.Celsius?.Max);
            Assert.Equal(1, capabilities[0].Temperatures?.Celsius?.Step);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.GetEarlyStartAsync"/> returns early start settings.
        /// </summary>
        [Fact]
        public async Task GetEarlyStartAsync_ReturnsEarlyStart()
        {
            // Arrange
            var expectedEarlyStart = new EarlyStart { Enabled = true };

            var (service, mock) = CreateService();
            mock.Setup(s => s.GetEarlyStartAsync(1, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedEarlyStart);

            // Act
            var earlyStart = await service.GetEarlyStartAsync(1, 1);

            // Assert
            Assert.NotNull(earlyStart);
            Assert.True(earlyStart.Enabled);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.GetZoneTemperatureOffsetAsync"/> returns the zone temperature offset.
        /// </summary>
        [Fact]
        public async Task GetZoneTemperatureOffsetAsync_ReturnsTemperatureOffset()
        {
            // Arrange
            var expectedTemperature = new Temperature { Celsius = 1.0, Fahrenheit = 33.8 };
            var zone = new Zone { Devices = new[] { new Device { ShortSerialNo = "ABC123" } } };

            var (service, mock) = CreateService();
            mock.Setup(s => s.GetZoneTemperatureOffsetAsync(zone, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTemperature);

            // Act
            var result = await service.GetZoneTemperatureOffsetAsync(zone);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1.0, result.Celsius);
            Assert.Equal(33.8, result.Fahrenheit);
        }
    }
}
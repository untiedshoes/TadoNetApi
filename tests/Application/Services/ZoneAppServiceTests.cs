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

namespace TadoNetApi.Tests.Application.Services
{
    /// <summary>
    /// Unit tests for <see cref="ZoneAppService"/>.
    /// Covers zone retrieval, zone state mapping, zone summary mapping, and error handling.
    /// </summary>
    public class ZoneAppServiceTests
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
        [Fact(DisplayName = "GetZonesAsync returns mapped zones")]
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
        [Fact(DisplayName = "GetZoneAsync returns mapped zone")]
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
        [Fact(DisplayName = "GetZoneStateAsync returns mapped state")]
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
        [Fact(DisplayName = "GetZoneSummaryAsync returns mapped summary")]
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
        [Fact(DisplayName = "GetZoneCapabilitiesAsync returns mapped capabilities")]
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
        /// Tests that <see cref="ZoneAppService.DeleteZoneOverlayAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "DeleteZoneOverlayAsync passes through to domain service")]
        public async Task DeleteZoneOverlayAsync_PassesThroughToDomainService()
        {
            var (service, mock) = CreateService();
            mock.Setup(s => s.DeleteZoneOverlayAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var deleted = await service.DeleteZoneOverlayAsync(1, 2, CancellationToken.None);

            Assert.True(deleted);
            mock.Verify(s => s.DeleteZoneOverlayAsync(1, 2, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.SetHeatingCircuitAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "SetHeatingCircuitAsync passes through to domain service")]
        public async Task SetHeatingCircuitAsync_PassesThroughToDomainService()
        {
            var expected = new ZoneControl { HeatingCircuit = 3 };
            var (service, mock) = CreateService();
            mock.Setup(s => s.SetHeatingCircuitAsync(1, 2, 3, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await service.SetHeatingCircuitAsync(1, 2, 3, CancellationToken.None);

            Assert.Same(expected, result);
            mock.Verify(s => s.SetHeatingCircuitAsync(1, 2, 3, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.SetHeatingTemperatureFahrenheitAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "SetHeatingTemperatureFahrenheitAsync passes through to domain service")]
        public async Task SetHeatingTemperatureFahrenheitAsync_PassesThroughToDomainService()
        {
            var expected = new ZoneSummary();
            var (service, mock) = CreateService();
            mock.Setup(s => s.SetHeatingTemperatureFahrenheitAsync(1, 2, 72.5, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await service.SetHeatingTemperatureFahrenheitAsync(1, 2, 72.5, CancellationToken.None);

            Assert.Same(expected, result);
            mock.Verify(s => s.SetHeatingTemperatureFahrenheitAsync(1, 2, 72.5, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.SetHotWaterTemperatureCelsiusAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "SetHotWaterTemperatureCelsiusAsync passes through to domain service")]
        public async Task SetHotWaterTemperatureCelsiusAsync_PassesThroughToDomainService()
        {
            var expected = new ZoneSummary();
            var (service, mock) = CreateService();
            mock.Setup(s => s.SetHotWaterTemperatureCelsiusAsync(1, 2, 55.0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await service.SetHotWaterTemperatureCelsiusAsync(1, 2, 55.0, CancellationToken.None);

            Assert.Same(expected, result);
            mock.Verify(s => s.SetHotWaterTemperatureCelsiusAsync(1, 2, 55.0, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.SetHotWaterTemperatureFahrenheitAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "SetHotWaterTemperatureFahrenheitAsync passes through to domain service")]
        public async Task SetHotWaterTemperatureFahrenheitAsync_PassesThroughToDomainService()
        {
            var expected = new ZoneSummary();
            var (service, mock) = CreateService();
            mock.Setup(s => s.SetHotWaterTemperatureFahrenheitAsync(1, 2, 131.0, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await service.SetHotWaterTemperatureFahrenheitAsync(1, 2, 131.0, CancellationToken.None);

            Assert.Same(expected, result);
            mock.Verify(s => s.SetHotWaterTemperatureFahrenheitAsync(1, 2, 131.0, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.SwitchHeatingOffAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "SwitchHeatingOffAsync passes through to domain service")]
        public async Task SwitchHeatingOffAsync_PassesThroughToDomainService()
        {
            var expected = new ZoneSummary();
            var (service, mock) = CreateService();
            mock.Setup(s => s.SwitchHeatingOffAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await service.SwitchHeatingOffAsync(1, 2, CancellationToken.None);

            Assert.Same(expected, result);
            mock.Verify(s => s.SwitchHeatingOffAsync(1, 2, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.SwitchHotWaterOffAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "SwitchHotWaterOffAsync passes through to domain service")]
        public async Task SwitchHotWaterOffAsync_PassesThroughToDomainService()
        {
            var expected = new ZoneSummary();
            var (service, mock) = CreateService();
            mock.Setup(s => s.SwitchHotWaterOffAsync(1, 2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await service.SwitchHotWaterOffAsync(1, 2, CancellationToken.None);

            Assert.Same(expected, result);
            mock.Verify(s => s.SwitchHotWaterOffAsync(1, 2, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.GetZoneControlAsync"/> returns correctly mapped zone control details.
        /// </summary>
        [Fact(DisplayName = "GetZoneControlAsync returns mapped zone control")]
        public async Task GetZoneControlAsync_ReturnsMappedZoneControl()
        {
            // Arrange
            var expectedZoneControl = new ZoneControl
            {
                Type = "HEATING",
                EarlyStartEnabled = true,
                HeatingCircuit = 1,
                Duties = new ZoneControlDuties
                {
                    Type = "HEATING",
                    Leader = new Device { SerialNo = "SU1234567890" }
                }
            };

            var (service, mock) = CreateService();
            mock.Setup(s => s.GetZoneControlAsync(1, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedZoneControl);

            // Act
            var control = await service.GetZoneControlAsync(1, 1);

            // Assert
            Assert.NotNull(control);
            Assert.Equal("HEATING", control.Type);
            Assert.True(control.EarlyStartEnabled);
            Assert.Equal(1, control.HeatingCircuit);
            Assert.Equal("SU1234567890", control.Duties?.Leader?.SerialNo);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.GetDefaultZoneOverlayAsync"/> returns correctly mapped default overlay details.
        /// </summary>
        [Fact(DisplayName = "GetDefaultZoneOverlayAsync returns mapped default overlay")]
        public async Task GetDefaultZoneOverlayAsync_ReturnsMappedDefaultOverlay()
        {
            // Arrange
            var expectedDefaultOverlay = new DefaultZoneOverlay
            {
                TerminationCondition = new Termination
                {
                    Type = "Timer",
                    DurationInSeconds = 900
                }
            };

            var (service, mock) = CreateService();
            mock.Setup(s => s.GetDefaultZoneOverlayAsync(1, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDefaultOverlay);

            // Act
            var defaultOverlay = await service.GetDefaultZoneOverlayAsync(1, 1);

            // Assert
            Assert.NotNull(defaultOverlay);
            Assert.Equal("Timer", defaultOverlay.TerminationCondition?.Type);
            Assert.Equal(900, defaultOverlay.TerminationCondition?.DurationInSeconds);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.GetEarlyStartAsync"/> returns early start settings.
        /// </summary>
        [Fact(DisplayName = "GetEarlyStartAsync returns early start")]
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
        /// Tests that <see cref="ZoneAppService.GetAwayConfigurationAsync"/> returns away-configuration settings.
        /// </summary>
        [Fact(DisplayName = "GetAwayConfigurationAsync returns away configuration")]
        public async Task GetAwayConfigurationAsync_ReturnsAwayConfiguration()
        {
            // Arrange
            var expectedAwayConfiguration = new AwayConfiguration
            {
                Type = "HEATING",
                AutoAdjust = false,
                ComfortLevel = "BALANCE",
                Setting = new Setting
                {
                    Power = PowerStates.On,
                    Temperature = new Temperature { Celsius = 15 }
                }
            };

            var (service, mock) = CreateService();
            mock.Setup(s => s.GetAwayConfigurationAsync(1, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedAwayConfiguration);

            // Act
            var awayConfiguration = await service.GetAwayConfigurationAsync(1, 1);

            // Assert
            Assert.NotNull(awayConfiguration);
            Assert.Equal("HEATING", awayConfiguration.Type);
            Assert.False(awayConfiguration.AutoAdjust);
            Assert.Equal("BALANCE", awayConfiguration.ComfortLevel);
            Assert.Equal(PowerStates.On, awayConfiguration.Setting?.Power);
            Assert.Equal(15, awayConfiguration.Setting?.Temperature?.Celsius);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.CreateZoneAsync"/> delegates to the domain service.
        /// </summary>
        [Fact(DisplayName = "CreateZoneAsync passes through to domain service")]
        public async Task CreateZoneAsync_PassesThroughToDomainService()
        {
            var (service, mock) = CreateService();
            IReadOnlyList<string> deviceSerials = ["SU1234567890", "VA1234567890"];

            mock.Setup(s => s.CreateZoneAsync(1, "HEATING", deviceSerials, true, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await service.CreateZoneAsync(1, "HEATING", deviceSerials, true, CancellationToken.None);

            mock.Verify(s => s.CreateZoneAsync(1, "HEATING", deviceSerials, true, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.GetZoneDayReportAsync"/> returns the day-report payload.
        /// </summary>
        [Fact(DisplayName = "GetZoneDayReportAsync returns zone day report")]
        public async Task GetZoneDayReportAsync_ReturnsZoneDayReport()
        {
            var expectedDayReport = new ZoneDayReport
            {
                ZoneType = "HEATING",
                HoursInDay = 24,
                Interval = new ZoneDayReportInterval
                {
                    From = new DateTime(2026, 4, 4, 0, 0, 0, DateTimeKind.Utc),
                    To = new DateTime(2026, 4, 4, 23, 59, 59, DateTimeKind.Utc)
                },
                MeasuredData = new ZoneDayReportMeasuredData
                {
                    InsideTemperature = new ZoneDayReportTemperatureTimeSeries
                    {
                        ValueType = "temperature",
                        DataPoints =
                        [
                            new ZoneDayReportTemperatureDataPoint
                            {
                                Timestamp = new DateTime(2026, 4, 4, 12, 0, 0, DateTimeKind.Utc),
                                Value = new Temperature { Celsius = 21.5 }
                            }
                        ]
                    }
                }
            };

            var (service, mock) = CreateService();
            mock.Setup(s => s.GetZoneDayReportAsync(1, 1, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDayReport);

            var dayReport = await service.GetZoneDayReportAsync(1, 1);

            Assert.NotNull(dayReport);
            Assert.Equal("HEATING", dayReport.ZoneType);
            Assert.Equal(24, dayReport.HoursInDay);
            Assert.Equal(new DateTime(2026, 4, 4, 0, 0, 0, DateTimeKind.Utc), dayReport.Interval?.From);
            Assert.Equal(21.5, dayReport.MeasuredData?.InsideTemperature?.DataPoints?[0].Value?.Celsius);
        }

        /// <summary>
        /// Tests that <see cref="ZoneAppService.GetZoneTemperatureOffsetAsync"/> returns the zone temperature offset.
        /// </summary>
        [Fact(DisplayName = "GetZoneTemperatureOffsetAsync returns temperature offset")]
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
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TadoNetApi.Domain.Enums;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Exceptions;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Services;
using TadoNetApi.Tests.Mocks;
using Moq;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Services
{
    public class TadoZoneServiceReadTests
    {
        /// <summary>
        /// GetZonesAsync returns mapped zones when API returns valid response.
        /// </summary>
        [Fact(DisplayName = "GetZonesAsync returns mapped zones when API returns valid response")]
        public async Task GetZonesAsync_ShouldReturnZones_WhenApiReturnsValidResponse()
        {
            // Arrange
            var response = new List<TadoZoneResponse>
            {
                new TadoZoneResponse { Id = 1, Name = "Living Room", CurrentType = "HEATING" },
                new TadoZoneResponse { Id = 2, Name = "Bedroom", CurrentType = "HEATING" }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoZoneService(mockHttp.Object);

            // Act
            var zones = await service.GetZonesAsync(homeId: 1, CancellationToken.None);

            // Assert
            Assert.Equal(2, zones.Count);
            Assert.Equal("Living Room", zones[0].Name);
            Assert.Equal("Bedroom", zones[1].Name);
        }

        /// <summary>
        /// GetZonesAsync throws TadoApiException with 401 when API returns Unauthorized.
        /// </summary>
        [Fact(DisplayName = "GetZonesAsync throws TadoApiException with 401 when API returns Unauthorized")]
        public async Task GetZonesAsync_ShouldThrowTadoApiException_WhenApiReturns401()
        {
            // Arrange
            var mockHttp = MockTadoHttpClient.CreateGet<List<TadoZoneResponse>>(
                returnValue: null!,
                transientFailures: int.MaxValue,
                transientException: new TadoApiException(HttpStatusCode.Unauthorized, "Unauthorized"));

            var service = new TadoZoneService(mockHttp.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetZonesAsync(homeId: 1, CancellationToken.None));

            Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        }

        /// <summary>
        /// GetZonesAsync propagates TadoApiException with RequestTimeout when request times out.
        /// </summary>
        [Fact(DisplayName = "GetZonesAsync propagates TadoApiException with RequestTimeout when request times out")]
        public async Task GetZonesAsync_ShouldThrowTadoApiException_WhenRequestTimesOut()
        {
            // Arrange — TadoHttpClient translates TaskCanceledException → TadoApiException(RequestTimeout)
            var mockHttp = MockTadoHttpClient.CreateGet<List<TadoZoneResponse>>(
                returnValue: null!,
                transientFailures: int.MaxValue,
                transientException: new TadoApiException(HttpStatusCode.RequestTimeout, "Request timed out"));

            var service = new TadoZoneService(mockHttp.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<TadoApiException>(() =>
                service.GetZonesAsync(homeId: 1, CancellationToken.None));

            Assert.Equal(HttpStatusCode.RequestTimeout, exception.StatusCode);
        }

        /// <summary>
        /// GetZoneControlAsync returns mapped zone control.
        /// </summary>
        [Fact(DisplayName = "GetZoneControlAsync returns mapped zone control")]
        public async Task GetZoneControlAsync_ReturnsMappedZoneControl()
        {
            // Arrange
            var response = new TadoZoneControlResponse
            {
                Type = "HEATING",
                EarlyStartEnabled = true,
                HeatingCircuit = 1,
                Duties = new TadoZoneControlDutiesResponse
                {
                    Type = "HEATING",
                    Leader = new TadoDeviceResponse
                    {
                        DeviceType = "SU02",
                        SerialNo = "SU1234567890",
                        ShortSerialNo = "SU1234567890"
                    },
                    Drivers =
                    [
                        new TadoDeviceResponse
                        {
                            DeviceType = "VA02",
                            SerialNo = "VA1234567890",
                            ShortSerialNo = "VA1234567890"
                        }
                    ]
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoZoneService(mockHttp.Object);

            // Act
            var control = await service.GetZoneControlAsync(homeId: 1, zoneId: 2, CancellationToken.None);

            // Assert
            Assert.Equal("HEATING", control.Type);
            Assert.True(control.EarlyStartEnabled);
            Assert.Equal(1, control.HeatingCircuit);
            Assert.Equal("SU1234567890", control.Duties?.Leader?.SerialNo);
            Assert.Single(control.Duties?.Drivers!);
            Assert.Equal("VA1234567890", control.Duties?.Drivers?[0].SerialNo);
        }

        /// <summary>
        /// GetDefaultZoneOverlayAsync returns mapped default overlay.
        /// </summary>
        [Fact(DisplayName = "GetDefaultZoneOverlayAsync returns mapped default overlay")]
        public async Task GetDefaultZoneOverlayAsync_ReturnsMappedDefaultOverlay()
        {
            // Arrange
            var response = new TadoDefaultZoneOverlayResponse
            {
                TerminationCondition = new TadoTerminationResponse
                {
                    CurrentType = DurationModes.Timer,
                    DurationInSeconds = 900
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoZoneService(mockHttp.Object);

            // Act
            var defaultOverlay = await service.GetDefaultZoneOverlayAsync(homeId: 1, zoneId: 2, CancellationToken.None);

            // Assert
            Assert.NotNull(defaultOverlay);
            Assert.Equal("Timer", defaultOverlay.TerminationCondition?.Type);
            Assert.Equal(900, defaultOverlay.TerminationCondition?.DurationInSeconds);
        }

        /// <summary>
        /// GetAwayConfigurationAsync returns mapped away configuration.
        /// </summary>
        [Fact(DisplayName = "GetAwayConfigurationAsync returns mapped away configuration")]
        public async Task GetAwayConfigurationAsync_ReturnsMappedAwayConfiguration()
        {
            // Arrange
            var response = new TadoAwayConfigurationResponse
            {
                Type = "HEATING",
                AutoAdjust = false,
                ComfortLevel = "BALANCE",
                Setting = new TadoSettingResponse
                {
                    Power = PowerStates.On,
                    Temperature = new TadoTemperatureResponse { Celsius = 15 }
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoZoneService(mockHttp.Object);

            // Act
            var awayConfiguration = await service.GetAwayConfigurationAsync(homeId: 1, zoneId: 2, CancellationToken.None);

            // Assert
            Assert.NotNull(awayConfiguration);
            Assert.Equal("HEATING", awayConfiguration.Type);
            Assert.False(awayConfiguration.AutoAdjust);
            Assert.Equal("BALANCE", awayConfiguration.ComfortLevel);
            Assert.Equal(PowerStates.On, awayConfiguration.Setting?.Power);
            Assert.Equal(15, awayConfiguration.Setting?.Temperature?.Celsius);
        }

        /// <summary>
        /// GetActiveTimetableTypeAsync returns mapped timetable type.
        /// </summary>
        [Fact(DisplayName = "GetActiveTimetableTypeAsync returns mapped timetable type")]
        public async Task GetActiveTimetableTypeAsync_ReturnsMappedTimetableType()
        {
            var response = new TadoTimetableTypeResponse
            {
                Id = 1,
                Type = "ONE_DAY"
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoZoneService(mockHttp.Object);

            var timetableType = await service.GetActiveTimetableTypeAsync(homeId: 1, zoneId: 2, CancellationToken.None);

            Assert.NotNull(timetableType);
            Assert.Equal(1, timetableType.Id);
            Assert.Equal("ONE_DAY", timetableType.Type);
        }

        /// <summary>
        /// GetZoneTimetablesAsync returns mapped timetable types.
        /// </summary>
        [Fact(DisplayName = "GetZoneTimetablesAsync returns mapped timetable types")]
        public async Task GetZoneTimetablesAsync_ReturnsMappedTimetableTypes()
        {
            var response = new List<TadoTimetableTypeResponse>
            {
                new() { Id = 1, Type = "ONE_DAY" },
                new() { Id = 2, Type = "THREE_DAY" },
                new() { Id = 3, Type = "SEVEN_DAY" }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoZoneService(mockHttp.Object);

            var timetableTypes = await service.GetZoneTimetablesAsync(homeId: 1, zoneId: 2, CancellationToken.None);

            Assert.Equal(3, timetableTypes.Count);
            Assert.Equal("ONE_DAY", timetableTypes[0].Type);
            Assert.Equal("SEVEN_DAY", timetableTypes[2].Type);
        }

        /// <summary>
        /// GetZoneTimetableAsync uses the timetable type route and returns the mapped result.
        /// </summary>
        [Fact(DisplayName = "GetZoneTimetableAsync uses the timetable type route and returns the mapped result")]
        public async Task GetZoneTimetableAsync_UsesExpectedRoute()
        {
            var response = new TadoTimetableTypeResponse
            {
                Id = 3,
                Type = "SEVEN_DAY"
            };

            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(client => client.GetAsync<TadoTimetableTypeResponse>(
                    "homes/1/zones/2/schedule/timetables/3",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var service = new TadoZoneService(mockHttp.Object);

            var timetableType = await service.GetZoneTimetableAsync(1, 2, 3, CancellationToken.None);

            Assert.NotNull(timetableType);
            Assert.Equal(3, timetableType.Id);
            Assert.Equal("SEVEN_DAY", timetableType.Type);
            mockHttp.VerifyAll();
        }

        /// <summary>
        /// GetZoneTimetableBlocksAsync returns mapped timetable blocks.
        /// </summary>
        [Fact(DisplayName = "GetZoneTimetableBlocksAsync returns mapped timetable blocks")]
        public async Task GetZoneTimetableBlocksAsync_ReturnsMappedTimetableBlocks()
        {
            var response = new List<TadoTimetableBlockResponse>
            {
                new()
                {
                    DayType = "MONDAY",
                    Start = "06:00",
                    End = "08:00",
                    GeolocationOverride = false,
                    Setting = new TadoSettingResponse
                    {
                        Power = PowerStates.On,
                        Temperature = new TadoTemperatureResponse { Celsius = 20 }
                    }
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoZoneService(mockHttp.Object);

            var blocks = await service.GetZoneTimetableBlocksAsync(homeId: 1, zoneId: 2, timetableTypeId: 3, CancellationToken.None);

            Assert.Single(blocks);
            Assert.Equal("MONDAY", blocks[0].DayType);
            Assert.Equal("06:00", blocks[0].Start);
            Assert.False(blocks[0].GeolocationOverride);
            Assert.Equal(20, blocks[0].Setting?.Temperature?.Celsius);
        }

        /// <summary>
        /// GetTimetableBlocksByDayTypeAsync appends the day type to the route.
        /// </summary>
        [Fact(DisplayName = "GetTimetableBlocksByDayTypeAsync appends the day type to the route")]
        public async Task GetTimetableBlocksByDayTypeAsync_UsesExpectedRoute()
        {
            var response = new List<TadoTimetableBlockResponse>
            {
                new()
                {
                    DayType = "MONDAY",
                    Start = "08:00",
                    End = "10:00",
                    GeolocationOverride = true,
                    Setting = new TadoSettingResponse
                    {
                        Power = PowerStates.On,
                        Temperature = new TadoTemperatureResponse { Celsius = 21 }
                    }
                }
            };

            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(client => client.GetAsync<List<TadoTimetableBlockResponse>>(
                    "homes/1/zones/2/schedule/timetables/3/blocks/MONDAY",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var service = new TadoZoneService(mockHttp.Object);

            var blocks = await service.GetTimetableBlocksByDayTypeAsync(1, 2, 3, "MONDAY", CancellationToken.None);

            Assert.Single(blocks);
            Assert.True(blocks[0].GeolocationOverride);
            Assert.Equal(21, blocks[0].Setting?.Temperature?.Celsius);
            mockHttp.VerifyAll();
        }

        /// <summary>
        /// GetZoneDayReportAsync returns mapped typed day report.
        /// </summary>
        [Fact(DisplayName = "GetZoneDayReportAsync returns mapped typed day report")]
        public async Task GetZoneDayReportAsync_ReturnsMappedZoneDayReport()
        {
            var response = new TadoZoneDayReportResponse
            {
                ZoneType = "HEATING",
                HoursInDay = 24,
                Interval = new TadoZoneDayReportIntervalResponse
                {
                    From = new DateTime(2026, 4, 4, 0, 0, 0, DateTimeKind.Utc),
                    To = new DateTime(2026, 4, 4, 23, 59, 59, DateTimeKind.Utc)
                },
                MeasuredData = new TadoZoneDayReportMeasuredDataResponse
                {
                    MeasuringDeviceConnected = new TadoZoneDayReportBooleanTimeSeriesResponse
                    {
                        TimeSeriesType = "dataIntervals",
                        ValueType = "boolean",
                        DataIntervals =
                        [
                            new TadoZoneDayReportBooleanDataIntervalResponse
                            {
                                From = new DateTime(2026, 4, 4, 0, 0, 0, DateTimeKind.Utc),
                                To = new DateTime(2026, 4, 4, 23, 59, 59, DateTimeKind.Utc),
                                Value = true
                            }
                        ]
                    },
                    InsideTemperature = new TadoZoneDayReportTemperatureTimeSeriesResponse
                    {
                        TimeSeriesType = "dataPoints",
                        ValueType = "temperature",
                        Min = new TadoTemperatureResponse { Celsius = 18.5 },
                        Max = new TadoTemperatureResponse { Celsius = 21.5 },
                        DataPoints =
                        [
                            new TadoZoneDayReportTemperatureDataPointResponse
                            {
                                Timestamp = new DateTime(2026, 4, 4, 12, 0, 0, DateTimeKind.Utc),
                                Value = new TadoTemperatureResponse { Celsius = 21.5 }
                            }
                        ]
                    }
                },
                Weather = new TadoZoneDayReportWeatherResponse
                {
                    Slots = new TadoZoneDayReportWeatherSlotTimeSeriesResponse
                    {
                        TimeSeriesType = "slots",
                        ValueType = "weatherCondition",
                        Slots = new Dictionary<string, TadoZoneDayReportWeatherSlotResponse>
                        {
                            ["12:00"] = new()
                            {
                                State = "SUN",
                                Temperature = new TadoTemperatureResponse { Celsius = 14 }
                            }
                        }
                    }
                }
            };

            var mockHttp = MockTadoHttpClient.CreateGet(response);
            var service = new TadoZoneService(mockHttp.Object);

            var dayReport = await service.GetZoneDayReportAsync(homeId: 1, zoneId: 2, cancellationToken: CancellationToken.None);

            Assert.NotNull(dayReport);
            Assert.Equal("HEATING", dayReport.ZoneType);
            Assert.Equal(24, dayReport.HoursInDay);
            Assert.True(dayReport.MeasuredData?.MeasuringDeviceConnected?.DataIntervals?[0].Value);
            Assert.Equal(21.5, dayReport.MeasuredData?.InsideTemperature?.DataPoints?[0].Value?.Celsius);
            Assert.Equal("SUN", dayReport.Weather?.Slots?.Slots?["12:00"].State);
        }

        /// <summary>
        /// GetZoneDayReportAsync appends the optional date query.
        /// </summary>
        [Fact(DisplayName = "GetZoneDayReportAsync appends the optional date query")]
        public async Task GetZoneDayReportAsync_WithDate_UsesExpectedPath()
        {
            var response = new TadoZoneDayReportResponse
            {
                ZoneType = "HEATING"
            };

            var mockHttp = new Mock<ITadoHttpClient>();
            mockHttp
                .Setup(client => client.GetAsync<TadoZoneDayReportResponse>(
                    "homes/1/zones/2/dayReport?date=2026-04-04",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var service = new TadoZoneService(mockHttp.Object);

            var dayReport = await service.GetZoneDayReportAsync(1, 2, new DateOnly(2026, 4, 4), CancellationToken.None);

            Assert.NotNull(dayReport);
            Assert.Equal("HEATING", dayReport.ZoneType);
            mockHttp.VerifyAll();
        }
    }
}
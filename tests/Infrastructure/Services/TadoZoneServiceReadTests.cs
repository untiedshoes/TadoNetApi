using System.Threading;
using System.Threading.Tasks;
using TadoNetApi.Domain.Enums;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Http;
using TadoNetApi.Infrastructure.Services;
using TadoNetApi.Tests.Mocks;
using Moq;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Services
{
    public class TadoZoneServiceReadTests
    {
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
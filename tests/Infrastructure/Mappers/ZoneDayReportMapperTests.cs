using System;
using System.Collections.Generic;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Mappers;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Mappers
{
    /// <summary>
    /// Unit tests for <see cref="ZoneDayReportMapper"/>.
    /// </summary>
    public class ZoneDayReportMapperTests
    {
        /// <summary>
        /// ToDomain maps a full day report payload including nested interval and weather structures.
        /// </summary>
        [Fact(DisplayName = "ToDomain maps full day report payload including nested structures")]
        public void ToDomain_MapsFullDayReportPayload_IncludingNestedStructures()
        {
            var timestamp = new DateTime(2026, 5, 27, 12, 0, 0, DateTimeKind.Utc);
            var dto = new TadoZoneDayReportResponse
            {
                ZoneType = "HEATING",
                HoursInDay = 24,
                Interval = new TadoZoneDayReportIntervalResponse
                {
                    From = timestamp.AddHours(-12),
                    To = timestamp.AddHours(12)
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
                                From = timestamp.AddHours(-1),
                                To = timestamp,
                                Value = true
                            }
                        ]
                    },
                    InsideTemperature = new TadoZoneDayReportTemperatureTimeSeriesResponse
                    {
                        TimeSeriesType = "dataPoints",
                        ValueType = "temperature",
                        Min = new TadoTemperatureResponse { Celsius = 18.5, Fahrenheit = 65.3 },
                        Max = new TadoTemperatureResponse { Celsius = 22.5, Fahrenheit = 72.5 },
                        DataPoints =
                        [
                            new TadoZoneDayReportTemperatureDataPointResponse
                            {
                                Timestamp = timestamp,
                                Value = new TadoTemperatureResponse { Celsius = 21.1, Fahrenheit = 70.0 }
                            }
                        ]
                    },
                    Humidity = new TadoZoneDayReportPercentageTimeSeriesResponse
                    {
                        TimeSeriesType = "dataPoints",
                        ValueType = "humidity",
                        PercentageUnit = "%",
                        Min = 30,
                        Max = 60,
                        DataPoints =
                        [
                            new TadoZoneDayReportPercentageDataPointResponse
                            {
                                Timestamp = timestamp,
                                Value = 44
                            }
                        ]
                    }
                },
                Stripes = new TadoZoneDayReportStripesTimeSeriesResponse
                {
                    TimeSeriesType = "dataIntervals",
                    ValueType = "stripes",
                    DataIntervals =
                    [
                        new TadoZoneDayReportStripesDataIntervalResponse
                        {
                            From = timestamp.AddHours(-2),
                            To = timestamp.AddHours(-1),
                            Value = new TadoZoneDayReportStripeValueResponse
                            {
                                StripeType = "HEATING",
                                Setting = new TadoZoneDayReportSettingResponse
                                {
                                    Type = "HEATING",
                                    Power = "ON",
                                    Temperature = new TadoTemperatureResponse { Celsius = 20.0 },
                                    Mode = "AUTO",
                                    IsBoost = false
                                }
                            }
                        }
                    ]
                },
                Settings = new TadoZoneDayReportSettingTimeSeriesResponse
                {
                    TimeSeriesType = "dataIntervals",
                    ValueType = "setting",
                    DataIntervals =
                    [
                        new TadoZoneDayReportSettingDataIntervalResponse
                        {
                            From = timestamp.AddHours(-3),
                            To = timestamp.AddHours(-2),
                            Value = new TadoZoneDayReportSettingResponse
                            {
                                Type = "HEATING",
                                Power = "ON",
                                Temperature = new TadoTemperatureResponse { Celsius = 19.0 },
                                Mode = "MANUAL",
                                IsBoost = true
                            }
                        }
                    ]
                },
                CallForHeat = new TadoZoneDayReportCallForHeatTimeSeriesResponse
                {
                    TimeSeriesType = "dataIntervals",
                    ValueType = "callForHeat",
                    DataIntervals =
                    [
                        new TadoZoneDayReportCallForHeatDataIntervalResponse
                        {
                            From = timestamp.AddMinutes(-30),
                            To = timestamp,
                            Value = "HEATING"
                        }
                    ]
                },
                HotWaterProduction = new TadoZoneDayReportBooleanTimeSeriesResponse
                {
                    TimeSeriesType = "dataIntervals",
                    ValueType = "boolean",
                    DataIntervals =
                    [
                        new TadoZoneDayReportBooleanDataIntervalResponse
                        {
                            From = timestamp.AddHours(-4),
                            To = timestamp.AddHours(-3),
                            Value = false
                        }
                    ]
                },
                AcActivity = new TadoZoneDayReportPowerTimeSeriesResponse
                {
                    TimeSeriesType = "dataIntervals",
                    ValueType = "power",
                    DataIntervals =
                    [
                        new TadoZoneDayReportPowerDataIntervalResponse
                        {
                            From = timestamp.AddHours(-5),
                            To = timestamp.AddHours(-4),
                            Value = "LOW"
                        }
                    ]
                },
                Weather = new TadoZoneDayReportWeatherResponse
                {
                    Condition = new TadoZoneDayReportWeatherConditionTimeSeriesResponse
                    {
                        TimeSeriesType = "dataIntervals",
                        ValueType = "weatherCondition",
                        DataIntervals =
                        [
                            new TadoZoneDayReportWeatherConditionDataIntervalResponse
                            {
                                From = timestamp.AddHours(-1),
                                To = timestamp,
                                Value = new TadoZoneDayReportWeatherConditionValueResponse
                                {
                                    State = "SUN",
                                    Temperature = new TadoTemperatureResponse { Celsius = 14.0 }
                                }
                            }
                        ]
                    },
                    Sunny = new TadoZoneDayReportBooleanTimeSeriesResponse
                    {
                        TimeSeriesType = "dataIntervals",
                        ValueType = "boolean",
                        DataIntervals =
                        [
                            new TadoZoneDayReportBooleanDataIntervalResponse
                            {
                                From = timestamp.AddHours(-1),
                                To = timestamp,
                                Value = true
                            }
                        ]
                    },
                    Slots = new TadoZoneDayReportWeatherSlotTimeSeriesResponse
                    {
                        TimeSeriesType = "slots",
                        ValueType = "weatherSlot",
                        Slots = new Dictionary<string, TadoZoneDayReportWeatherSlotResponse>
                        {
                            ["12:00"] = new TadoZoneDayReportWeatherSlotResponse
                            {
                                State = "SUN",
                                Temperature = new TadoTemperatureResponse { Celsius = 15.2 }
                            }
                        }
                    }
                }
            };

            var result = dto.ToDomain();

            Assert.Equal("HEATING", result.ZoneType);
            Assert.Equal(24, result.HoursInDay);
            Assert.Equal(timestamp.AddHours(-12), result.Interval?.From);
            Assert.True(result.MeasuredData?.MeasuringDeviceConnected?.DataIntervals?[0].Value);
            Assert.Equal(21.1, result.MeasuredData?.InsideTemperature?.DataPoints?[0].Value?.Celsius);
            Assert.Equal(44, result.MeasuredData?.Humidity?.DataPoints?[0].Value);
            Assert.Equal("HEATING", result.Stripes?.DataIntervals?[0].Value?.StripeType);
            Assert.Equal("MANUAL", result.Settings?.DataIntervals?[0].Value?.Mode);
            Assert.Equal("HEATING", result.CallForHeat?.DataIntervals?[0].Value);
            Assert.Equal("LOW", result.AcActivity?.DataIntervals?[0].Value);
            Assert.Equal("SUN", result.Weather?.Condition?.DataIntervals?[0].Value?.State);
            Assert.Equal("SUN", result.Weather?.Slots?.Slots?["12:00"].State);
        }

        /// <summary>
        /// ToDomain throws ArgumentNullException when the source DTO is null.
        /// </summary>
        [Fact(DisplayName = "ToDomain throws ArgumentNullException when source DTO is null")]
        public void ToDomain_ThrowsArgumentNullException_WhenSourceDtoIsNull()
        {
            TadoZoneDayReportResponse dto = null!;

            Assert.Throws<ArgumentNullException>(() => ZoneDayReportMapper.ToDomain(dto));
        }
    }
}

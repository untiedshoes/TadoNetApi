using System;
using System.Collections.Generic;
using TadoNetApi.Domain.Enums;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Mappers;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Mappers
{
    /// <summary>
    /// Unit tests for <see cref="StateMapper"/>.
    /// </summary>
    public class StateMapperTests
    {
        /// <summary>
        /// ToDomain maps a full state payload including nested overlay, activity, and sensor data.
        /// </summary>
        [Fact(DisplayName = "ToDomain maps full state payload including nested overlay activity and sensor data")]
        public void ToDomain_MapsFullStatePayload_IncludingNestedOverlayActivityAndSensorData()
        {
            var timestamp = new DateTime(2026, 5, 27, 12, 0, 0, DateTimeKind.Utc);
            var dto = new TadoStateResponse
            {
                TadoMode = "HOME",
                GeolocationOverride = false,
                GeolocationOverrideDisableTime = timestamp,
                Preparation = "none",
                OverlayType = "MANUAL",
                OpenWindow = "closed",
                OpenWindowDetected = false,
                Setting = new TadoSettingResponse
                {
                    DeviceType = DeviceTypes.Heating,
                    Power = PowerStates.On,
                    Temperature = new TadoTemperatureResponse { Celsius = 21.0, Fahrenheit = 69.8 },
                    Mode = "MANUAL",
                    IsBoost = false
                },
                Overlay = new TadoOverlayResponse
                {
                    Setting = new TadoSettingResponse
                    {
                        DeviceType = DeviceTypes.Heating,
                        Power = PowerStates.On,
                        Temperature = new TadoTemperatureResponse { Celsius = 22.0 }
                    },
                    Termination = new TadoTerminationResponse
                    {
                        CurrentType = DurationModes.Timer,
                        DurationInSeconds = 900,
                        Expiry = timestamp.AddMinutes(15),
                        ProjectedExpiry = timestamp.AddMinutes(15)
                    }
                },
                Link = new TadoLinkResponse { State = "ONLINE" },
                ActivityDataPoints = new TadoActivityDataPointsResponse
                {
                    HeatingPower = new TadoHeatingPowerResponse
                    {
                        CurrentType = "PERCENTAGE",
                        Percentage = 34,
                        Timestamp = timestamp
                    }
                },
                SensorDataPoints = new TadoSensorDataPointsResponse
                {
                    InsideTemperature = new TadoInsideTemperatureResponse
                    {
                        Celsius = 21.3,
                        Fahrenheit = 70.3,
                        Timestamp = timestamp,
                        CurrentType = "TEMPERATURE",
                        Precision = new TadoPrecisionResponse
                        {
                            Celsius = 0.1,
                            Fahrenheit = 0.2
                        }
                    },
                    Humidity = new TadoHumidityResponse
                    {
                        CurrentType = "PERCENTAGE",
                        Percentage = 45,
                        Timestamp = timestamp
                    }
                }
            };

            var result = dto.ToDomain();

            Assert.Equal("HOME", result.TadoMode);
            Assert.False(result.GeolocationOverride);
            Assert.Equal("MANUAL", result.OverlayType);
            Assert.Equal(DeviceTypes.Heating, result.Setting?.DeviceType);
            Assert.Equal(21.0, result.Setting?.Temperature?.Celsius);
            Assert.Equal("ONLINE", result.Link?.State);
            Assert.Equal(34, result.ActivityDataPoints?.HeatingPower?.Percentage);
            Assert.Equal(21.3, result.SensorDataPoints?.InsideTemperature?.Celsius);
            Assert.Equal(0.1, result.SensorDataPoints?.InsideTemperature?.Precision?.Celsius);
            Assert.Equal(45, result.SensorDataPoints?.Humidity?.Percentage);
            Assert.Equal(900, result.Overlay?.Termination?.DurationInSeconds);
        }

        /// <summary>
        /// ToDomainList maps all items in the source list to domain entities.
        /// </summary>
        [Fact(DisplayName = "ToDomainList maps all items in source list")]
        public void ToDomainList_MapsAllItemsInSourceList()
        {
            var dtos = new List<TadoStateResponse>
            {
                new TadoStateResponse { TadoMode = "HOME" },
                new TadoStateResponse { TadoMode = "AWAY" }
            };

            var result = dtos.ToDomainList();

            Assert.Equal(2, result.Count);
            Assert.Equal("HOME", result[0].TadoMode);
            Assert.Equal("AWAY", result[1].TadoMode);
        }

        /// <summary>
        /// ToDomain throws ArgumentNullException when source DTO is null.
        /// </summary>
        [Fact(DisplayName = "ToDomain throws ArgumentNullException when source DTO is null")]
        public void ToDomain_ThrowsArgumentNullException_WhenSourceDtoIsNull()
        {
            TadoStateResponse dto = null!;

            Assert.Throws<ArgumentNullException>(() => StateMapper.ToDomain(dto));
        }
    }
}

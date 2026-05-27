using System;
using System.Collections.Generic;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Mappers;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Mappers;

/// <summary>
/// Unit tests for <see cref="WeatherMapper"/>.
/// </summary>
public class WeatherMapperTests
{
    /// <summary>
    /// ToDomain maps full weather payload including nested outside temperature and weather state.
    /// </summary>
    [Fact(DisplayName = "ToDomain maps full weather payload including nested values")]
    public void ToDomain_MapsFullWeatherPayload_IncludingNestedValues()
    {
        var timestamp = new DateTime(2026, 5, 27, 12, 0, 0, DateTimeKind.Utc);
        var dto = new TadoWeatherResponse
        {
            SolarIntensity = new TadoSolarIntensityResponse
            {
                CurrentType = "PERCENTAGE",
                Percentage = 70,
                Timestamp = timestamp
            },
            OutsideTemperature = new TadoOutsideTemperatureResponse
            {
                Celsius = 15.3,
                Fahrenheit = 59.5,
                Timestamp = timestamp,
                PurpleType = "TEMPERATURE",
                Precision = new TadoPrecisionResponse { Celsius = 0.1, Fahrenheit = 0.2 }
            },
            WeatherState = new TadoWeatherStateResponse
            {
                CurrentType = "SUNNY",
                Value = "CLEAR",
                Timestamp = timestamp
            }
        };

        var result = dto.ToDomain();

        Assert.Equal(70, result.SolarIntensity?.Percentage);
        Assert.Equal(15.3, result.OutsideTemperature?.Celsius);
        Assert.Equal("TEMPERATURE", result.OutsideTemperature?.PurpleType);
        Assert.Equal(0.1, result.OutsideTemperature?.Precision?.Celsius);
        Assert.Equal("SUNNY", result.WeatherState?.CurrentType);
        Assert.Equal("CLEAR", result.WeatherState?.Value);
    }

    /// <summary>
    /// ToDomain supports null nested payload values.
    /// </summary>
    [Fact(DisplayName = "ToDomain supports null nested payload values")]
    public void ToDomain_SupportsNullNestedPayloadValues()
    {
        var dto = new TadoWeatherResponse
        {
            SolarIntensity = null,
            OutsideTemperature = null,
            WeatherState = null
        };

        var result = dto.ToDomain();

        Assert.Null(result.SolarIntensity);
        Assert.Null(result.OutsideTemperature);
        Assert.Null(result.WeatherState);
    }

    /// <summary>
    /// ToDomain throws ArgumentNullException when weather DTO is null.
    /// </summary>
    [Fact(DisplayName = "ToDomain throws ArgumentNullException when weather DTO is null")]
    public void ToDomain_ThrowsArgumentNullException_WhenWeatherDtoIsNull()
    {
        TadoWeatherResponse dto = null!;

        Assert.Throws<ArgumentNullException>(() => WeatherMapper.ToDomain(dto));
    }

    /// <summary>
    /// ToDomainList maps all weather DTO entries.
    /// </summary>
    [Fact(DisplayName = "ToDomainList maps all weather DTO entries")]
    public void ToDomainList_MapsAllWeatherDtoEntries()
    {
        var dtos = new List<TadoWeatherResponse>
        {
            new() { WeatherState = new TadoWeatherStateResponse { CurrentType = "SUN" } },
            new() { WeatherState = new TadoWeatherStateResponse { CurrentType = "RAIN" } }
        };

        var result = dtos.ToDomainList();

        Assert.Equal(2, result.Count);
        Assert.Equal("SUN", result[0].WeatherState?.CurrentType);
        Assert.Equal("RAIN", result[1].WeatherState?.CurrentType);
    }
}
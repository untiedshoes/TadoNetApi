using System;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Entities.MobileDevice;
using Xunit;

namespace TadoNetApi.Tests.Domain.Entities;

/// <summary>
/// Smoke tests for low-coverage domain entities with property-only behavior.
/// </summary>
public class LowCoverageEntitiesSmokeTests
{
    /// <summary>
    /// Property-only entities retain values assigned through setters.
    /// </summary>
    [Fact(DisplayName = "Property-only domain entities retain assigned values")]
    public void PropertyOnlyDomainEntities_RetainAssignedValues()
    {
        var now = new DateTime(2026, 5, 27, 12, 0, 0, DateTimeKind.Utc);

        var characteristics = new Characteristics
        {
            Capabilities = ["HEATING", "COOLING"]
        };

        var connectionState = new ConnectionState
        {
            Value = true,
            Timestamp = now
        };

        var dazzleMode = new DazzleMode
        {
            Supported = true,
            Enabled = false
        };

        var mountingState = new MountingState
        {
            Value = "MOUNTED",
            Timestamp = now
        };

        var solarIntensity = new SolarIntensity
        {
            CurrentType = "PERCENTAGE",
            Percentage = 75,
            Timestamp = now
        };

        var humidity = new Humidity
        {
            CurrentType = "PERCENTAGE",
            Percentage = 48,
            Timestamp = now
        };

        var insideTemperature = new InsideTemperature
        {
            CurrentType = "TEMPERATURE",
            Celsius = 21.6,
            Fahrenheit = 70.9,
            Timestamp = now,
            Precision = new Precision
            {
                Celsius = 0.1,
                Fahrenheit = 0.2
            }
        };

        var openWindowDetection = new OpenWindowDetection
        {
            Enabled = true,
            TimeoutInSeconds = 900
        };

        var weatherState = new WeatherState
        {
            CurrentType = "SUNNY",
            Value = "CLEAR",
            Timestamp = now
        };

        var bearing = new BearingFromHome
        {
            Degrees = 180,
            Radians = Math.PI
        };

        var details = new Details
        {
            Platform = "iOS",
            OsVersion = "18.0",
            Model = "iPhone",
            Locale = "en-GB"
        };

        var location = new Location
        {
            Stale = false,
            AtHome = true,
            BearingFromHome = bearing,
            RelativeDistanceFromHomeFence = 0.25
        };

        Assert.Equal("HEATING", characteristics.Capabilities?[0]);
        Assert.True(connectionState.Value);
        Assert.Equal(now, connectionState.Timestamp);
        Assert.True(dazzleMode.Supported);
        Assert.False(dazzleMode.Enabled);
        Assert.Equal("MOUNTED", mountingState.Value);
        Assert.Equal(75, solarIntensity.Percentage);
        Assert.Equal(48, humidity.Percentage);
        Assert.Equal(21.6, insideTemperature.Celsius);
        Assert.True(openWindowDetection.Enabled);
        Assert.Equal("SUNNY", weatherState.CurrentType);
        Assert.Equal(180, bearing.Degrees);
        Assert.Equal("iOS", details.Platform);
        Assert.True(location.AtHome);
        Assert.Same(bearing, location.BearingFromHome);
    }
}
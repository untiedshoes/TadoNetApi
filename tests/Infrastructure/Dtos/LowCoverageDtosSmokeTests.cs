using TadoNetApi.Infrastructure.Auth.Dtos;
using TadoNetApi.Infrastructure.Dtos.Requests;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Dtos.Responses.MobileDevice;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Dtos;

/// <summary>
/// Smoke tests for low-coverage infrastructure DTOs and request models.
/// </summary>
public class LowCoverageDtosSmokeTests
{
    /// <summary>
    /// Property-only infrastructure DTOs retain values assigned through setters.
    /// </summary>
    [Fact(DisplayName = "Property-only infrastructure DTOs retain assigned values")]
    public void PropertyOnlyInfrastructureDtos_RetainAssignedValues()
    {
        var deviceAuthResult = new DeviceAuthorisationResult
        {
            VerificationUri = "https://example.com/verify",
            UserCode = "ABC-123",
            Interval = 5,
            DeviceCode = "device-code"
        };

        var childLockRequest = new SetChildLockRequest
        {
            ChildLock = true
        };

        var scheduleRequest = new TadoScheduleRequest
        {
            Name = "Morning",
            TargetTemperature = 21.5,
            StartTime = "06:00",
            EndTime = "08:30"
        };

        var mobileBearing = new TadoMobileBearingFromHomeResponse
        {
            Degrees = 180,
            Radians = 3.14159
        };

        var characteristicsResponse = new TadoCharacteristicsResponse
        {
            Capabilities = ["HEATING"]
        };

        var earlyStartResponse = new TadoEarlyStartResponse
        {
            Enabled = true
        };

        var connectionStateResponse = new TadoConnectionStateResponse
        {
            Value = true,
            Timestamp = new System.DateTime(2026, 5, 27, 12, 0, 0, System.DateTimeKind.Utc)
        };

        var dazzleModeResponse = new TadoDazzleModeResponse
        {
            Supported = true,
            Enabled = false
        };

        var mountingStateResponse = new TadoMountingStateResponse
        {
            Value = "MOUNTED",
            Timestamp = new System.DateTime(2026, 5, 27, 12, 0, 0, System.DateTimeKind.Utc)
        };

        var temperaturesResponse = new TadoTemperaturesResponse
        {
            Celsius = new TadoTemperatureStepsResponse { Min = 5, Max = 25, Step = 1 },
            Fahrenheit = new TadoTemperatureStepsResponse { Min = 41, Max = 77, Step = 1 }
        };

        var solarIntensityResponse = new TadoSolarIntensityResponse
        {
            CurrentType = "PERCENTAGE",
            Percentage = 72,
            Timestamp = new System.DateTime(2026, 5, 27, 12, 0, 0, System.DateTimeKind.Utc)
        };

        var weatherStateResponse = new TadoWeatherStateResponse
        {
            CurrentType = "SUNNY",
            Value = "CLEAR",
            Timestamp = new System.DateTime(2026, 5, 27, 12, 0, 0, System.DateTimeKind.Utc)
        };

        var authResponse = new TadoAuthResponse
        {
            AccessToken = "access",
            RefreshToken = "refresh",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        var openWindowDetectionResponse = new TadoOpenWindowDetectionResponse
        {
            Supported = true,
            Enabled = false,
            TimeoutInSeconds = 900
        };

        Assert.Equal("https://example.com/verify", deviceAuthResult.VerificationUri);
        Assert.Equal("ABC-123", deviceAuthResult.UserCode);
        Assert.Equal(5, deviceAuthResult.Interval);
        Assert.Equal("device-code", deviceAuthResult.DeviceCode);
        Assert.True(childLockRequest.ChildLock);
        Assert.Equal("Morning", scheduleRequest.Name);
        Assert.Equal(21.5, scheduleRequest.TargetTemperature);
        Assert.Equal("06:00", scheduleRequest.StartTime);
        Assert.Equal("08:30", scheduleRequest.EndTime);
        Assert.Equal(180, mobileBearing.Degrees);
        Assert.Equal(3.14159, mobileBearing.Radians);
        Assert.Equal("HEATING", characteristicsResponse.Capabilities?[0]);
        Assert.True(earlyStartResponse.Enabled);
        Assert.True(connectionStateResponse.Value);
        Assert.True(dazzleModeResponse.Supported);
        Assert.Equal("MOUNTED", mountingStateResponse.Value);
        Assert.Equal(5, temperaturesResponse.Celsius?.Min);
        Assert.Equal(72, solarIntensityResponse.Percentage);
        Assert.Equal("CLEAR", weatherStateResponse.Value);
        Assert.Equal("access", authResponse.AccessToken);
        Assert.Equal(3600, authResponse.ExpiresIn);
        Assert.True(openWindowDetectionResponse.Supported);
        Assert.Equal(900, openWindowDetectionResponse.TimeoutInSeconds);
    }
}
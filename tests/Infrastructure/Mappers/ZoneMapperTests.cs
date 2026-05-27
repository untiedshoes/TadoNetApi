using System;
using TadoNetApi.Infrastructure.Dtos.Responses;
using TadoNetApi.Infrastructure.Mappers;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Mappers;

/// <summary>
/// Unit tests for <see cref="ZoneMapper"/>.
/// </summary>
public class ZoneMapperTests
{
    /// <summary>
    /// ToDomain maps zone payload including dazzle and open window values.
    /// </summary>
    [Fact(DisplayName = "ToDomain maps zone payload including dazzle and open window values")]
    public void ToDomain_MapsZonePayload_IncludingDazzleAndOpenWindowValues()
    {
        var dto = new TadoZoneResponse
        {
            Id = 3,
            Name = "Living Room",
            CurrentType = "HEATING",
            SupportsDazzle = true,
            DazzleEnabled = true,
            DazzleMode = new TadoDazzleModeResponse { Supported = true, Enabled = true },
            OpenWindowDetection = new TadoOpenWindowDetectionResponse
            {
                Supported = true,
                Enabled = false,
                TimeoutInSeconds = 900
            }
        };

        var result = dto.ToDomain();

        Assert.Equal(3, result.Id);
        Assert.Equal("Living Room", result.Name);
        Assert.True(result.DazzleMode?.Supported);
        Assert.False(result.OpenWindowDetection?.Enabled);
        Assert.Equal(900, result.OpenWindowDetection?.TimeoutInSeconds);
    }

    /// <summary>
    /// ToDomain throws ArgumentNullException when zone DTO is null.
    /// </summary>
    [Fact(DisplayName = "ToDomain throws ArgumentNullException when zone DTO is null")]
    public void ToDomain_ThrowsArgumentNullException_WhenZoneDtoIsNull()
    {
        TadoZoneResponse dto = null!;

        Assert.Throws<ArgumentNullException>(() => ZoneMapper.ToDomain(dto));
    }

    /// <summary>
    /// Open window mapping defaults nullable booleans to false.
    /// </summary>
    [Fact(DisplayName = "Open window mapping defaults nullable booleans to false")]
    public void OpenWindowMapping_DefaultsNullableBooleans_ToFalse()
    {
        var dto = new TadoOpenWindowDetectionResponse
        {
            Supported = null,
            Enabled = null,
            TimeoutInSeconds = 600
        };

        var result = dto.ToDomain();

        Assert.False(result.Supported);
        Assert.False(result.Enabled);
        Assert.Equal(600, result.TimeoutInSeconds);
    }

    /// <summary>
    /// Zone control mapping maps nested duties and device collections.
    /// </summary>
    [Fact(DisplayName = "Zone control mapping maps nested duties and device collections")]
    public void ZoneControlMapping_MapsNestedDuties_AndDeviceCollections()
    {
        var dto = new TadoZoneControlResponse
        {
            Type = "HEATING",
            EarlyStartEnabled = true,
            HeatingCircuit = 1,
            Duties = new TadoZoneControlDutiesResponse
            {
                Type = "HEATING",
                Driver = new TadoDeviceResponse { SerialNo = "DRIVER" },
                Drivers = [new TadoDeviceResponse { SerialNo = "DRIVER2" }],
                Ui = new TadoDeviceResponse { SerialNo = "UI" },
                Uis = [new TadoDeviceResponse { SerialNo = "UI2" }]
            }
        };

        var result = dto.ToDomain();

        Assert.Equal("HEATING", result.Type);
        Assert.True(result.EarlyStartEnabled);
        Assert.Equal("DRIVER", result.Duties?.Driver?.SerialNo);
        Assert.Equal("DRIVER2", result.Duties?.Drivers?[0].SerialNo);
        Assert.Equal("UI", result.Duties?.Ui?.SerialNo);
        Assert.Equal("UI2", result.Duties?.Uis?[0].SerialNo);
    }

    /// <summary>
    /// Default zone overlay and away configuration mapping paths map termination and setting values.
    /// </summary>
    [Fact(DisplayName = "Default overlay and away configuration mapping maps nested values")]
    public void DefaultOverlayAndAwayConfigurationMapping_MapsNestedValues()
    {
        var defaultOverlayDto = new TadoDefaultZoneOverlayResponse
        {
            TerminationCondition = new TadoTerminationResponse
            {
                DurationInSeconds = 300
            }
        };

        var awayDto = new TadoAwayConfigurationResponse
        {
            Type = "HEATING",
            AutoAdjust = true,
            ComfortLevel = "BALANCE",
            Setting = new TadoSettingResponse
            {
                Mode = "MANUAL",
                IsBoost = true
            }
        };

        var earlyStartDto = new TadoEarlyStartResponse { Enabled = true };

        var defaultOverlay = defaultOverlayDto.ToDomain();
        var away = awayDto.ToDomain();
        var earlyStart = earlyStartDto.ToDomain();

        Assert.Equal(300, defaultOverlay.TerminationCondition?.DurationInSeconds);
        Assert.Equal("HEATING", away.Type);
        Assert.Equal("MANUAL", away.Setting?.Mode);
        Assert.True(earlyStart.Enabled);
    }
}
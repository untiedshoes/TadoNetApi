using System;
using System.Collections.Generic;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Enums;
using TadoNetApi.Infrastructure.Dtos.Requests;
using Xunit;

namespace TadoNetApi.Tests.Infrastructure.Dtos.Requests;

/// <summary>
/// Unit tests for overlay request mapping helpers.
/// </summary>
public class OverlayRequestMappingTests
{
    /// <summary>
    /// SetZoneOverlaysRequest.FromDomain maps overlay entries and duration mode aliases.
    /// </summary>
    [Fact(DisplayName = "SetZoneOverlaysRequest.FromDomain maps overlay entries and duration aliases")]
    public void SetZoneOverlaysRequest_FromDomain_MapsOverlayEntriesAndDurationAliases()
    {
        var overlay = new Overlay
        {
            Setting = new Setting
            {
                DeviceType = DeviceTypes.Heating,
                Power = PowerStates.On,
                Temperature = new Temperature { Celsius = 21.5 },
                Mode = "MANUAL",
                IsBoost = true
            },
            Termination = new Termination
            {
                Type = "NEXT_TIME_BLOCK",
                DurationInSeconds = 600
            }
        };

        var result = SetZoneOverlaysRequest.FromDomain(new Dictionary<int, Overlay>
        {
            [3] = overlay
        });

        var entry = Assert.Single(result.Overlays);
        Assert.Equal(3, entry.Room);
        Assert.Equal(DurationModes.UntilNextTimedEvent, entry.Overlay.Termination.CurrentType);
        Assert.Equal(600, entry.Overlay.Termination.DurationInSeconds);
        Assert.Equal(21.5, entry.Overlay.Setting.Temperature?.Celsius);
    }

    /// <summary>
    /// SetZoneOverlayRequest.FromDomain handles manual/tado_mode and unknown termination values.
    /// </summary>
    [Fact(DisplayName = "SetZoneOverlayRequest.FromDomain maps manual tado_mode and unknown terminations")]
    public void SetZoneOverlayRequest_FromDomain_MapsManualTadoModeAndUnknownTerminations()
    {
        var manual = SetZoneOverlayRequest.FromDomain(new Overlay
        {
            Termination = new Termination { Type = "MANUAL" }
        });

        var tadoMode = SetZoneOverlayRequest.FromDomain(new Overlay
        {
            Termination = new Termination { Type = "TADO_MODE" }
        });

        var unknown = SetZoneOverlayRequest.FromDomain(new Overlay
        {
            Termination = new Termination { Type = "UNSUPPORTED" }
        });

        Assert.Equal(DurationModes.UntilNextManualChange, manual.Termination.CurrentType);
        Assert.Equal(DurationModes.UntilNextTimedEvent, tadoMode.Termination.CurrentType);
        Assert.Null(unknown.Termination.CurrentType);
    }

    /// <summary>
    /// SetZoneOverlayRequest.FromDomain maps additional duration aliases and null values.
    /// </summary>
    [Fact(DisplayName = "SetZoneOverlayRequest.FromDomain maps timer until-next-manual and null durations")]
    public void SetZoneOverlayRequest_FromDomain_MapsTimerUntilNextManualAndNullDurations()
    {
        var timer = SetZoneOverlayRequest.FromDomain(new Overlay
        {
            Termination = new Termination { Type = "TIMER", DurationInSeconds = 300 }
        });

        var untilNextManual = SetZoneOverlayRequest.FromDomain(new Overlay
        {
            Termination = new Termination { Type = "UNTILNEXTMANUALCHANGE" }
        });

        var nullType = SetZoneOverlayRequest.FromDomain(new Overlay
        {
            Termination = new Termination { Type = null }
        });

        Assert.Equal(DurationModes.Timer, timer.Termination.CurrentType);
        Assert.Equal(300, timer.Termination.DurationInSeconds);
        Assert.Equal(DurationModes.UntilNextManualChange, untilNextManual.Termination.CurrentType);
        Assert.Null(nullType.Termination.CurrentType);
    }

    /// <summary>
    /// SetDefaultZoneOverlayRequest.FromDomain maps known duration modes and null values.
    /// </summary>
    [Fact(DisplayName = "SetDefaultZoneOverlayRequest.FromDomain maps known duration modes and null values")]
    public void SetDefaultZoneOverlayRequest_FromDomain_MapsKnownDurationModesAndNullValues()
    {
        var manual = SetDefaultZoneOverlayRequest.FromDomain(new DefaultZoneOverlay
        {
            TerminationCondition = new Termination { Type = nameof(DurationModes.UntilNextManualChange), DurationInSeconds = 120 }
        });

        var timedEvent = SetDefaultZoneOverlayRequest.FromDomain(new DefaultZoneOverlay
        {
            TerminationCondition = new Termination { Type = nameof(DurationModes.UntilNextTimedEvent), DurationInSeconds = 240 }
        });

        var timer = SetDefaultZoneOverlayRequest.FromDomain(new DefaultZoneOverlay
        {
            TerminationCondition = new Termination { Type = nameof(DurationModes.Timer), DurationInSeconds = 300 }
        });

        var custom = SetDefaultZoneOverlayRequest.FromDomain(new DefaultZoneOverlay
        {
            TerminationCondition = new Termination { Type = "manual_custom" }
        });

        var fallback = SetDefaultZoneOverlayRequest.FromDomain(new DefaultZoneOverlay
        {
            TerminationCondition = new Termination { Type = null }
        });

        Assert.Equal("MANUAL", manual.TerminationCondition.Type);
        Assert.Equal(120, manual.TerminationCondition.DurationInSeconds);
        Assert.Equal("TADO_MODE", timedEvent.TerminationCondition.Type);
        Assert.Equal(240, timedEvent.TerminationCondition.DurationInSeconds);
        Assert.Equal("TIMER", timer.TerminationCondition.Type);
        Assert.Equal("MANUAL_CUSTOM", custom.TerminationCondition.Type);
        Assert.Equal(string.Empty, fallback.TerminationCondition.Type);
    }

    /// <summary>
    /// Mapping helpers throw ArgumentNullException when source payload is null.
    /// </summary>
    [Fact(DisplayName = "Overlay request mapping helpers throw ArgumentNullException when source is null")]
    public void OverlayRequestMappingHelpers_ThrowArgumentNullException_WhenSourceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => SetZoneOverlaysRequest.FromDomain(null!));
        Assert.Throws<ArgumentNullException>(() => SetZoneOverlayRequest.FromDomain(null!));
        Assert.Throws<ArgumentNullException>(() => SetDefaultZoneOverlayRequest.FromDomain(null!));
    }
}
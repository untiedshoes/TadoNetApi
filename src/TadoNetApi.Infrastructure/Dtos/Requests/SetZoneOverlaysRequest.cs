using System.Text.Json.Serialization;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Enums;

namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Request payload for applying manual overlays to multiple zones in one command.
/// </summary>
public sealed class SetZoneOverlaysRequest
{
    /// <summary>
    /// Gets or sets the overlay entries keyed by room.
    /// </summary>
    [JsonPropertyName("overlays")]
    public IReadOnlyList<SetZoneOverlayEntryRequest> Overlays { get; set; } = [];

    /// <summary>
    /// Maps a set of domain overlays into the API request shape.
    /// </summary>
    public static SetZoneOverlaysRequest FromDomain(IReadOnlyDictionary<int, Overlay> zoneOverlays)
    {
        ArgumentNullException.ThrowIfNull(zoneOverlays);

        return new SetZoneOverlaysRequest
        {
            Overlays = zoneOverlays
                .Select(zoneOverlay => new SetZoneOverlayEntryRequest
                {
                    Room = zoneOverlay.Key,
                    Overlay = SetZoneOverlayRequest.FromDomain(zoneOverlay.Value)
                })
                .ToArray()
        };
    }
}

/// <summary>
/// A single room overlay entry for the bulk overlay command.
/// </summary>
public sealed class SetZoneOverlayEntryRequest
{
    /// <summary>
    /// Gets or sets the room identifier.
    /// </summary>
    [JsonPropertyName("room")]
    public int Room { get; set; }

    /// <summary>
    /// Gets or sets the overlay payload to apply to the room.
    /// </summary>
    [JsonPropertyName("overlay")]
    public SetZoneOverlayRequest Overlay { get; set; } = new();
}

/// <summary>
/// Overlay payload for the bulk overlay command.
/// </summary>
public sealed class SetZoneOverlayRequest
{
    /// <summary>
    /// Gets or sets the desired zone setting.
    /// </summary>
    [JsonPropertyName("setting")]
    public SetZoneTemperatureSettingRequest Setting { get; set; } = new();

    /// <summary>
    /// Gets or sets the overlay termination details.
    /// </summary>
    [JsonPropertyName("termination")]
    public SetZoneTemperatureTerminationRequest Termination { get; set; } = new();

    /// <summary>
    /// Maps a domain <see cref="Overlay"/> into the API request shape.
    /// </summary>
    public static SetZoneOverlayRequest FromDomain(Overlay overlay)
    {
        ArgumentNullException.ThrowIfNull(overlay);

        return new SetZoneOverlayRequest
        {
            Setting = new SetZoneTemperatureSettingRequest
            {
                DeviceType = overlay.Setting?.DeviceType,
                Power = overlay.Setting?.Power,
                Temperature = overlay.Setting?.Temperature == null
                    ? null
                    : new SetZoneTemperatureValueRequest
                    {
                        Celsius = overlay.Setting.Temperature.Celsius,
                        Fahrenheit = overlay.Setting.Temperature.Fahrenheit
                    },
                Mode = overlay.Setting?.Mode,
                IsBoost = overlay.Setting?.IsBoost
            },
            Termination = new SetZoneTemperatureTerminationRequest
            {
                CurrentType = ToDurationMode(overlay.Termination?.Type),
                DurationInSeconds = overlay.Termination?.DurationInSeconds
            }
        };
    }

    private static DurationModes? ToDurationMode(string? type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return null;

        return type.ToUpperInvariant() switch
        {
            "UNTILNEXTMANUALCHANGE" => DurationModes.UntilNextManualChange,
            "UNTILNEXTTIMEDEVENT" => DurationModes.UntilNextTimedEvent,
            "TIMER" => DurationModes.Timer,
            "MANUAL" => DurationModes.UntilNextManualChange,
            "TADO_MODE" => DurationModes.UntilNextTimedEvent,
            "NEXT_TIME_BLOCK" => DurationModes.UntilNextTimedEvent,
            _ => null
        };
    }
}
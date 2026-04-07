using System.Text.Json.Serialization;
using TadoNetApi.Domain.Entities;
using TadoNetApi.Domain.Enums;

namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Request payload for updating the default overlay configuration of a zone.
/// </summary>
public sealed class SetDefaultZoneOverlayRequest
{
    /// <summary>
    /// Gets or sets the default termination condition.
    /// </summary>
    [JsonPropertyName("terminationCondition")]
    public SetDefaultZoneOverlayTerminationRequest TerminationCondition { get; set; } = new();

    /// <summary>
    /// Maps a domain <see cref="DefaultZoneOverlay"/> into the API request shape.
    /// </summary>
    public static SetDefaultZoneOverlayRequest FromDomain(DefaultZoneOverlay defaultOverlay)
    {
        ArgumentNullException.ThrowIfNull(defaultOverlay);

        return new SetDefaultZoneOverlayRequest
        {
            TerminationCondition = new SetDefaultZoneOverlayTerminationRequest
            {
                Type = ToApiTerminationType(defaultOverlay.TerminationCondition?.Type),
                DurationInSeconds = defaultOverlay.TerminationCondition?.DurationInSeconds
            }
        };
    }

    private static string ToApiTerminationType(string? type)
        => type switch
        {
            nameof(DurationModes.UntilNextManualChange) => "MANUAL",
            nameof(DurationModes.UntilNextTimedEvent) => "TADO_MODE",
            nameof(DurationModes.Timer) => "TIMER",
            _ => type?.ToUpperInvariant() ?? string.Empty
        };
}

/// <summary>
/// Termination condition payload for updating the default zone overlay.
/// </summary>
public sealed class SetDefaultZoneOverlayTerminationRequest
{
    /// <summary>
    /// Gets or sets the termination type.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional timer duration in seconds.
    /// </summary>
    [JsonPropertyName("durationInSeconds")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? DurationInSeconds { get; set; }
}
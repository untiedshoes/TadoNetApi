using System.Text.Json.Serialization;
using TadoNetApi.Domain.Entities;

namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Request payload for updating zone open window detection settings.
/// </summary>
public sealed class SetOpenWindowDetectionRequest
{
    /// <summary>
    /// Gets or sets the target room identifier.
    /// </summary>
    [JsonPropertyName("roomId")]
    public int RoomId { get; set; }

    /// <summary>
    /// Gets or sets whether open window detection is enabled.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets or sets the timeout in seconds.
    /// </summary>
    [JsonPropertyName("timeoutInSeconds")]
    public long TimeoutInSeconds { get; set; }

    /// <summary>
    /// Maps the domain model to the request payload expected by the API.
    /// </summary>
    /// <param name="zoneId">The zone identifier, sent as roomId in the request body.</param>
    /// <param name="settings">The open window detection settings to map.</param>
    /// <returns>The mapped request payload.</returns>
    public static SetOpenWindowDetectionRequest FromDomain(int zoneId, OpenWindowDetection settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return new SetOpenWindowDetectionRequest
        {
            RoomId = zoneId,
            Enabled = settings.Enabled ?? false,
            TimeoutInSeconds = settings.TimeoutInSeconds ?? 0
        };
    }
}
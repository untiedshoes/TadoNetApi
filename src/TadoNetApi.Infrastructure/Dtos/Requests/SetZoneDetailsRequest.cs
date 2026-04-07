using System.Text.Json.Serialization;
using TadoNetApi.Domain.Entities;

namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Request payload for updating zone details.
/// </summary>
public sealed class SetZoneDetailsRequest
{
    /// <summary>
    /// Gets or sets the zone name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Maps a domain <see cref="Zone"/> into the writable zone-details request shape.
    /// </summary>
    public static SetZoneDetailsRequest FromDomain(Zone zone)
    {
        ArgumentNullException.ThrowIfNull(zone);

        return new SetZoneDetailsRequest
        {
            Name = zone.Name ?? string.Empty
        };
    }
}
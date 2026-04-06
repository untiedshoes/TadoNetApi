using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Request payload for updating a home's geo-tracking away radius.
/// </summary>
public class SetAwayRadiusInMetersRequest
{
    /// <summary>
    /// Distance in meters at which a mobile device is treated as away from home.
    /// </summary>
    [JsonPropertyName("awayRadiusInMeters")]
    public double AwayRadiusInMeters { get; set; }
}
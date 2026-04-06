using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Request payload for moving a device into an existing zone.
/// </summary>
public class MoveDeviceToZoneRequest
{
    /// <summary>
    /// The serial number of the device to move.
    /// </summary>
    [JsonPropertyName("serialNo")]
    public string SerialNo { get; set; } = string.Empty;
}
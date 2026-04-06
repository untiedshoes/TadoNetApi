using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Request payload for updating the measuring device of a zone.
/// </summary>
public class SetZoneMeasuringDeviceRequest
{
    /// <summary>
    /// The serial number of the device that should measure the zone temperature.
    /// </summary>
    [JsonPropertyName("serialNo")]
    public string SerialNo { get; set; } = string.Empty;
}
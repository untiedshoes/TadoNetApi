using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

/// <summary>
/// Represents a heating circuit response from the API.
/// </summary>
public class TadoHeatingCircuitResponse
{
    /// <summary>
    /// The heating circuit number.
    /// </summary>
    [JsonPropertyName("number")]
    public int? Number { get; set; }

    /// <summary>
    /// The full serial number of the controlling device.
    /// </summary>
    [JsonPropertyName("driverSerialNo")]
    public string? DriverSerialNo { get; set; }

    /// <summary>
    /// The short serial number of the controlling device.
    /// </summary>
    [JsonPropertyName("driverShortSerialNo")]
    public string? DriverShortSerialNo { get; set; }
}
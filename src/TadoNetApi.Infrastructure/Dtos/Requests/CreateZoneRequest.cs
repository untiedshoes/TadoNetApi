using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Request DTO for creating a new zone and moving devices into it.
/// </summary>
public class CreateZoneRequest
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "IMPLICIT_CONTROL";

    [JsonPropertyName("zoneType")]
    public string ZoneType { get; set; } = string.Empty;

    [JsonPropertyName("devices")]
    public IReadOnlyList<CreateZoneDeviceRequest> Devices { get; set; } = [];
}

public class CreateZoneDeviceRequest
{
    [JsonPropertyName("serialNo")]
    public string SerialNo { get; set; } = string.Empty;
}
using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

public class TadoDeviceResponse
{
    [JsonPropertyName("serialNo")]
    public string SerialNo { get; set; } = string.Empty;

    [JsonPropertyName("shortSerialNo")]
    public string ShortSerialNo { get; set; } = string.Empty;

    [JsonPropertyName("deviceType")]
    public string DeviceType { get; set; } = string.Empty;

    [JsonPropertyName("currentFwVersion")]
    public string CurrentFwVersion { get; set; } = string.Empty;
}
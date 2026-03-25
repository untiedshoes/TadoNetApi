using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses;

public class TadoDeviceResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("homeId")]
    public int HomeId { get; set; }

    [JsonPropertyName("serialNo")]
    public string? SerialNo { get; set; }

    [JsonPropertyName("deviceType")]
    public string? DeviceType { get; set; } 

    [JsonPropertyName("childLock")]
    public bool? ChildLock { get; set; }
}
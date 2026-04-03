using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Response DTO for GET homes/{homeId}/deviceList.
    /// </summary>
    public class TadoDeviceListResponse
    {
        [JsonPropertyName("entries")]
        public TadoDeviceListItemResponse[]? Entries { get; set; }
    }

    /// <summary>
    /// One entry in the device list response.
    /// </summary>
    public class TadoDeviceListItemResponse
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("device")]
        public TadoDeviceResponse? Device { get; set; }

        [JsonPropertyName("zone")]
        public TadoDeviceListZoneResponse? Zone { get; set; }
    }

    /// <summary>
    /// Zone metadata attached to a device list entry.
    /// </summary>
    public class TadoDeviceListZoneResponse
    {
        [JsonPropertyName("discriminator")]
        public long? Discriminator { get; set; }

        [JsonPropertyName("duties")]
        public string[]? Duties { get; set; }
    }
}
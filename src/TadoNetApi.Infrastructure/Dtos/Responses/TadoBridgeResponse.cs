using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Represents a bridge payload returned by the public bridge endpoint.
    /// </summary>
    public class TadoBridgeResponse
    {
        [JsonPropertyName("partner")]
        public object? Partner { get; set; }

        [JsonPropertyName("homeId")]
        public int? HomeId { get; set; }
    }
}
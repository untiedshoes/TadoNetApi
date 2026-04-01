using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// State of the connection towards a Tado device
    /// </summary>
    public partial class TadoConnectionStateResponse
    {
        [JsonPropertyName("value")]
        public bool Value { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
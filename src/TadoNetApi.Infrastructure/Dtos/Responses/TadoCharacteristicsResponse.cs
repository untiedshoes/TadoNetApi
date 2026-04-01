using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Characteristics of a device
    /// </summary>
    public class TadoCharacteristicsResponse
    {
        /// <summary>
        /// The list of capabilities supported by the device
        /// </summary>
        [JsonPropertyName("capabilities")]
        public string[]? Capabilities { get; set; }
    }
}
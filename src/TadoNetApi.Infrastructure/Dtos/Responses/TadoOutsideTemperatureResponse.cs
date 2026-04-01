using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Represents the outside temperature measured by a Tado device
    /// </summary>
    public partial class TadoOutsideTemperatureResponse
    {
        /// <summary>
        /// The temperature in Celsius
        /// </summary>
        [JsonPropertyName("celsius")]
        public double? Celsius { get; set; }

        /// <summary>
        /// The temperature in Fahrenheit
        /// </summary>
        [JsonPropertyName("fahrenheit")]
        public double? Fahrenheit { get; set; }

        /// <summary>
        /// The timestamp when the temperature was recorded
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// The type of temperature reading
        /// </summary>
        [JsonPropertyName("type")]
        public string? PurpleType { get; set; }

        /// <summary>
        /// The precision of the temperature reading
        /// </summary>
        [JsonPropertyName("precision")]
        public TadoPrecisionResponse? Precision { get; set; }
    }
}
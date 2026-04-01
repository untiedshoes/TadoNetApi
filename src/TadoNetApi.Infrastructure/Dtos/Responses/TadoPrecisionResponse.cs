using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Represents the precision of a temperature reading
    /// </summary>
    public partial class TadoPrecisionResponse
    {
        /// <summary>
        /// The precision in Celsius
        /// </summary>
        [JsonPropertyName("celsius")]
        public double? Celsius { get; set; }

        /// <summary>
        /// The precision in Fahrenheit
        /// </summary>
        [JsonPropertyName("fahrenheit")]
        public double? Fahrenheit { get; set; }
    }
}
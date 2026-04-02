using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Represents the minimum, maximum, and step values for temperature settings
    /// </summary>
    public class TadoTemperatureStepsResponse
    {
        /// <summary>
        /// The minimum temperature value
        /// </summary>
        [JsonPropertyName("min")]
        public double? Min { get; set; }

        /// <summary>
        /// The maximum temperature value
        /// </summary>
        [JsonPropertyName("max")]
        public double? Max { get; set; }

        /// <summary>
        /// The step increment for temperature adjustments
        /// </summary>
        [JsonPropertyName("step")]
        public double? Step { get; set; }
    }
}
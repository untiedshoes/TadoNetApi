using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Represents temperature step settings in both Celsius and Fahrenheit
    /// </summary>
    public partial class TadoTemperaturesResponse
    {
        /// <summary>
        /// Temperature step settings in Celsius
        /// </summary>
        [JsonPropertyName("celsius")]
        public TadoTemperatureStepsResponse? Celsius { get; set; }

        /// <summary>
        /// Temperature step settings in Fahrenheit
        /// </summary>
        [JsonPropertyName("fahrenheit")]
        public TadoTemperatureStepsResponse? Fahrenheit { get; set; }
    }
}
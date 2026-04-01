using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// The current weather
    /// </summary>
    public partial class TadoWeatherResponse
    {
        /// <summary>
        /// The current solar intensity
        /// </summary>
        [JsonPropertyName("solarIntensity")]
        public TadoSolarIntensityResponse? SolarIntensity { get; set; }

        /// <summary>
        /// The current outside temperature
        /// </summary>
        [JsonPropertyName("outsideTemperature")]
        public TadoOutsideTemperatureResponse? OutsideTemperature { get; set; }

        /// <summary>
        /// The current weather state (e.g., SUNNY, CLOUDY)
        /// </summary>
        [JsonPropertyName("weatherState")]
        public TadoWeatherStateResponse? WeatherState { get; set; }
    }
}
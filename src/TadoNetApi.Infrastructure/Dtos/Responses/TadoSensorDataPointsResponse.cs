using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Temperature and humidity measured by a Tado device
    /// </summary>
    public partial class TadoSensorDataPointsResponse
    {
        /// <summary>
        /// The inside temperature data point
        /// </summary>
        [JsonPropertyName("insideTemperature")]
        public TadoInsideTemperatureResponse? InsideTemperature { get; set; }

        /// <summary>
        /// The humidity data point
        /// </summary>
        [JsonPropertyName("humidity")]
        public TadoHumidityResponse? Humidity { get; set; }
    }
}
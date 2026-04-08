using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Represents the maximum output temperature returned by bridge-scoped boiler endpoints.
    /// </summary>
    public class TadoBoilerMaxOutputTemperatureResponse
    {
        [JsonPropertyName("boilerMaxOutputTemperatureInCelsius")]
        public double? BoilerMaxOutputTemperatureInCelsius { get; set; }
    }
}
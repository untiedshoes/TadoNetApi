using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Requests
{
    /// <summary>
    /// Represents the request payload for updating a boiler's maximum output temperature via bridge-scoped endpoints.
    /// </summary>
    public class SetBoilerMaxOutputTemperatureRequest
    {
        [JsonPropertyName("boilerMaxOutputTemperatureInCelsius")]
        public double BoilerMaxOutputTemperatureInCelsius { get; set; }
    }
}
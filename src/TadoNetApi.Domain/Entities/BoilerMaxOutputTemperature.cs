namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Represents the maximum output temperature configured for a boiler.
    /// </summary>
    public class BoilerMaxOutputTemperature
    {
        /// <summary>
        /// Gets or sets the configured maximum output temperature in Celsius.
        /// </summary>
        public double? BoilerMaxOutputTemperatureInCelsius { get; set; }
    }
}
namespace TadoNetApi.Domain.Entities
{
    public partial class Temperatures
    {
        /// <summary>
        /// Temperature step settings in Celsius
        /// </summary>
        public TemperatureSteps? Celsius { get; set; }

        /// <summary>
        /// Temperature step settings in Fahrenheit
        /// </summary>
        public TemperatureSteps? Fahrenheit { get; set; }
    }
}   
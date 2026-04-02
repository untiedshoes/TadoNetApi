namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Represents the minimum, maximum, and step values for temperature settings
    /// </summary>
    public class TemperatureSteps
    {
        /// <summary>
        /// The minimum temperature value
        /// </summary>
        public double? Min { get; set; }

        /// <summary>
        /// The maximum temperature value
        /// </summary>
        public double? Max { get; set; }

        /// <summary>
        /// The step increment for temperature adjustments
        /// </summary>
        public double? Step { get; set; }
    }
}
namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Represents the current weather state
    /// </summary>
    public partial class WeatherState
    {
        /// <summary>
        /// The type of weather state (e.g., SUNNY, CLOUDY)
        /// </summary>
        public string? CurrentType { get; set; }

        /// <summary>
        /// The value describing the weather condition
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// The timestamp when the weather state was recorded
        /// </summary>
        public DateTime? Timestamp { get; set; }
    }
}
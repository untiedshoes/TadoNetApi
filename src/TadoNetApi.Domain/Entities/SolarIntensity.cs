namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Represents the solar intensity measured by a Tado device
    /// </summary>
    public partial class SolarIntensity
    {
        /// <summary>
        /// The type of solar intensity measurement
        /// </summary>
        public string? CurrentType { get; set; }

        /// <summary>
        /// The percentage of solar intensity
        /// </summary>
        public double? Percentage { get; set; }

        /// <summary>
        /// The timestamp when the solar intensity was recorded
        /// </summary>
        public DateTime? Timestamp { get; set; }
    }
}
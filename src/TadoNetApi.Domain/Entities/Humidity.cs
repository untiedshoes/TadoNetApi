namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Humidity measured by a Tado device
    /// </summary>
    public partial class Humidity
    {
        /// <summary>
        /// The type of humidity measurement (e.g., PERCENTAGE)
        /// </summary>
        public string? CurrentType { get; set; }

        /// <summary>
        /// The humidity percentage
        /// </summary>
        public double? Percentage { get; set; }

        /// <summary>
        /// The timestamp when the humidity was measured
        /// </summary>
        public DateTime? Timestamp { get; set; }
    }
}
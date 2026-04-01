namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Represents the heating power data point of a device
    /// </summary>
    public class HeatingPower
    {
        /// <summary>
        /// The type of heating power (e.g., percentage)
        /// </summary>
        public string? CurrentType { get; set; }

        /// <summary>
        /// The percentage of heating power being used
        /// </summary>
        public double? Percentage { get; set; }

        /// <summary>
        /// The timestamp when the heating power was recorded
        /// </summary>
        public DateTime? Timestamp { get; set; }
    }
}
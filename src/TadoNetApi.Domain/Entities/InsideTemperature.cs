namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Represents the inside temperature measured by a Tado device
    /// </summary>
    public partial class InsideTemperature
    {
        /// <summary>
        /// The temperature in Celsius
        /// </summary>
        public double? Celsius { get; set; }

        /// <summary>
        /// The temperature in Fahrenheit
        /// </summary>
        public double? Fahrenheit { get; set; }

        /// <summary>
        /// The timestamp when the temperature was recorded
        /// </summary>
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// The type of temperature reading
        /// </summary>
        public string? CurrentType { get; set; }

        /// <summary>
        /// The precision of the temperature reading
        /// </summary>
        public Precision? Precision { get; set; }
    }
}
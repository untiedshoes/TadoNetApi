namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Represents the precision of a temperature reading
    /// </summary>
    public partial class Precision
    {
        /// <summary>
        /// The precision in Celsius
        /// </summary>
        public double? Celsius { get; set; }

        /// <summary>
        /// The precision in Fahrenheit
        /// </summary>
        public double? Fahrenheit { get; set; }
    }
}
namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Temperature and humidity measured by a Tado device
    /// </summary>
    public partial class SensorDataPoints
    {
        /// <summary>
        /// The inside temperature data point
        /// </summary>
        public InsideTemperature? InsideTemperature { get; set; }

        /// <summary>
        /// The humidity data point
        /// </summary>
        public Humidity? Humidity { get; set; }
    }
}
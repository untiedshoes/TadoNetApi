namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Contains activity data points such as heating power
    /// </summary>
    public class ActivityDataPoints
    {
        /// <summary>
        /// The heating power data point
        /// </summary>
        public HeatingPower? HeatingPower { get; set; }
    }
}
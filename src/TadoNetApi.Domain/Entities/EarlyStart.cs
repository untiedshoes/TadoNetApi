namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Indicates whether the Early Start feature is enabled
    /// </summary>
    public class EarlyStart
    {
        /// <summary>
        /// Whether Early Start is enabled for the schedule
        /// </summary>
        public bool? Enabled { get; set; }
    }
}
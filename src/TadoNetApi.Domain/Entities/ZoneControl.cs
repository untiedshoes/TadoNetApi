namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Represents the control configuration for a zone.
    /// </summary>
    public class ZoneControl
    {
        /// <summary>
        /// The zone type, for example HEATING or HOT_WATER.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Indicates whether early start is enabled for the zone.
        /// </summary>
        public bool? EarlyStartEnabled { get; set; }

        /// <summary>
        /// The heating circuit number associated with the zone.
        /// </summary>
        public int? HeatingCircuit { get; set; }

        /// <summary>
        /// The devices in the zone grouped by duty.
        /// </summary>
        public ZoneControlDuties? Duties { get; set; }
    }
}
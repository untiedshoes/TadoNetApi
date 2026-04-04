namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Groups zone devices by their duty.
    /// </summary>
    public class ZoneControlDuties
    {
        /// <summary>
        /// The zone type associated with this duty grouping.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// The single driver device, when present.
        /// </summary>
        public Device? Driver { get; set; }

        /// <summary>
        /// The driver devices for the zone.
        /// </summary>
        public Device[]? Drivers { get; set; }

        /// <summary>
        /// The single leader device, when present.
        /// </summary>
        public Device? Leader { get; set; }

        /// <summary>
        /// The leader devices for the zone.
        /// </summary>
        public Device[]? Leaders { get; set; }

        /// <summary>
        /// The single UI device, when present.
        /// </summary>
        public Device? Ui { get; set; }

        /// <summary>
        /// The UI devices for the zone.
        /// </summary>
        public Device[]? Uis { get; set; }
    }
}
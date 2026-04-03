namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Represents one entry from the home device list endpoint, including optional zone association.
    /// </summary>
    public class DeviceListEntry
    {
        /// <summary>
        /// The device type code for the entry.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// The device details for this entry.
        /// </summary>
        public Device? Device { get; set; }

        /// <summary>
        /// The associated zone ID when available.
        /// </summary>
        public long? ZoneId { get; set; }

        /// <summary>
        /// Device duties within the associated zone when available.
        /// </summary>
        public string[]? ZoneDuties { get; set; }
    }
}
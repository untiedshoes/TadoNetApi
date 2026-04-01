namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Information about one zone
    /// </summary>
    public class Zone
    {
        /// <summary>
        /// The unique identifier of the zone
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// The name of the zone
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The current type of the zone (e.g., HEATING, HOT_WATER)
        /// </summary>
        public string? CurrentType { get; set; }

        /// <summary>
        /// The date and time when the zone was created
        /// </summary>
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// The types of devices associated with the zone
        /// </summary>
        public string[]? DeviceTypes { get; set; }

        /// <summary>
        /// The list of devices in the zone
        /// </summary>
        public Device[]? Devices { get; set; }

        /// <summary>
        /// Indicates whether a report is available for the zone
        /// </summary>
        public bool? ReportAvailable { get; set; }

        /// <summary>
        /// Indicates whether the zone supports the Dazzle feature
        /// </summary>
        public bool? SupportsDazzle { get; set; }

        /// <summary>
        /// Indicates whether the Dazzle feature is enabled
        /// </summary>
        public bool? DazzleEnabled { get; set; }

        /// <summary>
        /// The current Dazzle mode configuration
        /// </summary>
        public DazzleMode? DazzleMode { get; set; }

        /// <summary>
        /// The open window detection settings for the zone
        /// </summary>
        public OpenWindowDetection? OpenWindowDetection { get; set; }
    }
}
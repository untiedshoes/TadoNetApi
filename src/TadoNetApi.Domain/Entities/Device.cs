namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Information about one Tado device
    /// </summary>
    public class Device
    {
        /// <summary>
        /// The type of the device (e.g., SMART_THERMOSTAT)
        /// </summary>
        public string? DeviceType { get; set; }

        /// <summary>
        /// The friendly name for the hardware device type when known.
        /// </summary>
        public string? DeviceTypeName => DeviceTypeCatalogue.GetFriendlyName(DeviceType);

        /// <summary>
        /// The full serial number of the device
        /// </summary>
        public string? SerialNo { get; set; }

        /// <summary>
        /// The short version of the device's serial number
        /// </summary>
        public string? ShortSerialNo { get; set; }

        /// <summary>
        /// The current firmware version installed on the device
        /// </summary>
        public string? CurrentFwVersion { get; set; }

        /// <summary>
        /// The current connection state of the device
        /// </summary>
        public ConnectionState? ConnectionState { get; set; }

        /// <summary>
        /// The characteristics of the device
        /// </summary>
        public Characteristics? Characteristics { get; set; }

        /// <summary>
        /// The list of duties assigned to the device
        /// </summary>
        public string[]? Duties { get; set; }

        /// <summary>
        /// The mounting state of the device
        /// </summary>
        public MountingState? MountingState { get; set; }

        /// <summary>
        /// The battery state of the device (e.g., NORMAL, LOW)
        /// </summary>
        public string? BatteryState { get; set; }

        /// <summary>
        /// Indicates if child lock is enabled or disabled on the Tado device
        /// </summary>
        public bool? ChildLockEnabled { get; set; }
    }
}
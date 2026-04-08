namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Represents the wiring and bridge connectivity state for a boiler installation.
    /// </summary>
    public class BoilerWiringInstallationState
    {
        /// <summary>
        /// Gets or sets the overall installation state.
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// Gets or sets the device currently wired to the boiler.
        /// </summary>
        public BoilerWiredDevice? DeviceWiredToBoiler { get; set; }

        /// <summary>
        /// Gets or sets whether the bridge is currently connected.
        /// </summary>
        public bool? BridgeConnected { get; set; }

        /// <summary>
        /// Gets or sets whether a hot-water zone is present.
        /// </summary>
        public bool? HotWaterZonePresent { get; set; }

        /// <summary>
        /// Gets or sets the boiler output temperature details.
        /// </summary>
        public BoilerWiringBoiler? Boiler { get; set; }
    }

    /// <summary>
    /// Represents the device wired to a boiler.
    /// </summary>
    public class BoilerWiredDevice
    {
        /// <summary>
        /// Gets or sets the device type.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the device serial number.
        /// </summary>
        public string? SerialNo { get; set; }

        /// <summary>
        /// Gets or sets the therm interface type.
        /// </summary>
        public string? ThermInterfaceType { get; set; }

        /// <summary>
        /// Gets or sets whether the device is connected.
        /// </summary>
        public bool? Connected { get; set; }

        /// <summary>
        /// Gets or sets the last request timestamp.
        /// </summary>
        public DateTime? LastRequestTimestamp { get; set; }
    }

    /// <summary>
    /// Represents boiler telemetry nested under the boiler wiring installation state.
    /// </summary>
    public class BoilerWiringBoiler
    {
        /// <summary>
        /// Gets or sets the current boiler output temperature.
        /// </summary>
        public BoilerOutputTemperature? OutputTemperature { get; set; }
    }

    /// <summary>
    /// Represents a boiler output temperature reading.
    /// </summary>
    public class BoilerOutputTemperature
    {
        /// <summary>
        /// Gets or sets the output temperature in Celsius.
        /// </summary>
        public double? Celsius { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the reading.
        /// </summary>
        public DateTime? Timestamp { get; set; }
    }
}
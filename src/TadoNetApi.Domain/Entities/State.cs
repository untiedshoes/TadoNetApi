namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// State of a specific zone
    /// </summary>
    public class State
    {
        /// <summary>
        /// The current Tado mode (e.g., HOME, AWAY)
        /// </summary>
        public string? TadoMode { get; set; }

        /// <summary>
        /// Indicates whether geolocation override is active
        /// </summary>
        public bool? GeolocationOverride { get; set; }

        /// <summary>
        /// The time when geolocation override will be disabled
        /// </summary>
        public object? GeolocationOverrideDisableTime { get; set; }

        /// <summary>
        /// Preparation state information (if any)
        /// </summary>
        public object? Preparation { get; set; }

        /// <summary>
        /// The current setting applied to the zone
        /// </summary>
        public Setting? Setting { get; set; }

        /// <summary>
        /// The type of overlay currently active
        /// </summary>
        public string? OverlayType { get; set; }

        /// <summary>
        /// The overlay configuration currently applied
        /// </summary>
        public Overlay? Overlay { get; set; }

        /// <summary>
        /// Information about an open window event (if any)
        /// </summary>
        public object? OpenWindow { get; set; }

        /// <summary>
        /// Indicates whether an open window has been detected
        /// </summary>
        public bool? OpenWindowDetected { get; set; }

        /// <summary>
        /// The link state of the zone
        /// </summary>
        public Link? Link { get; set; }

        /// <summary>
        /// Activity data points such as heating power
        /// </summary>
        public ActivityDataPoints? ActivityDataPoints { get; set; }

        /// <summary>
        /// Sensor data points such as temperature and humidity
        /// </summary>
        public SensorDataPoints? SensorDataPoints { get; set; }
    }
}
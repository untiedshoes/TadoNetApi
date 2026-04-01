namespace TadoNetApi.Domain.Entities.MobileDevice
{
    /// <summary>
    /// Contains settings specific to the device
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Indicates whether geolocation tracking is enabled for the device
        /// </summary>
        public bool? GeoTrackingEnabled { get; set; }
    }
}
using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses.MobileDevice
{
    /// <summary>
    /// Contains settings specific to the device
    /// </summary>
    public class TadoMobileSettingsResponse
    {
        /// <summary>
        /// Indicates whether geolocation tracking is enabled for the device
        /// </summary>
        [JsonPropertyName("geoTrackingEnabled")]
        public bool? GeoTrackingEnabled { get; set; }
    }
}
using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Requests
{
    /// <summary>
    /// Request payload for updating mobile device settings.
    /// </summary>
    public class SetMobileDeviceSettingsRequest
    {
        /// <summary>
        /// Indicates whether geo-tracking should be enabled for the mobile device.
        /// </summary>
        [JsonPropertyName("geoTrackingEnabled")]
        public bool? GeoTrackingEnabled { get; set; }
    }
}
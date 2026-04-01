using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses.MobileDevice
{
    /// <summary>
    /// Contains the location of a device
    /// </summary>
    public class TadoMobileLocationResponse
    {
        /// <summary>
        /// Indicates whether the location data is outdated
        /// </summary>
        [JsonPropertyName("stale")]
        public bool? Stale { get; set; }

        /// <summary>
        /// Indicates whether the device is currently at home
        /// </summary>
        [JsonPropertyName("atHome")]
        public bool? AtHome { get; set; }

        /// <summary>
        /// The direction from the home location to the device
        /// </summary>
        [JsonPropertyName("bearingFromHome")]
        public TadoMobileBearingFromHomeResponse? BearingFromHome { get; set; }

        /// <summary>
        /// The relative distance of the device from the home fence
        /// </summary>
        [JsonPropertyName("relativeDistanceFromHomeFence")]
        public double? RelativeDistanceFromHomeFence { get; set; }
    }
}
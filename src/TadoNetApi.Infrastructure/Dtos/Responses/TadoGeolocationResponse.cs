using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Represents the geographic location of a device or home
    /// </summary>
    public class TadoGeolocationResponse
    {
        /// <summary>
        /// The latitude coordinate
        /// </summary>
        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        /// <summary>
        /// The longitude coordinate
        /// </summary>
        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }
    }
}
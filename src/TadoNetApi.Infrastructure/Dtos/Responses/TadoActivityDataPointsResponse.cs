using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses {
    /// <summary>
    /// Contains activity data points such as heating power
    /// </summary>
    public class TadoActivityDataPointsResponse
    {
        /// <summary>
        /// The heating power data point
        /// </summary>
        [JsonPropertyName("heatingPower")]
        public TadoHeatingPowerResponse? HeatingPower { get; set; }
    }
}
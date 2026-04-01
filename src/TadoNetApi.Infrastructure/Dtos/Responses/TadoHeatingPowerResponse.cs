using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Represents the heating power data point of a device
    /// </summary>
    public class TadoHeatingPowerResponse
    {
        /// <summary>
        /// The type of heating power (e.g., percentage)
        /// </summary>
        [JsonPropertyName("type")]
        public string? CurrentType { get; set; }

        /// <summary>
        /// The percentage of heating power being used
        /// </summary>
        [JsonPropertyName("percentage")]
        public double? Percentage { get; set; }

        /// <summary>
        /// The timestamp when the heating power was recorded
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }
    }
}
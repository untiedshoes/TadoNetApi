using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Indicates whether the Early Start feature is enabled
    /// </summary>
    public class TadoEarlyStartResponse
    {
        /// <summary>
        /// Whether Early Start is enabled for the schedule
        /// </summary>
        [JsonPropertyName("enabled")]
        public bool? Enabled { get; set; }
    }
}
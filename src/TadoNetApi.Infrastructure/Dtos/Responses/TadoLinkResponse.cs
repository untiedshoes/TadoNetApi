using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Represents the link state of a Tado device or component
    /// </summary>
    public partial class TadoLinkResponse
    {
        /// <summary>
        /// The current state of the link (e.g., ONLINE, OFFLINE)
        /// </summary>
        [JsonPropertyName("state")]
        public string? State { get; set; }
    }
}
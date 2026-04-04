using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Represents the API response for a zone's default overlay configuration.
    /// </summary>
    public class TadoDefaultZoneOverlayResponse
    {
        /// <summary>
        /// The default termination condition for overlays created via a device interaction.
        /// </summary>
        [JsonPropertyName("terminationCondition")]
        public TadoTerminationResponse? TerminationCondition { get; set; }
    }
}
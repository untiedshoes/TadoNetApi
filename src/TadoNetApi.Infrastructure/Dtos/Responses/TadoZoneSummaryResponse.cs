using System.Text.Json.Serialization;
namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Summarized state of a zone
    /// </summary>
    public class TadoZoneSummaryResponse
    {
        /// <summary>
        /// The current state of the Tado device
        /// </summary>
        [JsonPropertyName("setting")]
        public TadoSettingResponse? Setting { get; set; }

        /// <summary>
        /// Information on when the current state of the Tado device will end
        /// </summary>
        [JsonPropertyName("termination")]
        public TadoTerminationResponse? Termination { get; set; }
    }
}
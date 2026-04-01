using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Represents the current overlay state of a Tado device
    /// </summary>
    public partial class TadoOverlayResponse
    {
        /// <summary>
        /// The current setting applied to the Tado device
        /// </summary>
        [JsonPropertyName("setting")]
        public TadoSettingResponse? Setting { get; set; }

        /// <summary>
        /// Information on when the current setting will end
        /// </summary>
        [JsonPropertyName("termination")]
        public TadoTerminationResponse? Termination { get; set; }
    }
}
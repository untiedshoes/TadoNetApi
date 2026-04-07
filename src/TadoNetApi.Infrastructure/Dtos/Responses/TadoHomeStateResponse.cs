using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Information about the state of the home
    /// </summary>
    public class TadoHomeStateResponse
    {
        [JsonPropertyName("presence")]
        public string Presence { get; set; } = string.Empty;
    }
}
using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Represents boiler presence and identity returned by the bridge-scoped boiler info route.
    /// </summary>
    public class TadoBoilerInfoResponse
    {
        [JsonPropertyName("boilerPresent")]
        public bool? BoilerPresent { get; set; }

        [JsonPropertyName("boilerId")]
        public int? BoilerId { get; set; }
    }
}
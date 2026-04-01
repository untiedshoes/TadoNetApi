using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Contains information about a home where Tado is being used
    /// </summary>
    public class TadoHomeResponse
    {
        /// <summary>
        /// The unique identifier of the home
        /// </summary>
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        /// <summary>
        /// The name of the home
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
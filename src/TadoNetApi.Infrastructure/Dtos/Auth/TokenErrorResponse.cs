using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Auth
{
    public class TokenErrorResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;

        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; } = string.Empty;
    }
}
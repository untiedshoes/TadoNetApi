using System.Text.Json.Serialization;
namespace TadoNetApi.Infrastructure.Dtos.Auth
{
    /// <summary>
    /// Represents an error response from Tado's token endpoint during device authorization.
    /// </summary>
    public class TokenErrorResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;

        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; } = string.Empty;
    }
}
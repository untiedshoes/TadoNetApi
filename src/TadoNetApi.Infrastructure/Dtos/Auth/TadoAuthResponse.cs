using System;
using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Auth
{
    public class TadoAuthResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        private DateTime? _expiryTime;

        [JsonIgnore]
        public DateTime ExpiryTime => _expiryTime ??= DateTime.UtcNow.AddSeconds(ExpiresIn);

        [JsonIgnore]
        public bool IsExpired => DateTime.UtcNow >= ExpiryTime;
    }
}
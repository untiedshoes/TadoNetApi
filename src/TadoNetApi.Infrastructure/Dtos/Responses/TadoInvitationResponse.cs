using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Responses
{
    /// <summary>
    /// Pending invitation payload returned by the Tado API.
    /// </summary>
    public class TadoInvitationResponse
    {
        /// <summary>
        /// The unique invitation token.
        /// </summary>
        [JsonPropertyName("token")]
        public string? Token { get; set; }

        /// <summary>
        /// The email address the invitation was sent to.
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// The timestamp when the invitation was first sent.
        /// </summary>
        [JsonPropertyName("firstSent")]
        public DateTime? FirstSent { get; set; }

        /// <summary>
        /// The timestamp when the invitation was last sent.
        /// </summary>
        [JsonPropertyName("lastSent")]
        public DateTime? LastSent { get; set; }

        /// <summary>
        /// The inviting user.
        /// </summary>
        [JsonPropertyName("inviter")]
        public TadoInvitationInviterResponse? Inviter { get; set; }
    }

    /// <summary>
    /// Inviting user details returned within an invitation payload.
    /// </summary>
    public class TadoInvitationInviterResponse
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [JsonPropertyName("enabled")]
        public bool? Enabled { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("homeId")]
        public long? HomeId { get; set; }

        [JsonPropertyName("locale")]
        public string? Locale { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("home")]
        public TadoHomeResponse? Home { get; set; }
    }
}
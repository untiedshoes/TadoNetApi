using System.Text.Json.Serialization;

namespace TadoNetApi.Infrastructure.Dtos.Requests;

/// <summary>
/// Request payload for sending a home invitation.
/// </summary>
public class SendInvitationRequest
{
    /// <summary>
    /// The email address that should receive the invitation.
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}
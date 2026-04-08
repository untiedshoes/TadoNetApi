namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Represents a pending home invitation.
    /// </summary>
    public class Invitation
    {
        /// <summary>
        /// The unique invitation token.
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// The email address the invitation was sent to.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// The timestamp when the invitation was first sent.
        /// </summary>
        public DateTime? FirstSent { get; set; }

        /// <summary>
        /// The timestamp when the invitation was last sent.
        /// </summary>
        public DateTime? LastSent { get; set; }

        /// <summary>
        /// The user who created the invitation.
        /// </summary>
        public InvitationInviter? Inviter { get; set; }
    }

    /// <summary>
    /// Represents the user who created a pending home invitation.
    /// </summary>
    public class InvitationInviter
    {
        /// <summary>
        /// The full name of the inviting user.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The email address of the inviting user.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// The username of the inviting user.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Indicates whether the inviting user account is enabled.
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// The unique identifier of the inviting user.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// The home ID associated with the inviting user.
        /// </summary>
        public long? HomeId { get; set; }

        /// <summary>
        /// The locale of the inviting user.
        /// </summary>
        public string? Locale { get; set; }

        /// <summary>
        /// The Tado account type of the inviting user.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// The home associated with the invitation.
        /// </summary>
        public Home? Home { get; set; }
    }
}
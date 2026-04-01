namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Contains information about a user
    /// </summary>
    public class User
    {
        /// <summary>
        /// The full name of the user
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The email address of the user
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// The username used by the user
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// The unique identifier of the user
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// The list of homes associated with the user
        /// </summary>
        public Home[]? Homes { get; set; }

        /// <summary>
        /// The locale or language preference of the user
        /// </summary>
        public string? Locale { get; set; }

        /// <summary>
        /// The list of mobile devices linked to the user
        /// </summary>
        public MobileDevice.Item[]? MobileDevices { get; set; }
    }
}
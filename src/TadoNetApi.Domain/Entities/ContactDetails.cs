namespace TadoNetApi.Domain.Entities
{
    /// <summary>
    /// Contains contact details of an owner of a house
    /// </summary>
    public class ContactDetails
    {
        /// <summary>
        /// The full name of the contact person
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The email address of the contact person
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// The phone number of the contact person
        /// </summary>
        public string? Phone { get; set; }
    }
}